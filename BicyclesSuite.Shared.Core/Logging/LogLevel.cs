namespace BicyclesSuite.Shared.Logging
{
    public enum LogLevel
    {
        /// <summary>
        /// Off log level.
        /// </summary>
        Off = 0,

        /// <summary>
        /// Trace log level.
        /// </summary>
        Trace = 1000,

        /// <summary>
        /// Debug log level.
        /// </summary>
        Debug = 2000,

        /// <summary>
        /// Info log level.
        /// </summary>
        Info = 3000,

        /// <summary>
        /// Warn log level.
        /// </summary>
        Warn = 4000,

        /// <summary>
        /// Error log level.
        /// </summary>
        Error = 5000,

        /// <summary>
        /// Fatal log level.
        /// </summary>
        Fatal = 6000
    }
}
