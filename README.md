# Discord music bot

Бот для воспроизведения музыки через Discord.

## Фичи:
### 1. https://cobalt.tools
Использует cobalt.tools и стриминг, чтобы ускорить получение музыки с YouTube. Это также позволяет не попадать на rate-limit.
### 2. Command API
Это уже будет полезно разработчикам: достаточно создать команду внутри ```Bot/Commands```:
```csharp
public class MyCommand : Command
{
    public MyCommand()
    {
        Name = "mycommand";
        Description = "My description";
        Handler = Handle;
    }
    
    public static async Task Handle(SocketSlashCommand command)
    {
        await command.RespondAsync("test");
    }    
}
```
### 3. Docker
Скопируйте ```compose.yml``` на сервер, измените ```TOKEN``` и вы готовы к запуску!
```shell
docker compose up -d
```