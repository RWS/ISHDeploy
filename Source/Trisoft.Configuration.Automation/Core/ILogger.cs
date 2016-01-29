using System;
using System.Management.Automation;

namespace Trisoft.Configuration.Automation.Core
{
    public interface ILogger
    {
        void WriteDetail(string text);
        void WriteDebug(string text);
        void WriteError(Exception exception, string errorId, ErrorCategory errorCategory);
        void WriteProgress(string message, string statusDescription);
        void WriteVerbose(string text);
        void WriteWarning(string text);
    }
}
