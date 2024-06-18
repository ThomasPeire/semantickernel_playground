using CommonStuff;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Plugins.Core;
using SemanticKernel_Plugins_Combined.Plugins;

var config = Common.GetConfig(typeof(Program).Assembly);

var apiKey = config["OpenAI:ApiKey"]!;
var modelId = config["OpenAI:ModelId"]!;

var builder = Kernel.CreateBuilder().AddOpenAIChatCompletion(modelId, apiKey);

builder.Plugins.AddFromType<InvoicePlugin>();
builder.Plugins.AddFromType<MailGenerationPlugin>();
#pragma warning disable SKEXP0050
builder.Plugins.AddFromType<TimePlugin>();
#pragma warning restore SKEXP0050

var kernel = builder.Build();

kernel.ImportPluginFromPromptDirectory("Prompts");

var chat = kernel.Services.GetRequiredService<IChatCompletionService>();

var chatHistory = new ChatHistory();

Common.TalkAsAi("How can I help you?");

while (true)
{
    var prompt = Common.GetUserResponse();

    chatHistory.AddUserMessage(prompt);

    // Enable auto function calling
    OpenAIPromptExecutionSettings openAiPromptExecutionSettings = new()
    {
        ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
    };
    
    await foreach (var response in chat.GetStreamingChatMessageContentsAsync(
                       chatHistory,
                       executionSettings: openAiPromptExecutionSettings,
                       kernel: kernel))
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write(response);
    }

    Console.WriteLine();
}