#pragma warning disable SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable CS0618 // Type or member is obsolete
#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
using ChatCompletionStreaming;
using ChatCompletionStreaming.Data;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;

using Serilog;


////setup our DI
//var serviceProvider = new ServiceCollection()
//    .AddSingleton<IPartCatalogService, PartCatalogService>()
//    .BuildServiceProvider();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.Debug()
    .CreateLogger();

//const string ModelName = "deepseek-r1";
const string ModelName = "llama3.2";
//const string ModelName = "mistral";
const string Thinking = ">>> Thinking...";

var ollamaUri = new Uri("http://localhost:11434");

var kernelBuilder = Kernel.CreateBuilder()
           .AddOllamaChatCompletion(
               endpoint: ollamaUri,
               modelId: ModelName);

kernelBuilder.Services.AddSingleton<IFunctionInvocationFilter, LoggingFilter>();
kernelBuilder.Services.AddSingleton<IPartCatalogService, PartCatalogService>();

kernelBuilder.Plugins.AddFromType<PartCatalogPlugin>(nameof(PartCatalogPlugin));

var kernel = kernelBuilder.Build();

//Get pluging functions 
var retrivePartNumberRecordFunc = kernel.Plugins.GetFunction(nameof(PartCatalogPlugin), PartCatalogPlugin.RetrivePartNumberRecordFuncName);
var generateEmailTextFunc = kernel.Plugins.GetFunction(nameof(PartCatalogPlugin), PartCatalogPlugin.CreateEmailTextFuncName);

var chatService = kernel.GetRequiredService<IChatCompletionService>();

Console.WriteLine("======== Ollama - Chat Completion Streaming ========");

// Create a history store the conversation
var chatHistory = new ChatHistory(@$"""
Instructions: 
- You are a friendly agent dedicated to providing part number catalog information for the aircraft industry, supporting the sales team by making queries to get part information.
- Focus solely on these tasks and refrain from responding to any unrelated inquiries.
- Do not answer any questions, queries, or requests unrelated to part number information.
- Assist the user with querying part numbers.
- The abbreviation PN stands for Part Number.
- You can create email text, if user do this requests.

Choices: {PartCatalogPlugin.RetrivePartNumberRecordFuncName}, {PartCatalogPlugin.CreateEmailTextFuncName}

User Input: Check part number ABC123
Intent: {PartCatalogPlugin.RetrivePartNumberRecordFuncName}
Assistant Response: Part Number ABC123 'DESCRIPTION' is in stock and here are the details:
    Available Quantity: 10
    Price: $1,235.34
    Condition Code: XYZ
    Warehouse: XYZ

User Input: Create email text for part number ABC123
Intent: {PartCatalogPlugin.CreateEmailTextFuncName}

""");

// Add System Message 
chatService.GetStreamingChatMessageContentsAsync(
    chatHistory: chatHistory,
    kernel: kernel);

// Configure Prompt
OllamaPromptExecutionSettings ollamaExecutionSettings = new()
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(), // FunctionChoiceBehavior.Auto(functions: [retrivePartNumberRecordFunc, generateEmailTextFunc]),
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
