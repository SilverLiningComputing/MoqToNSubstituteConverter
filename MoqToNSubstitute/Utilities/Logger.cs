namespace MoqToNSubstitute.Utilities;

/// <summary>
/// A console and file logger
/// </summary>
public static class Logger
{
    private static readonly string LogPath = Path.Combine(Directory.GetCurrentDirectory(), "logs", $"log_{DateTime.Now:yyyyMMddHHmmss}.txt");

    /// <summary>
    /// Constructor for the logger that will create the logs folder
    /// </summary>
    /// <exception cref="InvalidOperationException">Throws an exception if creating the folder fails</exception>
    static Logger()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(LogPath) ?? throw new InvalidOperationException());
        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occurred trying to create the log folder: {e}");
        }
    }

    /// <summary>
    /// Static method to log to the console and the log file 
    /// </summary>
    /// <param name="message">The log message</param>
    public static void Log(string message)
    {
        Console.WriteLine(message);
        File.AppendAllText(LogPath, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}\n");
    }
}