using MelonLoader;

namespace labyrinthine_library.Modules;

public enum LogType
{
    Error,
    Info,
    Warning
}

public class Logs
{
    private static readonly string FileName = "Labyrinthine_Library.txt";
    private static readonly string LogsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");

    public static void Warning(string message)
    {
        MelonLogger.Warning(message);
        WriteToFile(LogType.Warning, message);
    }

    public static void Error(string message)
    {
        MelonLogger.Error(message);
        WriteToFile(LogType.Error, message);
    }

    public static void Info(string message)
    {
        MelonLogger.Msg(message);
        WriteToFile(LogType.Info, message);
    }

    private static void WriteToFile(LogType type, string data)
    {
        try
        {
            Directory.CreateDirectory(LogsDirectory);

            string path = Path.Combine(LogsDirectory, FileName);

            File.AppendAllText(
                path,
                $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {type}:{data}{Environment.NewLine}"
            );
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"Failed to write log file '{FileName}': {ex}");
        }
    }
}