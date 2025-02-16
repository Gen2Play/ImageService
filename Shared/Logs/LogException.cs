using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Logs;

public static class LogException
{
    public static void LogExceptions(Exception exception, string message)
    {
        LogToFile(exception, message, null);
        LogToConsole(exception, message, null);
        LogToDebbuger(exception, message, null);
        Console.WriteLine(message);
    }

    private static void LogToDebbuger(Exception exception, string message, object value)
    {
        Log.Debug(message);
    }

    private static void LogToConsole(Exception exception, string message, object value)
    {
        Log.Warning(message, value);
    }

    private static void LogToFile(Exception exception, string message, object value)
    {
        Log.Error(exception, message, value);
    }
}
