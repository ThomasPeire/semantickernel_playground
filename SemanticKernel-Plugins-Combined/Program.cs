using CommonStuff;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Planning.Handlebars;
using Microsoft.SemanticKernel.Plugins.Core;
using SemanticKernel_Plugins_Combined.Plugins;
#pragma warning disable SKEXP0060
#pragma warning disable SKEXP0050

var config = Common.GetConfig(typeof(Program).Assembly);

var apiKey = config["OpenAI:ApiKey"]!;
var modelId = config["OpenAI:ModelId"]!;

var builder = Kernel.CreateBuilder().AddOpenAIChatCompletion(modelId, apiKey);

builder.Plugins.AddFromType<InvoicePlugin>();
builder.Plugins.AddFromType<MailGenerationPlugin>();
builder.Plugins.AddFromType<TimePlugin>();

var kernel = builder.Build();

kernel.ImportPluginFromPromptDirectory("Prompts");

var chat = kernel.Services.GetRequiredService<IChatCompletionService>();

var chatHistory = new ChatHistory();

Common.TalkAsAi("How can I help you?");

while (true)
{
    var prompt = Common.GetUserResponse();

    chatHistory.AddUserMessage(prompt);
    //Enable auto function calling
    OpenAIPromptExecutionSettings openAiPromptExecutionSettings = new()
    {
        ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
    };
    var plannerOptions = new HandlebarsPlannerOptions()
    {
        ExecutionSettings = new OpenAIPromptExecutionSettings()
        {
            Temperature = 0.0,
            TopP = 0.1,
        }
    };
    
    var planner = new HandlebarsPlanner(plannerOptions);
    
    var plan = await planner.CreatePlanAsync(kernel, prompt);
    Console.ForegroundColor = ConsoleColor.DarkRed;
    Console.WriteLine($"Plan: {plan}");
    Console.ReadLine();
    
    //var result = (await plan.InvokeAsync(kernel)).Trim();

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


//Get the open balance of the invoices of the last 10 days and send give me the email message that I can send to Marina Decruyden to pay her open invoices.