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
}

public static class Console
{
    public static void WriteLineAsAi(string message)
    {
        System.Console.ForegroundColor = ConsoleColor.Green;
        System.Console.WriteLine($"AI: {message}");
    }
    public static void WriteLineAsSystem(string message)
    {
        System.Console.ForegroundColor = ConsoleColor.DarkRed;
        System.Console.WriteLine($"System: {message}");
    }

    public static string GetUserPrompt()
    {
        System.Console.WriteLine();
        System.Console.ForegroundColor = ConsoleColor.Gray;
        System.Console.Write("User: ");
        return System.Console.ReadLine() ?? string.Empty;
    }

    public static void WriteAsAi(string message)
    {
        System.Console.ForegroundColor = ConsoleColor.Green;
        System.Console.Write(message);
    }
}