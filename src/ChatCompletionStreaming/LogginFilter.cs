using System.Diagnostics;

using Microsoft.SemanticKernel;

namespace ChatCompletionStreaming;
public sealed class LoggingFilter() : IFunctionInvocationFilter
{
    public async Task OnFunctionInvocationAsync(FunctionInvocationContext context, Func<FunctionInvocationContext, Task> next)
    {
        Debug.WriteLine($">> Invoking: {context.Function.PluginName}.{context.Function.Name}");

        await next(context);

        Debug.WriteLine($">> Invoking: {context.Function.PluginName}.{context.Function.Name} | Result: {context.Result}");
    }
}