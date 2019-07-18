namespace PickleJar.ServiceHost
{
    public class Program
    {
        public static void Main()
        {
            using(SetupLogging.BeginScope())
            {
                new SetupHosting().RunService();
            }
        }
    }
}