using System;
using Serilog;
using Topshelf;
using Topshelf.HostConfigurators;
using Topshelf.ServiceConfigurators;

namespace PickleJar.ServiceHost
{
    internal class SetupHosting
    {
        internal void RunService()
        {
            var exitCode = HostFactory.Run(Configure);

            Environment.ExitCode = (int)exitCode;
        }

        private void Configure(HostConfigurator hostOptions)
        {
            hostOptions.SetDescription("PickleJar.ServiceHost.Description");
            hostOptions.SetDisplayName("PickleJar.ServiceHost.DisplayName");
            hostOptions.SetServiceName("PickleJar.ServiceHost.ServiceName");

            hostOptions.Service<AppRootService>(ConfigureService);

            hostOptions.EnablePauseAndContinue();
            hostOptions.EnableShutdown();

            hostOptions.DependsOnEventLog();
            hostOptions.RunAsNetworkService();
            hostOptions.StartAutomaticallyDelayed();

            hostOptions.UseSerilog();

            hostOptions.OnException(SetupLogging.LogException);
            hostOptions.AfterInstall(SetupLogging.AfterInstall);
            hostOptions.BeforeUninstall(SetupLogging.BeforeUninstall);
        }

        private void ConfigureService(ServiceConfigurator<AppRootService> serviceOptions)
        {
            serviceOptions.ConstructUsing(name => new AppRootService());

            serviceOptions.WhenStarted(tc => tc.Start());
            serviceOptions.WhenStopped(tc => tc.Stop());

            serviceOptions.WhenPaused(tc => tc.Pause());
            serviceOptions.WhenContinued(tc => tc.Continue());

            serviceOptions.WhenShutdown(tc => tc.Shutdown());
        }
    }
}
