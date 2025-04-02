#pragma warning disable SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable CS0618 // Type or member is obsolete
#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
using ChatCompletionStreaming;
using ChatCompletionStreaming.Data;
using ChatCompletionStreaming.Filters;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;

using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.Debug()
    .CreateLogger();

//const string ModelName = "deepseek-r1";
//const string ModelName = "mistral";
const string ModelName = "llama3.2";
const string Thinking = ">>> Thinking...";

// **************************************
// TODO: 2.1 Create the Kernel
var ollamaUri = new Uri("http://localhost:11434");

var kernelBuilder = Kernel.CreateBuilder()
           .AddOllamaChatCompletion(
               endpoint: ollamaUri,
               modelId: ModelName);

// **************************************
// TODO: 2.2 Dependency Injection
kernelBuilder.Services.AddSingleton<IPartCatalogService, PartCatalogService>();

// Reference: https://learn.microsoft.com/en-us/training/modules/combine-prompts-functions/4-filter-invoked-functions
kernelBuilder.Services.AddSingleton<IFunctionInvocationFilter, LoggingFilter>();

kernelBuilder.Plugins.AddFromType<PartCatalogPlugin>(nameof(PartCatalogPlugin));

var kernel = kernelBuilder.Build();


// ********************************************************
// Chat configuration
var chatService = kernel.GetRequiredService<IChatCompletionService>();

Console.WriteLine("======== Part Catalog Assistant ========");

// **************************************
// TODO: 2.4 Create System Message (Chat context)
var chatHistory = new ChatHistory(@$"
Instructions: 
- You are a friendly agent dedicated to providing part number catalog information for the aircraft industry, supporting the sales team by making queries to get part information.
- Focus solely on these tasks and refrain from responding to any unrelated inquiries.
- Do not answer any questions, queries, or requests unrelated to part number information.
- Assist the user with querying part numbers.
- The abbreviation PN stands for Part Number.
- You can create email text, if user do this requests.

Choices: 
- {PartCatalogPlugin.RetrivePartNumberRecordFuncName}
- {PartCatalogPlugin.CreateEmailTextFuncName}
- {PartCatalogPlugin.CreateQuoteFuncName}

Examples:

User Input: Check part number ABC123
Intent: {PartCatalogPlugin.RetrivePartNumberRecordFuncName}
Assistant Response: Part Number ABC123 'DESCRIPTION' is in stock and here are the details:
    Available Quantity: 10
    Price: $1,235.34
    Condition Code: XYZ
    Warehouse: XYZ

User Input: Create email text for part number ABC123
Intent: {PartCatalogPlugin.CreateEmailTextFuncName}

User Input: Create a quote for part number {{$part_number}} for customer {{$customer_name}} and quantity {{$quantity}}
Intent: {PartCatalogPlugin.CreateQuoteFuncName}
Assistant Response: Customer {{$customer_name}} Quote for Part Number: {{$part_number}}
----------------------------------------
Description: {{{PartCatalogPlugin.CreateQuoteFuncName} $description}}
Quantity: {{{PartCatalogPlugin.CreateQuoteFuncName} $quantity}}
Total Price: {{{PartCatalogPlugin.CreateQuoteFuncName} $unit_price}}
Condition Code: {{{PartCatalogPlugin.CreateQuoteFuncName} $condition_code}}
Warehouse: {{{PartCatalogPlugin.CreateQuoteFuncName} $warehouse_name}}
----------------------------------------
Thank you for your inquiry. If you have any further questions or need additional assistance, please let us know.
");

// Add System Message 
chatService.GetStreamingChatMessageContentsAsync(
    chatHistory: chatHistory,
    kernel: kernel);

// Configure Prompt
var ollamaExecutionSettings = new OllamaPromptExecutionSettings()
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
    Temperature = 0.9f
};

// Initiate a back-and-forth chat
string? userInput;

do
{
    // Collect user input
    Console.Write("User > ");
    userInput = Console.ReadLine();
    if (userInput is null || userInput.Equals("bye", StringComparison.InvariantCultureIgnoreCase)) break;

    // Add user input
    chatHistory.AddUserMessage(userInput!);
    Console.WriteLine(Thinking);

    // Get the response from the AI
    var result = await chatService.GetChatMessageContentAsync(
        chatHistory,
        executionSettings: ollamaExecutionSettings,
        kernel: kernel);

    // Print the results
    Console.WriteLine($"{result.Role} > " + result);

    // Add the message from the agent to the chat history
    chatHistory.AddMessage(result.Role, result.Content ?? string.Empty);
} while (userInput is not null);

Console.WriteLine("\n======== End of Chat ========");
