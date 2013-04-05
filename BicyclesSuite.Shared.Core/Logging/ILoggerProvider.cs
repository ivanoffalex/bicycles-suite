using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BicyclesSuite.Shared.Logging
{
    public interface ILoggerProvider
    {
        void Trace(string message, params object[] args);
        void Trace(Exception ex, string message = null);

        void Debug(string message, params object[] args);
        void Debug(Exception ex, string message = null);

        void Info(string message, params object[] args);
        void Info(Exception ex, string message = null);

        void Warn(string message, params object[] args);
        void Warn(Exception ex, string message = null);

        void Error(string message, params object[] args);
        void Error(Exception ex, string message = null);

        void Fatal(string message, params object[] args);
        void Fatal(Exception ex, string message = null);

        void Log(LogLevel level, string message, params object[] args);
        void Log(LogLevel level, Exception ex, string message = null);
    }
}
