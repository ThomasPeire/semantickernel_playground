using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace SemanticKernel_Plugins_Combined.Plugins;

public sealed class MailGenerationPlugin
{
    [KernelFunction, Description("Generates an email message to urge a customer to pay his or her open invoices.")]
    public async Task<string> GenerateUnpaidInvoicesMailMessage(
        Kernel kernel,
        [Description("The name of the customer")] string customerName,
        [Description("The open balance in euros")] string openBalance)
    {
        if (!kernel.Plugins.TryGetPlugin("Prompts", out var prompts) || !prompts.TryGetFunction("GetUnpaidInvoicesMailSpecific", out var function)) 
            return string.Empty;
        
        var result = await kernel.InvokeAsync(function,
            new KernelArguments
            {
                { "customer_name", customerName }, 
                { "balance", openBalance }
            });
            
        return result.ToString();

    }
}