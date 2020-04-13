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

            this.notificationIcon = new NotifyIcon
            {
                Icon = UserIdleMonitor.Properties.Resources.hourglass,
                Text = "Idle Monitor"
            };

#if DEBUG
            System.Text.StringBuilder iconText = new System.Text.StringBuilder("Idle Monitor");
            iconText.Append(System.Environment.NewLine);
            iconText.Append(string.Format("Timeout: {0}", Settings.IdleTimeoutMinutes));
            iconText.Append(System.Environment.NewLine);
            iconText.Append(string.Format("Warning: {0}", Settings.WarningMinutes));
            iconText.Append(System.Environment.NewLine);
            iconText.Append(string.Format("Repeat: {0}", Settings.WarningRepeatMinutes));
            this.notificationIcon.Text = iconText.ToString();
#endif

            this.notificationIcon.Visible = true;

            this.ThreadExit += AppContext_ThreadExit;

            this.inputUpdateTimer = new System.Timers.Timer(10000);
            this.inputUpdateTimer.Elapsed += new System.Timers.ElapsedEventHandler(InputUpdateTimerElapsed);

            this.inputUpdateTimer.Start();
        }

        private void AppContext_ThreadExit(object sender, EventArgs e)
        {
            this.notificationIcon.Visible = false;
            this.notificationIcon.Dispose();

            this.inputUpdateTimer.Stop();
            this.inputUpdateTimer = null;
        }

        private void CheckIdleTimeAndLock()
        {
            int minutesUntilLogoff = Settings.IdleTimeoutMinutes - (int)(NativeMethods.SecondsSinceLastInput / 60);
            if (this.inputUpdateTimer.Enabled)
            {
                if (minutesUntilLogoff <= 0)
                { // Log off the current user.
                    this.lastBalloonTipShown = DateTime.Now;
                    this.notificationIcon.ShowBalloonTip(10000, "Idle Monitor", "You are about to be logged off, unless you move the mouse or press a key.", ToolTipIcon.Warning);
                    System.Threading.Thread.Sleep(10000);
                    minutesUntilLogoff = Settings.IdleTimeoutMinutes - (int)(NativeMethods.SecondsSinceLastInput / 60);
                    if (minutesUntilLogoff <= 0)
                    {
                        NativeMethods.LogOff(false);
                    }
                }
                else if (minutesUntilLogoff <= Settings.WarningMinutes)
                { // We may need to display a warning.

                    if (DateTime.Now.Subtract(lastBalloonTipShown).TotalMinutes > Settings.WarningRepeatMinutes)
                    {
                        this.lastBalloonTipShown = DateTime.Now;
                        this.notificationIcon.ShowBalloonTip(10000, "Idle Monitor", string.Format("You will be logged off in {0:N0} minute{1}, unless you move the mouse or press a key.", minutesUntilLogoff, (minutesUntilLogoff == 1) ? string.Empty : "s"), ToolTipIcon.Info);
                    }
                }
            }
        }

        void InputUpdateTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.CheckIdleTimeAndLock();
        }
    }
}
