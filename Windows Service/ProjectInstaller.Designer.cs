namespace KioskRebootService
{
    using System;
    using System.Configuration.Install;

    /// <summary>
    /// This class provides a custom installation for the kiosk reboot service.
    /// </summary>
    public partial class ProjectInstaller
    {
        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            this.serviceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.serviceInstaller = new System.ServiceProcess.ServiceInstaller();

            // 
            // serviceProcessInstaller
            // 
            this.serviceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceProcessInstaller.Password = null;
            this.serviceProcessInstaller.Username = null;

            // 
            // serviceInstaller
            // 
            this.serviceInstaller.ServiceName = "KioskRebootService";
            this.serviceInstaller.Description = "Reboots kiosk PCs if no one is logged on for a set amount of time.";
            this.serviceInstaller.DisplayName = "Kiosk Reboot Service";
            this.serviceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            this.serviceInstaller.AfterInstall += new InstallEventHandler(AfterServiceInstall);
            this.serviceInstaller.BeforeUninstall += new InstallEventHandler(BeforeServiceUninstall);

            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] { this.serviceProcessInstaller, this.serviceInstaller });
        }

        #endregion
    }
}