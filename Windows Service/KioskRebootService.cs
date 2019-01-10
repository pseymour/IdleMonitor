namespace KioskRebootService
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.ServiceProcess;

    /// <summary>
    /// This class is the main class for the kiosk reboot service.
    /// </summary>
    public partial class KioskRebootService : ServiceBase
    {       
        /// <summary>
        /// The date and time at which the service first noticed that no one was logged on to the computer.
        /// </summary>
        private DateTime firstNoticedNobodyLoggedOn;
        
        /// <summary>
        /// A timer to periodically check the computer's status.
        /// </summary>
        private System.Timers.Timer statusTimer;

        /// <summary>
        /// Initializes a new instance of the KioskRebootService class.
        /// </summary>
        public KioskRebootService()
        {
            this.InitializeComponent();

            this.WriteEventLogEntry("Initializing Kiosk Reboot Service.", EventLogEntryType.Information);

            this.firstNoticedNobodyLoggedOn = DateTime.MaxValue;

            // Initialize the status timer.
            this.statusTimer = new System.Timers.Timer(15 * 1000);
            this.statusTimer.Elapsed += new System.Timers.ElapsedEventHandler(this.TimerElapsed);
            this.statusTimer.Enabled = false;

            this.WriteEventLogEntry("Kiosk Reboot Service initialized.", EventLogEntryType.Information);
        }

        /// <summary>
        /// This function executes when a Start command is sent to the service by the Service Control Manager (SCM)
        /// or when the operating system starts (for a service that starts automatically).
        /// </summary>
        /// <param name="args">
        /// Data passed by the start command.
        /// </param>
        protected override void OnStart(string[] args)
        {
            base.OnStart(args);

            this.SetProcessPriority();

            this.WriteEventLogEntry("The service is starting.", EventLogEntryType.Information);

            this.statusTimer.Start();
        }

        /// <summary>
        /// Sets the process priority for the service.
        /// </summary>
        /// <remarks>
        /// The service's priority is set to below normal.
        /// </remarks>
        private void SetProcessPriority()
        {
            try
            {
                System.Diagnostics.Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.BelowNormal;
            }
            catch (Win32Exception)
            {
                this.WriteEventLogEntry("Win32 Exception while setting process priority.", EventLogEntryType.Warning);
            }
            catch (PlatformNotSupportedException)
            {
                this.WriteEventLogEntry("Platform Not Supported Exception while setting process priority.", EventLogEntryType.Warning);
            }
            catch (SystemException)
            {
                this.WriteEventLogEntry("System Exception while setting process priority.", EventLogEntryType.Warning);
            }
        }
        
        /// <summary>
        /// Performs a system shutdown if no users are logged on to the computer.
        /// </summary>
        private void PerformShutdown()
        {
            if (this.GetLoggedOnUserCount() > 0)
            { // At least one user is logged on to the computer.
                // Reset the "nobody is logged on" time to prevent shutdowns.
                this.firstNoticedNobodyLoggedOn = DateTime.MaxValue;
            }
            else
            { // No users are logged on to the computer.

#if DEBUG
                this.WriteEventLogEntry(string.Format("No User Logon Threshold = {0:N0}", Settings.NoUserLogonThreshold), EventLogEntryType.Information);
#endif

                if (DateTime.MaxValue == this.firstNoticedNobodyLoggedOn)
                { // This is the first time we've noticed that no one is logged on to the computer.
                    this.firstNoticedNobodyLoggedOn = DateTime.Now;
                    this.WriteEventLogEntry("No one is logged on to this computer.", EventLogEntryType.Information);
                }
                else
                { // This is not the first time we've noticed that no one is logged on to the computer.
                    double lastLogonInMinutes = DateTime.Now.Subtract(this.firstNoticedNobodyLoggedOn).TotalMinutes;
#if DEBUG
                    this.WriteEventLogEntry(string.Format("last logon minutes = {0:N2}", lastLogonInMinutes), EventLogEntryType.Information);
#endif
                    if (lastLogonInMinutes >= Settings.NoUserLogonThreshold)
                    {
                        this.WriteEventLogEntry(string.Format("No users have been logged on to this computer for {0:N2} minutes. Rebooting.", lastLogonInMinutes), EventLogEntryType.Information);
                        Interop.Win32.NativeMethods.PerformShutdownAction(Interop.Win32.NativeMethods.ShutdownFlag.Reboot, false);
                    }
                }
            }
        }

        /// <summary>
        /// Writes an entry in the event log.
        /// </summary>
        /// <param name="text">
        /// The text to be contained in the event log message.
        /// </param>
        /// <param name="entryType">
        /// The type of event log entry to be created.
        /// </param>
        private void WriteEventLogEntry(string text, EventLogEntryType entryType)
        {
            try
            {
                this.EventLog.WriteEntry(text, entryType);
            }
            catch (System.Exception)
            {
            }
        }

        /// <summary>
        /// Gets the number of users that are currently logged on to the computer.
        /// </summary>
        /// <returns>
        /// Returns an integer that contains the number of users that are currently
        /// logged on to the computer.
        /// </returns>
        private int GetLoggedOnUserCount()
        {
            int returnValue = 0;           

            string[] userNames = LsaLogonSessions.LogonSessions.GetLoggedOnUserNames();
            if (userNames != null)
            {
                returnValue = userNames.Length;
            }

            return returnValue;
        }

        /// <summary>
        /// Handles the Elapsed event for the status timer.
        /// </summary>
        /// <param name="sender">
        /// The timer whose interval has elapsed.
        /// </param>
        /// <param name="e">
        /// Data specific to the event.
        /// </param>
        private void TimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.PerformShutdown();
        }
    }
}
