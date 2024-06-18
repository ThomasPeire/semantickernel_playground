using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace CommonStuff;

public static class Common
{
    public static IConfigurationRoot GetConfig(Assembly assembly)
    {
        return new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddUserSecrets(assembly)
            .Build();
    }
    
    public static void TalkAsAi(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"AI: {message}");
    }

    public static string GetUserResponse()
    {
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write("User: ");
        return Console.ReadLine() ?? string.Empty;
    }
}