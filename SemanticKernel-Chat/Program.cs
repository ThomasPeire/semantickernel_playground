using CommonStuff;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

var config = Common.GetConfig(typeof(Program).Assembly);

var apiKey = config["OpenAI:ApiKey"]!;
var modelId = config["OpenAI:ModelId"]!;

var builder = Kernel.CreateBuilder().AddOpenAIChatCompletion(modelId, apiKey);

var kernel = builder.Build();

var chat = kernel.Services.GetRequiredService<IChatCompletionService>();

var chatHistory = new ChatHistory();

//Give context to the AI
chatHistory.AddSystemMessage(
    "My agenda for today looks like this: 10h standup, 12h lunch, 15h meeting with the boss, 17h code review, 19h dinner with my girlfriend.");
chatHistory.AddSystemMessage(
    "You are a very professional personal assistant for a developer. But you answer everything as Mario from the Super Mario series");

Common.TalkAsAi("How can I help you?");

while (true)
{
    var prompt = Common.GetUserResponse();

    chatHistory.AddUserMessage(prompt);

    var fullResponse = string.Empty;
    await foreach (var response in chat.GetStreamingChatMessageContentsAsync(chatHistory))
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write(response);
        fullResponse += response;
    }
    
    chatHistory.AddAssistantMessage(fullResponse);

    Console.WriteLine();
}
