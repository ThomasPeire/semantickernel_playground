﻿using CommonStuff;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.Core;

var config = Common.GetConfig(typeof(Program).Assembly);

var apiKey = config["OpenAI:ApiKey"]!;
var modelId = config["OpenAI:ModelId"]!;

var builder = Kernel.CreateBuilder().AddOpenAIChatCompletion(modelId, apiKey);

var kernel = builder.Build();
var prompts = kernel.ImportPluginFromPromptDirectory("Prompts");
#pragma warning disable SKEXP0050
builder.Plugins.AddFromType<ConversationSummaryPlugin>();
#pragma warning restore SKEXP0050

const string input = "Generate me an email to ask customer Marina Decruyden to pay the invoice of 150 euros.";

var result = await kernel.InvokeAsync(prompts["GetUnpaidInvoicesMail"], new() { { "input", input } });

Console.WriteLine(result);