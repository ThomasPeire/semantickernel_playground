using CommonStuff;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Plugins.Core;
using SemanticKernel.Plugins;
using Console = CommonStuff.Console;

#region kernel build

var config = Common.GetConfig(typeof(Program).Assembly);
var kernel = Kernel
    .CreateBuilder()
    .AddOpenAIChatCompletion(config["OpenAI:ModelId"]!, config["OpenAI:ApiKey"]!)
    .Build();

#endregion

#region register plugins

kernel.Plugins.AddFromType<InvoicePlugin>();

#pragma warning disable SKEXP0050
kernel.Plugins.AddFromType<TimePlugin>();

#endregion

var chat = kernel.Services.GetRequiredService<IChatCompletionService>();
var chatHistory = new ChatHistory();

Console.WriteLineAsAi("How can I help you?");

while (true)
{
    var prompt = Console.GetUserPrompt();

    chatHistory.AddUserMessage(prompt);

    // Enable auto function calling
    OpenAIPromptExecutionSettings openAiPromptExecutionSettings = new()
    {
        ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
    };

    var fullResponse = string.Empty;
    await foreach (var response in chat.GetStreamingChatMessageContentsAsync(
                       chatHistory,
                       executionSettings: openAiPromptExecutionSettings,
                       kernel: kernel))
    {
        Console.WriteAsAi(response.ToString());
        fullResponse += response;
    }
    
    chatHistory.AddAssistantMessage(fullResponse);
}