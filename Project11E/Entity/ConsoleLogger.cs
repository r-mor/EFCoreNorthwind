﻿using Microsoft.Extensions.Logging;
using static System.Console;

namespace Project11E.Entity;

public class ConsoleLoggerProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName)
    {
        return new ConsoleLogger();
    }

    public void Dispose() { }
}

public class ConsoleLogger : ILogger
{
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        switch (logLevel)
        {
            case LogLevel.Trace:
            case LogLevel.Information:
            case LogLevel.None:
                return false;
            case LogLevel.Debug:
            case LogLevel.Warning:
            case LogLevel.Error:
            case LogLevel.Critical:
            default:
                return true;
        };
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (eventId.Id == 20100)
        {

            Write($"Level: {logLevel}, EventId: {eventId}");

            if (state is not null)
            {
                Write($", State: {state}");
            }

            if (exception is not null)
            {
                Write($", Exception: {exception.Message}");
            }

            WriteLine();
        }
    }
}
