using DiscordMusicBot.Bot.Utility;

namespace DiscordMusicBot.Bot;

public static class Translation
{
    // Status
    public const string Status = "Переключаю треки 🎧";
    
    // Commands
    public const string LoopCommandDescription = "Переключить зацикливание текущего трека";
    public const string PingCommandDescription = "Проверить время ответа бота";
    public const string PlayCommandDescription = "Включить трек с YouTube";
    public const string SkipCommandDescription = "Пропустить трек";
    public const string StatusCommandDescription = "Получить статус https://cobalt.tools API";
    public const string StopCommandDescription = "Остановить музыку и ливнуть";
    
    // Responses
    public const string ApiOffline = "Не могу подключиться к API ⛔";
    public const string ApiOnline = "API в сети ✅";
    public const string Disconnecting = "Отключаюсь ⛔";
    public static readonly Func<bool, string> Looping = looped => $"{(looped ? "Включаю" : "Выключаю")} зацикливание 🔂";
    public const string NotConnected = "Не подключён к войсу ⛔";
    public const string Pong = "Понг! 🏓";
    public static readonly Func<Song, string> Skipping = song => $"Пропускаю трек {Track(song)} ⏩";
    public const string Searching = "Ищу 🔍";
    public static Func<Song, string> Track => song => $"[{song.Artist} - {song.Title}]({song.YoutubeUrl})";
    public const string TrackNotFound = "Не могу найти трек ❌";
    public const string UnknownCommand = "Неизвестная команда ❌";
    public const string UserNotInVoice = "Ты не в войсе ⛔";
    
    // Parameters
    public const string TrackParamName = "трек";
    public const string TrackParamDescription = "Ссылка на трек или поисковый запрос";
}