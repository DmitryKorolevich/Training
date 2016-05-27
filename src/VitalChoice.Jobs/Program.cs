using System.ServiceProcess;

namespace VitalChoice.Jobs
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var servicesToRun = new ServiceBase[]
            {
                new JobWindowsService()
            };
            ServiceBase.Run(servicesToRun);
        }
    }
}