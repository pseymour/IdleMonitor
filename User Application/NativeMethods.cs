namespace UserIdleMonitor
{
    using System;
    using System.Runtime.InteropServices;

    internal class NativeMethods
    {
        /// <summary>
        /// A Remote Desktop Session Host server value for the computer on which the application is running.
        /// </summary>
        private static readonly IntPtr WTS_CURRENT_SERVER_HANDLE = IntPtr.Zero;

        private static readonly int WTS_CURRENT_SESSION = -1;

        [DllImport("wtsapi32.dll", SetLastError = true)]
        private static extern int WTSLogoffSession(System.IntPtr serverHandle, [MarshalAs(UnmanagedType.U4)] int sessionId, [MarshalAs(UnmanagedType.Bool)] bool wait);

        /// <summary>
        /// Retrieves the time of the last input event.
        /// </summary>
        /// <param name="lastInput">
        /// A LASTINPUTINFO structure that receives the time of the last input event.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero.
        /// </returns>
        /// <remarks>
        /// This function is useful for input idle detection. However, GetLastInputInfo does not provide
        /// system-wide user input information across all running sessions. Rather, GetLastInputInfo provides
        /// session-specific user input information for only the session that invoked the function.
        /// </remarks>
        [DllImport("user32.dll")]
        private static extern int GetLastInputInfo(out LASTINPUTINFO lastInput);

        /// <summary>
        /// Gets the number of seconds since the last input event in the current user session.
        /// </summary>
        /// <remarks>
        /// If the function is unable to determine the last input event time, float.MaxValue is returned.
        /// </remarks>
        public static float SecondsSinceLastInput
        {
            get
            {
                LASTINPUTINFO lastInput = new LASTINPUTINFO();
                lastInput.cbSize = (uint)Marshal.SizeOf(lastInput);
                int returnCode = GetLastInputInfo(out lastInput);
                if (returnCode == 0)
                { // The function call failed for GetLastInputInfo().
                    return float.MaxValue;
                }
                else
                {
                    uint elapsedTicks = (uint)Environment.TickCount - lastInput.dwTime;
                    return elapsedTicks / 1000.0f;
                }
            }
        }

        public static void LogOff(bool wait)
        {
            WTSLogoffSession(WTS_CURRENT_SERVER_HANDLE, WTS_CURRENT_SESSION, wait);
        }
    }
}
