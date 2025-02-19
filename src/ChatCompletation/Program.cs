#pragma warning disable SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable CS0618 // Type or member is obsolete
#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

using OllamaSharp;

using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();

const string ModelName = "llama3.2";
const string ContinueMessage = "Press Enter to continue...";
const string Thinking = ">>> Thinking...";

var ollamaUri = new Uri("http://localhost:11434");
ChatMessageContent? reply = null;

Console.WriteLine("======== Ollama - Chat Completion ========");

// Create Ollama API client
var ollamaClient = new OllamaApiClient(
    uri: ollamaUri,
    defaultModel: ModelName);

// Get ChatCompletion service
var chatService = ollamaClient.AsChatCompletionService();

// Initialize chat history
var chatHistory = new ChatHistory("You are a helpful assistant that knows about AI.");

// First user message
chatHistory.AddUserMessage("I'm looking for a book about AI");
OutputMessage(chatHistory);

// First assistant message 
Console.WriteLine(Thinking);
reply = await chatService.GetChatMessageContentAsync(chatHistory);
chatHistory.Add(reply);
OutputMessage(chatHistory);

// Second user message
chatHistory.AddUserMessage("I want to learn more about neural networks");
OutputMessage(chatHistory);

// Second assistant message
Console.WriteLine(Thinking);
reply = await chatService.GetChatMessageContentAsync(chatHistory);
chatHistory.Add(reply);
OutputMessage(chatHistory, false);


Console.WriteLine("\n======== End of Chat ========");

static void OutputMessage(ChatHistory chats, bool waitEnter = true)
{
    var chat = chats.Last();
    Console.WriteLine($"{chat.Role}: {chat}");

    if (waitEnter)
    {
        Console.WriteLine(ContinueMessage);
        Console.ReadLine();
    }
}


//var response = chatService.GetStreamingChatMessageContentsAsync(
//    chatHistory: chatHistory,
//    kernel: kernel);

//await foreach (var chat in response)
//{
//    Console.WriteLine(chat);
//}