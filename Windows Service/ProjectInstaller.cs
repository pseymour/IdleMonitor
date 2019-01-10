namespace KioskRebootService
{
    using System;
    using System.ComponentModel;
    using System.Configuration.Install;

    /// <summary>
    /// This class provides a custom installation for the kiosk reboot service.
    /// </summary>
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Installs an executable containing classes that extend ServiceBase. This
        /// class is called by installation utilities, such as InstallUtil.exe, when
        /// installing a service application.
        /// </summary>
        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller;

        /// <summary>
        /// Installs a class that extends ServiceBase to implement a service. This
        /// class is called by the install utility when installing a service application.
        /// </summary>
        private System.ServiceProcess.ServiceInstaller serviceInstaller;

        /// <summary>
        /// Initializes a new instance of the ProjectInstaller class.
        /// </summary>
        public ProjectInstaller()
        {
            this.InitializeComponent();
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">
        /// True if managed resources should be disposed; otherwise, false.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// This event fires before the installers perform their uninstall operations.
        /// </summary>
        /// <param name="sender">
        /// The service installer that is performing the installation.
        /// </param>
        /// <param name="e">
        /// Data specific to this event.
        /// </param>
        private void BeforeServiceUninstall(object sender, InstallEventArgs e)
        {
            try
            {
                System.ServiceProcess.ServiceController controller = new System.ServiceProcess.ServiceController(this.serviceInstaller.ServiceName);
                controller.Stop();
            }            
            catch (System.ComponentModel.Win32Exception)
            { // An error occurred when accessing a system API.
            }
            catch (InvalidOperationException)
            { // The service cannot be stopped.
            }
        }

        /// <summary>
        /// This event fires after the Install methods of all the installers in the
        /// Installers property have run.
        /// </summary>
        /// <param name="sender">
        /// The service installer that is performing the installation.
        /// </param>
        /// <param name="e">
        /// Data specific to this event.
        /// </param>
        private void AfterServiceInstall(object sender, InstallEventArgs e)
        {
            // Create a ServiceInstaller object from the sender object.
            System.ServiceProcess.ServiceInstaller installer = sender as System.ServiceProcess.ServiceInstaller;

            // If the ServiceInstaller object was created successfully, attempt to
            // start the service.
            if (installer != null)
            {
                // Attempt to start the service.
                try
                {
                    System.ServiceProcess.ServiceController controller = new System.ServiceProcess.ServiceController(installer.ServiceName);
                    controller.Start();
                }                
                catch (System.ComponentModel.Win32Exception)
                { // An error occurred when accessing a system API.
                }
                catch (InvalidOperationException)
                { // The service cannot be started.
                }
            }
        }
    }
}