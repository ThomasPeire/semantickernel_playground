using CommonStuff;
using Microsoft.SemanticKernel;
using Console = CommonStuff.Console;

#region kernel build

var config = Common.GetConfig(typeof(Program).Assembly);
var kernel = Kernel
    .CreateBuilder()
    .AddOpenAIChatCompletion(config["OpenAI:ModelId"]!, config["OpenAI:ApiKey"]!)
    .Build();

#endregion

var pluginFunctions = kernel.ImportPluginFromPromptDirectory("Prompts");

var arguments = new KernelArguments
{
    { "customer_name", "Marina Decruyden" },
    { "balance", "150" }
};

var result = await kernel.InvokeAsync(pluginFunctions["GetUnpaidInvoicesMail"], arguments);

Console.WriteLineAsAi(result.ToString());