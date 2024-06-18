using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace SemanticKernel_Plugins_Combined.Plugins;

public sealed class InvoicePlugin
{
    private static readonly List<Invoice> Invoices =
    [
        new Invoice(0, 80, DateTime.Today.AddDays(0)),
        new Invoice(150, 0, DateTime.Today.AddDays(-5)),
        new Invoice(100, 20, DateTime.Today.AddDays(-10)),
        new Invoice(50, 0, DateTime.Today.AddDays(-15)),
        new Invoice(99, 69, DateTime.Today.AddDays(-100)),
    ];
    
    [KernelFunction, Description("Returns the unpaid balance of invoices in euros since a given date.")]
    [return:Description("The total amount of unpaid invoices.")]
    public static decimal UnpaidAmountSince([Description("The date to start summing the unpaid amount")] DateTime date)
    {
        return Invoices
            .Where(invoice => invoice.InvoiceDate >= date)
            .Sum(invoice => invoice.AmountDue);
    }
    
}

internal record Invoice(decimal AmountPaid, decimal AmountDue, DateTime InvoiceDate);