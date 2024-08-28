using System.Reflection;
using Discord;
using Discord.WebSocket;
using DiscordMusicBot.Bot;
using DiscordMusicBot.Bot.Utility;
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

// Commands registration
List<Command> commands = new();
// will have to use reflection, because native aot is not supported by discord.net
/*
List<Command> commands = new()
{
    new LoopCommand(),
    new PingCommand(),
    new PlayCommand(),
    new SkipCommand(),
    new StatusCommand(),
    new StopCommand()
};
*/
// Disabling reflection opens a way to native AoT compilation
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
client.UserVoiceStateUpdated += handler.VoiceState;

// Start bot
await client.LoginAsync(TokenType.Bot, token);
await client.StartAsync();
await Task.Delay(-1);

// Stop bot
await serviceProvider.GetRequiredService<VoiceState>().Stop();
await client.StopAsync();
await client.LogoutAsync();

