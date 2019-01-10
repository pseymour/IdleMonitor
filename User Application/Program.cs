namespace UserIdleMonitor
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// This class contains the main entry point for the application.
    /// </summary>
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string mutexName = "{7321D23E-90F9-43F8-8EFD-6423AF969B06}";

            bool grantedOwnership = false;
            System.Threading.Mutex singleInstanceMutex = new System.Threading.Mutex(true, mutexName, out grantedOwnership);
            try
            {
                if (!grantedOwnership)
                {
                    return;
                }
                else
                {
                    Application.Run(new AppContext());
                }
            }
            finally
            {
                singleInstanceMutex.Close();
            }
        }
    }
}
