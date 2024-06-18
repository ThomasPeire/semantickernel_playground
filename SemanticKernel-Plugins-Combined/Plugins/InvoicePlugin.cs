using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace SemanticKernel_Plugins_Combined.Plugins;

public sealed class InvoicePlugin
{
    [KernelFunction, Description("Returns the unpaid balance of invoices in euros since a given date.")]
    [return:Description("The total amount of unpaid invoices.")]
    public static decimal UnpaidAmountSince([Description("The startdate from when we want to take the invoices into account")] DateTime date)
    {
        return DbContext.Invoices
            .Where(invoice => invoice.InvoiceDate.Date >= date.Date)
            .Sum(invoice => invoice.AmountDue);
    }
}

internal record Invoice(decimal AmountDue, DateTime InvoiceDate);

internal static class DbContext
{
    public static readonly List<Invoice> Invoices =
    [
        new Invoice(80, DateTime.Today.AddDays(0)),
        new Invoice(0, DateTime.Today.AddDays(-5)),
        new Invoice(20, DateTime.Today.AddDays(-10)),
        new Invoice(0, DateTime.Today.AddDays(-15)),
        new Invoice(69, DateTime.Today.AddDays(-100)),
    ];
}