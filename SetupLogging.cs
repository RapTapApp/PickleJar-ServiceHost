using System;
using System.Diagnostics;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace PickleJar.ServiceHost
{
    public class SetupLogging : IDisposable
    {
        const string EventLogSourceName = "PickleJar-ServiceHost-EventLog";

        public static IDisposable BeginScope()
        {
            var logSwitch = new LoggingLevelSwitch();
            logSwitch.MinimumLevel = LogEventLevel.Debug;

            var logConfig = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.ControlledBy(logSwitch);
                
            // Outputs
            logConfig.WriteTo.Console(standardErrorFromLevel: LogEventLevel.Error, levelSwitch: logSwitch, theme: AnsiConsoleTheme.Code); // outputTemplate: ConsoleTemplate, 
            logConfig.WriteTo.Debug(LogEventLevel.Debug);
            logConfig.WriteTo.EventLog(EventLogSourceName);

            Log.Logger = logConfig.CreateLogger();

            return new SetupLogging();
        }

        public void Dispose()
        {
            Log.CloseAndFlush();
        }



        public static void LogException(Exception exception)
        {
            Log.Fatal(exception, "Logging => Fatal service failure");
        }

        public static void AfterInstall()
        {
            Log.Information("Logging => AfterInstall");

            if (!EventLog.SourceExists(EventLogSourceName))
            {
                EventLog.CreateEventSource(EventLogSourceName, EventLogSourceName);

                Log.Information("Logging => Created EventLog '{EventLogSourceName}'", EventLogSourceName);
            }
        }

        public static void BeforeUninstall()
        {
            Log.Information("Logging => BeforeUninstall");

            if (EventLog.SourceExists(EventLogSourceName))
            {
                EventLog.DeleteEventSource(EventLogSourceName);

                Log.Information("Logging => Deleted EventLog '{EventLogSourceName}'", EventLogSourceName);
            }
        }
    }
}
