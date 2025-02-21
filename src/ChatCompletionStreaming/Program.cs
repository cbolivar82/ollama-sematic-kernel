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
var chatHistory = new ChatHistory(@$"

    You are a dedicated agent of aircraft industry to support sales team. 
    Sales team is looking for a part number in the inventory and the member name is 'Tony{DateTime.Now.Ticks}'.
    Your primary responsibilities are to assist with querying part numbers and write email for customers to submit a quote of the part number with details information. 
    If user enter an part number and you couldn't found it then display a message 'Part out of stock' for example.
    Don't forget to write a email text content for a customer with a proposal quote related to the part number. Only if the user ask for the email.
    Don't display any unrelated part number information.
    Please focus solely on these tasks and refrain from responding to any unrelated inquiries.");

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
