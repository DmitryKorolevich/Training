using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.ServiceProcess;

namespace VitalChoice.ExportService {
    [RunInstaller (true)]
    public class WindowsServiceInstaller: Installer {
        /// <summary>
        /// Public Constructor for WindowsServiceInstaller.
        /// - Put all of your Initialization code here.
        /// </summary>
        public WindowsServiceInstaller ()
        {
            var serviceProcessInstaller = new ServiceProcessInstaller();
            var serviceInstaller = new ServiceInstaller();

            //# Service Account Information

            serviceProcessInstaller.Account = ServiceAccount.NetworkService;
            serviceProcessInstaller.Username = null;
            serviceProcessInstaller.Password = null;

            //# Service Information

            serviceInstaller.DisplayName = "Vital Choice Export Service";
            serviceInstaller.Description = "VeraCore Integrations";
            serviceInstaller.StartType = ServiceStartMode.Automatic;

            //# This must be identical to the WindowsService.ServiceBase name

            //# set in the constructor of WindowsService.cs

            serviceInstaller.ServiceName = "exportservice";
            Installers.Add(serviceProcessInstaller);
            Installers.Add(serviceInstaller);
        }

        public override void Install (IDictionary stateSaver)
        {
            if (!EventLog.SourceExists("ExportService"))
            {
                EventLog.CreateEventSource("ExportService", "Application");
            }
            base.Install(stateSaver);
        }
    }
}