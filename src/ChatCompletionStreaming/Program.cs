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

kernelBuilder.Services.AddSingleton<IPartCatalogService, PartCatalogService>();

kernelBuilder.Plugins.AddFromType<PartCatalogPlugin>(nameof(PartCatalogPlugin));

var kernel = kernelBuilder.Build();

var chatService = kernel.GetRequiredService<IChatCompletionService>();

Console.WriteLine("======== Ollama - Chat Completion Streaming ========");

// Create a history store the conversation
var chatHistory = new ChatHistory(@$"""
Instructions: 
- You are a friendly agent dedicated to the part catalog for the aircraft industry, supporting the sales team by making queries to get part information.
- Focus solely on these tasks and refrain from responding to any unrelated inquiries.
- Do not answer any questions, queries, or requests unrelated to part number information.
- Assist the user with querying part numbers.
- Support the user in generating text for a quote email to send to customers, but only if the user asks for it.
- The abbreviation PN stands for Part Number.
- If you receive a part number, display the part details.

Choices: {PartCatalogPlugin.RetrivePartNumberRecordFuncName}, {PartCatalogPlugin.GenerateEmailTextFuncName}

User Input: Can you check the inventory of the part number ABC123?
Intent: {PartCatalogPlugin.RetrivePartNumberRecordFuncName}
Assistant Response: Part ABC123 is in stock and here are the details:
    Quantity: 10
    UnitOfMeasure: EA
    Price: 100.00
    Warehouse: A
    WarehouseCountry: USA
    IsHazmat: False

User Input: Check part number ABC123
Intent: {PartCatalogPlugin.RetrivePartNumberRecordFuncName}
Assistant Response: Part ABC123 is in stock and here are the details:
    Quantity: 10
    UnitOfMeasure: EA
    Price: 100.00
    Warehouse: A
    WarehouseCountry: USA
    IsHazmat: False
        
User Input: Can you generate email text?
Intent: {PartCatalogPlugin.GenerateEmailTextFuncName}
""");

var response = chatService.GetStreamingChatMessageContentsAsync(
    chatHistory: chatHistory,
    kernel: kernel);

// Enable planning
OllamaPromptExecutionSettings ollamaExecutionSettings = new()
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
    Temperature = 0.9f
};

// Initiate a back-and-forth chat
string? userInput;

//CheckPn("SR09270005-7001").GetAwaiter().GetResult();

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
