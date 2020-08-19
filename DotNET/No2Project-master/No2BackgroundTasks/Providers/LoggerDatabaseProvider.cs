using Microsoft.Extensions.Logging;
using No2API.Entities.Models;
using No2BackgroundTasks.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace No2BackgroundTasks.Providers
{

    public class LoggerDatabaseProvider : ILoggerProvider
    {
        private APIContext context;

        public LoggerDatabaseProvider(APIContext context)
        {
            this.context = context;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new Logger(categoryName, context);
        }

        public void Dispose()
        {
        }

        public class Logger : ILogger
        {
            private readonly string _categoryName;
            private readonly APIContext context;

            public Logger(string categoryName, APIContext context)
            {
                this.context = context;
                _categoryName = categoryName;
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return true;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                if (logLevel == LogLevel.Critical || logLevel == LogLevel.Error || logLevel == LogLevel.Warning)
                    RecordMsg(logLevel, eventId, state, exception, formatter);
            }

            private void RecordMsg<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                //context.Logs.Add(new Log
                //{
                //    LogLevel = logLevel.ToString(),
                //    CategoryName = _categoryName,
                //    Msg = formatter(state, exception),
                //    User = "username",
                //    Timestamp = DateTime.Now
                //});
                //context.SaveChanges();
            }

            public IDisposable BeginScope<TState>(TState state)
            {
                return new NoopDisposable();
            }

            private class NoopDisposable : IDisposable
            {
                public void Dispose()
                {
                }
            }
        }
    }
}
