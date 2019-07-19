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
        const string EventLogSource = "PickleJar-ServiceHost";
        const string EventLogName = "PickleJar";

        public static IDisposable BeginScope()
        {
            var logSwitch = new LoggingLevelSwitch();
            logSwitch.MinimumLevel = LogEventLevel.Debug;

            var logConfig = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.ControlledBy(logSwitch);
                
            // Outputs
            logConfig.WriteTo.Console(standardErrorFromLevel: LogEventLevel.Error, levelSwitch: logSwitch, theme: AnsiConsoleTheme.Literate); 
            logConfig.WriteTo.Debug(LogEventLevel.Debug);

            if (EventLog.SourceExists(EventLogSource) && EventLog.Exists(EventLogName))
            {
                logConfig.WriteTo.EventLog(EventLogSource, EventLogName);
            }

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

            if (!EventLog.SourceExists(EventLogSource))
            {
                EventLog.CreateEventSource(EventLogSource, EventLogName);

                Log.Information("Logging => Created EventSource: '{EventLogSource}'", EventLogSource);
            }
        }

        public static void BeforeUninstall()
        {
            Log.Information("Logging => BeforeUninstall");

            if (EventLog.SourceExists(EventLogSource))
            {
                EventLog.DeleteEventSource(EventLogSource);

                Log.Information("Logging => Deleted EventSource '{EventLogSource}'", EventLogSource);
            }
        }
    }
}
