namespace MoqToNSubstitute.Utilities;

public static class Logger
{
    private static readonly string LogPath = Path.Combine(Directory.GetCurrentDirectory(), "logs", $"log_{DateTime.Now:yyyyMMddHHmmss}.txt");

    static Logger()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(LogPath));
    }

    public static void Log(string message)
    {
        Console.WriteLine(message);
        File.AppendAllText(LogPath, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}\n");
    }
}