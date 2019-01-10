namespace UserIdleMonitor
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// Contains the time of the last input.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    struct LASTINPUTINFO
    {
        /// <summary>
        /// The size of the structure, in bytes.
        /// </summary>
        /// <remarks>
        /// This member must be set to sizeof(LASTINPUTINFO).
        /// </remarks>
        [MarshalAs(UnmanagedType.U4)]
        public uint cbSize;

        /// <summary>
        /// The tick count when the last input event was received.
        /// </summary>
        [MarshalAs(UnmanagedType.U4)]
        public uint dwTime;
    }
}
