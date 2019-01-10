namespace KioskRebootService
{
    using System;
    using System.ServiceProcess;

    /// <summary>
    /// Contains the main entry point for the application.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static void Main()
        {
            ServiceBase[] servicesToRun = new ServiceBase[] { new KioskRebootService() };
            ServiceBase.Run(servicesToRun);
        }
    }
}
