using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using SlashCommandBuilder = Discord.SlashCommandBuilder;

namespace DiscordMusicBot.Bot;

public class Handler(IServiceProvider serviceProvider)
{
    private List<Command> _commands = serviceProvider.GetRequiredService<List<Command>>();
    
    public async Task SlashCommand(SocketSlashCommand slashCommand)
    {
        Command? command = _commands.FirstOrDefault(x => x.Name == slashCommand.CommandName);
        if (command == null || command.Handler == null)
            await slashCommand.RespondAsync("unknown command");
        else
            await command.Handler(slashCommand, serviceProvider);
    }

    public async Task Ready()
    {
        DiscordSocketClient discordSocketClient = serviceProvider.GetRequiredService<DiscordSocketClient>();
        IReadOnlyCollection<SocketApplicationCommand> existingCommands = await discordSocketClient.GetGlobalApplicationCommandsAsync();
        foreach (SocketApplicationCommand existingCommand in existingCommands)
            await existingCommand.DeleteAsync();
        foreach (Command command in _commands)
        {
            SlashCommandBuilder commandBuilder = new();
            commandBuilder.WithName(command.Name);
            commandBuilder.WithDescription(command.Description);
            if (command.Parameters != null)
                foreach (Parameter parameter in command.Parameters)
                    commandBuilder.AddOption(parameter.Name, parameter.Type, parameter.Description, parameter.Required);
            await discordSocketClient.CreateGlobalApplicationCommandAsync(commandBuilder.Build());
        }
    }
}