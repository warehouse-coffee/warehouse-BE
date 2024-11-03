
using warehouse_BE.Application.Common.Interfaces;

namespace warehouse_BE.Infrastructure.Service;

public class LoggerService : ILoggerService
{
    private readonly string _logDirectory;

    public LoggerService()
    {
        _logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");

        if (_logDirectory == null)
        {
            throw new ArgumentNullException(nameof(_logDirectory), "Log directory path cannot be null");
        }

        if (!Directory.Exists(_logDirectory))
        {
            Directory.CreateDirectory(_logDirectory);
        }
    }

    private string GetLogFilePath(DateTime? date)
    {
        return Path.Combine(_logDirectory, $"{date:yyyy_MM_dd}.txt");
    }

    public void LogInformation(string message)
    {
        WriteLog("INFO", message);
    }

    public void LogWarning(string message)
    {
        WriteLog("WARNING", message);
    }

    public void LogError(string message, Exception ex)
    {
        WriteLog("ERROR", $"{message}. Exception: {ex.Message}");
    }

    private void WriteLog(string logLevel, string message)
    {
        string logFilePath = GetLogFilePath(DateTime.Now);

        using (StreamWriter writer = new StreamWriter(logFilePath, true))
        {
            writer.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{logLevel}] - {message}");
        }
    }

    public List<string> ReadLogs(DateTime? date, string? logLevel = null, int? hour = null)
    {
        List<string> logs = new List<string>();

        if (date == default && logLevel == null && hour == null)
        {
            var currentDate = DateTime.Now;
            int currentMonth = currentDate.Month;
            int currentYear = currentDate.Year;

            for (int day = 1; day <= DateTime.DaysInMonth(currentYear, currentMonth); day++)
            {
                DateTime logDate = new DateTime(currentYear, currentMonth, day);
                string logFilePath = GetLogFilePath(logDate);

                if (File.Exists(logFilePath))
                {
                    logs.AddRange(File.ReadLines(logFilePath));
                }
            }
        }
        else
        {
            string logFilePath = GetLogFilePath(date);

            if (!File.Exists(logFilePath))
            {
                return new List<string>();
            }

            logs = File.ReadLines(logFilePath)
                .Where(line =>
                {
                    bool levelMatches = string.IsNullOrEmpty(logLevel) || line.Contains($"[{logLevel}]");

                    bool hourMatches = true;
                    if (hour.HasValue)
                    {
                        var logTime = DateTime.Parse(line.Substring(0, 19));
                        hourMatches = logTime.Hour == hour.Value;
                    }

                    return levelMatches && hourMatches;
                })
                .ToList();
        }

        return logs;
    }

}
