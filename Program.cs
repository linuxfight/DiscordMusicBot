using System.Reflection;
using Discord;
using Discord.WebSocket;
using DiscordMusicBot.Bot;
using DiscordMusicBot.Utility;
using DiscordMusicBot.Utility.Cobalt;
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
    UseInteractionSnowflakeDate = false,
    GatewayIntents = GatewayIntents.All
};
services.AddSingleton<DiscordSocketClient>(_ => new(config));
services.AddSingleton<VoiceState>();

// Cobalt and YouTube API configuration
services.AddScoped<CobaltApiClient>(_ => new("https://api.cobalt.tools"));
services.AddScoped<YoutubeApiClient>();

// Commands registration
List<Command> commands = new();
Type commandType = typeof(Command);
List<Type> types = Assembly.GetExecutingAssembly().GetTypes()
    .Where(t => t is { IsClass: true, IsAbstract: false } && t.IsSubclassOf(commandType)).ToList();
foreach (Type type in types)
{
    if (Activator.CreateInstance(type) is Command command)
        commands.Add(command);
}
services.AddSingleton(commands);

// Build DI
IServiceProvider serviceProvider = services.BuildServiceProvider();

// Adding handlers
Handler handler = new(serviceProvider);
DiscordSocketClient client = serviceProvider.GetRequiredService<DiscordSocketClient>();
client.Log += Logger.Log;
client.Ready += handler.Ready;
client.SlashCommandExecuted += handler.SlashCommand;

// Start bot
await client.LoginAsync(TokenType.Bot, token);
await client.StartAsync();
await Task.Delay(-1);

