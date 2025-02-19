#pragma warning disable SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable CS0618 // Type or member is obsolete
#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;

using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();

//const string ModelName = "llama3.2";
const string ModelName = "mistral";
const string Thinking = ">>> Thinking...";

var ollamaUri = new Uri("http://localhost:11434");

var kernel = Kernel.CreateBuilder()
           .AddOllamaChatCompletion(
               endpoint: ollamaUri,
               modelId: ModelName)
           .Build();

var chatService = kernel.GetRequiredService<IChatCompletionService>();

Console.WriteLine("======== Ollama - Chat Completion Streaming ========");

// Create a history store the conversation
var chatHistory = new ChatHistory();

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
