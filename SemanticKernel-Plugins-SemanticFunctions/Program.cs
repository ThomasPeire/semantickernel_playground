using CommonStuff;
using Microsoft.SemanticKernel;
#pragma warning disable SKEXP0050

var config = Common.GetConfig(typeof(Program).Assembly);

var apiKey = config["OpenAI:ApiKey"]!;
var modelId = config["OpenAI:ModelId"]!;

var builder = Kernel.CreateBuilder().AddOpenAIChatCompletion(modelId, apiKey);

var kernel = builder.Build();

var pluginFunctions = kernel.ImportPluginFromPromptDirectory("Prompts");

var arguments = new KernelArguments
{
    { "customer_name", "Marina Decruyden" },
    { "balance", "150" }
};

var result = await kernel.InvokeAsync(pluginFunctions["GetUnpaidInvoicesMail"], arguments);
Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine(result);