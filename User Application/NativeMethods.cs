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

        /// <summary>
        /// Indicates the session in which the calling application is running (or the current session).
        /// </summary>
        private static readonly int WTS_CURRENT_SESSION = -1;

        /// <summary>
        /// Logs off a specified Remote Desktop Services session.
        /// </summary>
        /// <param name="serverHandle">
        /// A handle to a Remote Desktop Session Host server. 
        /// </param>
        /// <param name="sessionId">
        /// A Remote Desktop Services session identifier.
        /// </param>
        /// <param name="wait">
        /// Indicates whether the operation is synchronous.
        /// </param>
        /// <remarks>
        /// If wait is true, the function returns when the session is logged off.
        /// If wait is false, the function returns immediately.
        /// </remarks>
        /// <returns>
        /// If the function succeeds, the return value is a nonzero value.
        /// If the function fails, the return value is zero.
        /// </returns>
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

        /// <summary>
        /// Logs off the current session on the local computer.
        /// </summary>
        /// <param name="waitForLogoff">
        /// Indicates whether to wait for the logoff to complete (true) or
        /// return immediately (false).
        /// </param>
        public static void LogOff(bool waitForLogoff)
        {
            WTSLogoffSession(WTS_CURRENT_SERVER_HANDLE, WTS_CURRENT_SESSION, waitForLogoff);
        }
    }
}
