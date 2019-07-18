using System;
using System.Runtime.CompilerServices;
using System.Timers;
using Serilog;

namespace PickleJar.ServiceHost
{
    public class AppRootService
    {
        readonly Timer timer;

        public AppRootService()
        {
            timer = new Timer(1000);
            timer.AutoReset = true;
            timer.Elapsed += (sender, eventArgs) => Elapsed();
        }



        public void Start()
        {
            LogMethod();

            timer.Start();
        }

        public void Stop()
        {
            LogMethod();

            timer.Stop();
        }



        public void Pause()
        {
            LogMethod();

            timer.Stop();
        }

        public void Continue()
        {
            LogMethod();

            timer.Start();
        }



        public void Shutdown()
        {
            LogMethod();

            timer.Stop();
        }



        private void Elapsed()
        {
            LogMethod();
        }

        private void LogMethod([CallerMemberName] string methodName = null)
        {
            Log.Information("AppRootService => {MethodName}", methodName);
        }
    }
}
