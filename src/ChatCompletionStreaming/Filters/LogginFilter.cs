﻿using System.Diagnostics;

using Microsoft.SemanticKernel;

namespace ChatCompletionStreaming.Filters;
// **************************************
// TODO: 2.3 Logging filter

/// <summary>
/// A filter that logs the invocation of a function.
/// </summary>
public sealed class LoggingFilter() : IFunctionInvocationFilter
{
    public async Task OnFunctionInvocationAsync(FunctionInvocationContext context, Func<FunctionInvocationContext, Task> next)
    {
        Debug.WriteLine($">> Invoking: {context.Function.PluginName}.{context.Function.Name}");

        await next(context);

        Debug.WriteLine($">> Invoking: {context.Function.PluginName}.{context.Function.Name} | Result: {context.Result}");
    }
}