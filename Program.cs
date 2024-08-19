using System.Reflection;
using Discord;
using Discord.WebSocket;
using DiscordMusicBot;
using DiscordMusicBot.Bot;
using DiscordMusicBot.Utility.CobaltApi;
using Microsoft.Extensions.DependencyInjection;

// Token configuration
string? token = Environment.GetEnvironmentVariable("TOKEN");
if (token == null)
{
    Console.WriteLine("TOKEN env var is not set");
    Environment.Exit(1);
}

// Create DI
ServiceCollection services = new();

// Client configuration
DiscordSocketConfig config = new()
{
    UseInteractionSnowflakeDate = false
};
DiscordSocketClient client = new(config);
services.AddSingleton(client);

// Cobalt API configuration
services.AddScoped<Client>(_ => new("https://api.cobalt.tools"));

// Commands registration
List<Command> commands = new();
Type commandType = typeof(Command);
List<Type> types = Assembly.GetExecutingAssembly().GetTypes()
    .Where(t => t is { IsClass: true, IsAbstract: false } && t.IsSubclassOf(commandType)).ToList();
foreach (Type type in types)
    commands.Add((Command)Activator.CreateInstance(type)!);
services.AddSingleton(commands);

// Build DI
IServiceProvider serviceProvider = services.BuildServiceProvider();

// Adding handlers
Handler handler = new(serviceProvider);
client.Log += Logger.Log;
client.Ready += handler.Ready;
client.SlashCommandExecuted += handler.SlashCommand;

// Start bot
await client.LoginAsync(TokenType.Bot, token);
await client.StartAsync();
await Task.Delay(-1);

