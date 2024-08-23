using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using SlashCommandBuilder = Discord.SlashCommandBuilder;

namespace DiscordMusicBot.Bot;

public class Handler(IServiceProvider serviceProvider)
{
    private readonly List<Command> _commands = serviceProvider.GetRequiredService<List<Command>>();
    
    public async Task SlashCommand(SocketSlashCommand slashCommand)
    {
        Command? command = _commands.FirstOrDefault(x => x.Name == slashCommand.CommandName);
        if (command == null || command.Handler == null)
            await slashCommand.RespondAsync(Translation.UnknownCommand);
        else
            await command.Handler(slashCommand, serviceProvider);
    }

    public async Task Ready()
    {
        DiscordSocketClient discordSocketClient = serviceProvider.GetRequiredService<DiscordSocketClient>();
        await discordSocketClient.SetCustomStatusAsync(Translation.Status);
        IReadOnlyCollection<SocketApplicationCommand> existingCommands = await discordSocketClient.GetGlobalApplicationCommandsAsync();
        foreach (Command command in _commands)
        {
            SocketApplicationCommand? existingCommand = existingCommands.FirstOrDefault(x => x.Name == command.Name);
            if (existingCommand != null)
            {
                if (CheckIfExists(existingCommand, command))
                    continue;
                await existingCommand.DeleteAsync();
            }
            SlashCommandBuilder commandBuilder = new();
            commandBuilder.WithName(command.Name);
            commandBuilder.WithDescription(command.Description);
            if (command.Parameters.Count == 0)
                foreach (Parameter parameter in command.Parameters)
                    commandBuilder.AddOption(parameter.Name, parameter.Type, parameter.Description, parameter.Required);
            await discordSocketClient.CreateGlobalApplicationCommandAsync(commandBuilder.Build());
        }
    }

    private bool CheckIfExists(SocketApplicationCommand socketApplicationCommand, Command command)
    {
        if (socketApplicationCommand.Description != command.Description)
            return false;

        if (command.Parameters.Count == 0 && socketApplicationCommand.Options.Count == 0)
            return true;
        
        foreach (SocketApplicationCommandOption option in socketApplicationCommand.Options)
        foreach (Parameter parameter in command.Parameters)
        {
            if (option.Name != parameter.Name)
                return false;
            if (option.Description != parameter.Description)
                return false;
            if (option.IsRequired != parameter.Required)
                return false;
            if (option.Type != parameter.Type)
                return false;
        }
        
        return true;
    }
}