using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace NetworkObjects.Debug;

public static class LogInfo
{
    public enum LogState
    {
        Error,
        Warning,
        Log,
        Info
    }

    public static string FileName = "Log";
    private static bool locked;
    private static readonly string sep = "*";
    private static readonly string rep = ",";
    private static readonly Dictionary<string, string> MetaData = new();
    private static string metaData = "";
    private static readonly IWriteLoadFile writeLoadFile = new WriteLoadFile();
    public static bool LogOn { get; set; }
    public static bool SaveToFile { get; set; }

    public static void UpdateMeta(string key, string value)
    {
        MetaData[key] = value;
        if (!LogOn) return;
        while (locked) Console.WriteLine("cant update meta yet");

        locked = true;
        var meta = new string[MetaData.Count];
        var i = 0;
        foreach (var md in MetaData)
        {
            meta[i] = $"{md.Key} = {md.Value}";
            i++;
        }

        metaData = $"{FormatCsvText($"[{string.Join(", ", meta)}]")}";
        locked = false;
    }


    public static event Action<LogData>? OnLogUpdate;

    /// <summary>
    ///     Logs a warning message.
    /// </summary>
    /// <param name="log">The warning message to log.</param>
    /// <param name="memberName">The name of the calling method (automatically added by the compiler).</param>
    /// <param name="filePath">The file path of the calling method (automatically added by the compiler).</param>
    public static void LogInformation(string log, [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "")
    {
        if (!LogOn) return;
        var stackTrace = new StackTrace(true);
        var data = new LogData
        {
            LoggedTime = DateTime.Now,
            LogMessage = log,
            State = LogState.Info,
            ClassName = Path.GetFileNameWithoutExtension(filePath),
            MethodName = memberName,
            Path = filePath,
            BackTrace = stackTrace.ToString()
        };
        OnLogUpdate?.Invoke(data);
        if (SaveToFile) SaveData(data);
    }

    /// <summary>
    ///     Logs a message.
    /// </summary>
    /// <param name="log">The message to be logged.</param>
    /// <param name="memberName">The name of the calling member (automatically filled by the compiler).</param>
    /// <param name="filePath">The path of the file that contains the calling member (automatically filled by the compiler).</param>
    public static void Log(string log, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "")
    {
        if (!LogOn) return;
        var data = new LogData
        {
            LoggedTime = DateTime.Now,
            LogMessage = log,
            State = LogState.Log,
            ClassName = Path.GetFileNameWithoutExtension(filePath),
            MethodName = memberName,
            Path = filePath,
            BackTrace = "Log State Does Not Include BackTrace"
        };
        OnLogUpdate?.Invoke(data);
        if (SaveToFile) SaveData(data);
    }


    /// <summary>
    ///     Logs a warning message.
    /// </summary>
    /// <param name="log">The warning message to log.</param>
    /// <param name="memberName">The name of the calling method (automatically added by the compiler).</param>
    /// <param name="filePath">The file path of the calling method (automatically added by the compiler).</param>
    public static void LogWarning(string log, [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "")
    {
        if (!LogOn) return;
        var stackTrace = new StackTrace(true);
        var data = new LogData
        {
            LoggedTime = DateTime.Now,
            LogMessage = log,
            State = LogState.Warning,
            ClassName = Path.GetFileNameWithoutExtension(filePath),
            MethodName = memberName,
            Path = filePath,
            BackTrace = stackTrace.ToString()
        };
        OnLogUpdate?.Invoke(data);
        if (SaveToFile) SaveData(data);
    }

    /// <summary>
    ///     Logs an error message along with the name of the calling member and file.
    /// </summary>
    /// <param name="log">The error message to be logged.</param>
    /// <param name="memberName">The name of the calling member (automatically set by the compiler).</param>
    /// <param name="filePath">The file path of the calling file (automatically set by the compiler).</param>
    public static void LogError(string log, [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "")
    {
        if (!LogOn) return;
        var stackTrace = new StackTrace(true);
        var data = new LogData
        {
            LoggedTime = DateTime.Now,
            LogMessage = log,
            State = LogState.Error,
            ClassName = Path.GetFileNameWithoutExtension(filePath),
            MethodName = memberName,
            Path = filePath,
            BackTrace = stackTrace.ToString()
        };
        OnLogUpdate?.Invoke(data);
        if (SaveToFile) SaveData(data);
    }

    private static void SaveData(LogData data)
    {
        if (!SaveToFile || !LogOn) return;
        while (writeLoadFile.Locked || locked)
        {
            Console.WriteLine("cant save locked file");
            if (!SaveToFile || !LogOn) return;
        }

        locked = true;
        var path = $"Log/LogInfoData_{DateTime.Now.ToString("yyyy_MM_dd")}/{FileName}.csv";

        if (!File.Exists(path))
        {
            writeLoadFile.SaveToFile($"sep={sep}", path);
            writeLoadFile.AppendToFile($"Time{sep} " +
                                       $"ClassName{sep} " +
                                       $"Method Name{sep} " +
                                       $"Path{sep} " +
                                       $"Stat{sep} " +
                                       $"Back Trace{sep} " +
                                       $"Message{sep} " +
                                       $"Meta" +
                                       $"\n"
                , path);
        }


        writeLoadFile.AppendToFile($"{data.LoggedTime:dd/MM/yyyy hh:mm:ss.fff tt}{sep} " +
                                   $"{data.ClassName}{sep} " +
                                   $"{data.MethodName}{sep} " +
                                   $"{data.Path}{sep} " +
                                   $"{data.State}{sep} " +
                                   $"{FormatCsvText(data.BackTrace ?? "")}{sep} " +
                                   $"{FormatCsvText(data.LogMessage)}{sep} " +
                                   metaData +
                                   "\n", path);
        locked = false;
    }

    private static string FormatCsvText(string text)
    {
        text = Regex.Replace(text, @"\t|\n|\r", " | ");
        text = text.Replace("\"", "`");
        text = text.Replace("\'", "`");
        return $"'{text.Replace(sep, rep)}'";
    }

    public struct LogData
    {
        public DateTime LoggedTime;
        public string LogMessage { get; set; }
        public LogState State { get; set; }
        public string ClassName { get; set; }
        public string MethodName { get; set; }
        public string Path { get; set; }
        public string? BackTrace { get; set; }
    }
}