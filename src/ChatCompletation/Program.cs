#pragma warning disable SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable CS0618 // Type or member is obsolete
#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

using Microsoft.SemanticKernel;

using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();

const string ModelName = "llama3.2";
const string ContinueMessage = "Press Enter to continue...";
const string Thinking = ">>> Thinking... \n\n";

var ollamaUri = new Uri("http://localhost:11434");

var kernelBuilder = Kernel.CreateBuilder()
           .AddOllamaChatCompletion(
               endpoint: ollamaUri,
               modelId: ModelName);
var kernel = kernelBuilder.Build();

Console.WriteLine("======== Chat Completion ========");

// TODO: 1.1 Invoke a Prompt
string promptOne = "What is the capital city of the USA?";
OutputMessage("User", promptOne);

Console.WriteLine(Thinking);
var result = await kernel.InvokePromptAsync(promptOne);
OutputMessage("Assistant", result.ToString());

Console.WriteLine("\n======== End of Chat ========");

static void OutputMessage(string role, string text, bool waitEnter = true)
{

    Console.WriteLine($"{role} >>> {text}");

    if (waitEnter)
    {
        Console.WriteLine(ContinueMessage);
        Console.ReadLine();
    }
}
