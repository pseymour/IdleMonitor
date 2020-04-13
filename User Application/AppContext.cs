namespace UserIdleMonitor
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// This class specifies contextual information for the application.
    /// </summary>
    internal class AppContext : ApplicationContext
    {
        /// <summary>
        /// A timer that monitors keyboard and mouse input from the user.
        /// </summary>
        private System.Timers.Timer inputUpdateTimer;

        /// <summary>
        /// The icon in the notification area (what some people call the system tray).
        /// </summary>
        private NotifyIcon notificationIcon;

        /// <summary>
        /// The date and time at which the last user notification was shown.
        /// </summary>
        private DateTime lastBalloonTipShown = DateTime.MinValue;

        /// <summary>
        /// Constructor.
        /// </summary>
        public AppContext()
        {
            this.lastBalloonTipShown = DateTime.MinValue;

            notificationIcon = new NotifyIcon
            {
                Icon = UserIdleMonitor.Properties.Resources.hourglass,
                Text = "Idle Monitor"
            };

#if DEBUG
            // In debug mode, show the values for the various settings when the user hovers over
            // the notification icon.
            System.Text.StringBuilder iconText = new System.Text.StringBuilder("Idle Monitor");
            iconText.Append(System.Environment.NewLine);
            iconText.Append(string.Format("Timeout: {0}", Settings.IdleTimeoutMinutes));
            iconText.Append(System.Environment.NewLine);
            iconText.Append(string.Format("Warning: {0}", Settings.WarningMinutes));
            iconText.Append(System.Environment.NewLine);
            iconText.Append(string.Format("Repeat: {0}", Settings.WarningRepeatMinutes));
            notificationIcon.Text = iconText.ToString();
#endif

            notificationIcon.Visible = true;

            ThreadExit += AppContext_ThreadExit;

            inputUpdateTimer = new System.Timers.Timer(10000);
            inputUpdateTimer.Elapsed += new System.Timers.ElapsedEventHandler(InputUpdateTimerElapsed);

            inputUpdateTimer.Start();
        }


        /// <summary>
        /// Handles the ThreadExit event for the application context.
        /// </summary>
        /// <param name="sender">
        /// The application context whose thread is closing.
        /// </param>
        /// <param name="e">
        /// Data specific to the event.
        /// </param>
        private void AppContext_ThreadExit(object sender, EventArgs e)
        {
            notificationIcon.Visible = false;
            notificationIcon.Dispose();

            inputUpdateTimer.Stop();
            inputUpdateTimer = null;
        }


        /// <summary>
        /// Checks how long the current user has been idle and takes the appropriate action, if any.
        /// </summary>
        private void CheckIdleTime()
        {
            if (inputUpdateTimer.Enabled)
            {
                int minutesUntilLogoff = Settings.IdleTimeoutMinutes - (int)(NativeMethods.SecondsSinceLastInput / 60);
                if (minutesUntilLogoff <= 0)
                { // Log off the current user, unless they press a key or move the mouse in the next few seconds.
                    lastBalloonTipShown = DateTime.Now;
                    notificationIcon.ShowBalloonTip(10000, "Idle Monitor", "You are about to be logged off, unless you move the mouse or press a key.", ToolTipIcon.Warning);
                    System.Threading.Thread.Sleep(10000);
                    
                    // Check the last user input time again, in case the user generated activity while the thread was asleep.
                    minutesUntilLogoff = Settings.IdleTimeoutMinutes - (int)(NativeMethods.SecondsSinceLastInput / 60);

                    if (minutesUntilLogoff <= 0)
                    {
                        NativeMethods.LogOff(false);
                    }
                }
                else if ((minutesUntilLogoff <= Settings.WarningMinutes) && (DateTime.Now.Subtract(lastBalloonTipShown).TotalMinutes > Settings.WarningRepeatMinutes))
                { // We need to display a warning.
                    lastBalloonTipShown = DateTime.Now;
                    notificationIcon.ShowBalloonTip(10000, "Idle Monitor", string.Format("You will be logged off in {0:N0} minute{1}, unless you move the mouse or press a key.", minutesUntilLogoff, (minutesUntilLogoff == 1) ? string.Empty : "s"), ToolTipIcon.Info);
                }
            }
        }


        /// <summary>
        /// Handles the Elapsed event for the input update timer.
        /// </summary>
        /// <param name="sender">
        /// The timer whose Elapsed event has fired.
        /// </param>
        /// <param name="e">
        /// Data specific to the event.
        /// </param>
        void InputUpdateTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            CheckIdleTime();
        }
    }
}
