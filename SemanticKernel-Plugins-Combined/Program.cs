using CommonStuff;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Planning.Handlebars;
using Microsoft.SemanticKernel.Plugins.Core;
using SemanticKernel_Plugins_Combined.Plugins;
using Console = CommonStuff.Console;

#region kernel build

var config = Common.GetConfig(typeof(Program).Assembly);
var kernel = Kernel
    .CreateBuilder()
    .AddOpenAIChatCompletion(config["OpenAI:ModelId"]!, config["OpenAI:ApiKey"]!)
    .Build();

#endregion

#region register plugins

//native plugins
kernel.Plugins.AddFromType<InvoicePlugin>();
kernel.Plugins.AddFromType<MailGenerationPlugin>();
#pragma warning disable SKEXP0050
kernel.Plugins.AddFromType<TimePlugin>();

//semantic plugins
kernel.ImportPluginFromPromptDirectory("Prompts");

#endregion

var chat = kernel.Services.GetRequiredService<IChatCompletionService>();

var chatHistory = new ChatHistory();

Console.WriteLineAsAi("How can I help you?");

while (true)
{
    var prompt = Console.GetUserPrompt();

    chatHistory.AddUserMessage(prompt);

    #pragma warning disable SKEXP0060
    var plannerOptions = new HandlebarsPlannerOptions
    {
        ExecutionSettings = new OpenAIPromptExecutionSettings
        {
            Temperature = 0.0,
            TopP = 0.1,
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
        }
    };

    if (string.IsNullOrWhiteSpace(prompt)) 
    {
        continue;
    }

    var planner = new HandlebarsPlanner(plannerOptions);

    var plan = await planner.CreatePlanAsync(kernel, prompt);
    Console.WriteLineAsSystem(plan.ToString());

    var result = (await plan.InvokeAsync(kernel)).Trim();
    Console.WriteAsAi(result);
    
    chatHistory.AddAssistantMessage(result);
}


//Get the open balance of the invoices for the last 10 days and give me the email message that I can send to Marina Decruyden to pay her open invoices.