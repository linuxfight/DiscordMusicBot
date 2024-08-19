using Discord.WebSocket;
using SlashCommandBuilder = Discord.SlashCommandBuilder;

namespace DiscordMusicBot.Bot;

public class Handler(DiscordSocketClient client, List<Command> commands)
{
    public async Task SlashCommand(SocketSlashCommand slashCommand)
    {
        Command? command = commands.FirstOrDefault(x => x.Name == slashCommand.CommandName);
        if (command == null)
            await slashCommand.RespondAsync("unknown command");
        else
            await command.Handler(slashCommand);
    }

    public async Task Ready()
    {
        IReadOnlyCollection<SocketApplicationCommand> existingCommands = await client.GetGlobalApplicationCommandsAsync();
        foreach (SocketApplicationCommand existingCommand in existingCommands)
            await existingCommand.DeleteAsync();
        foreach (Command command in commands)
        {
            SlashCommandBuilder commandBuilder = new();
            commandBuilder.WithName(command.Name);
            commandBuilder.WithDescription(command.Description);
            await client.CreateGlobalApplicationCommandAsync(commandBuilder.Build());
        }
    }
}