using System;
using System.Management.Automation;
using Trisoft.Configuration.Automation.Core;

namespace Trisoft.Configuration.Automation.Cmdlets
{
    public sealed class CmdletsLogger : ILogger
    {
        private static readonly CmdletsLogger _instance = new CmdletsLogger();
        private static Cmdlet _cmdlet;

        public static ILogger Instance(Cmdlet cmdlet)
        {
            _cmdlet = cmdlet;
            return _instance;
        }

        public void WriteDetail(string text)
        {
            _cmdlet.WriteCommandDetail(text);
        }

        public void WriteDebug(string text)
        {
            _cmdlet.WriteDebug(text);
        }

        public void WriteError(Exception exception, string errorId, ErrorCategory errorCategory)
        {
            _cmdlet.WriteError(new ErrorRecord(exception, errorId, errorCategory, null));
        }

        public void WriteProgress(string message, string statusDescription)
        {
            _cmdlet.WriteProgress(new ProgressRecord(0, message, statusDescription));
        }

        public void WriteVerbose(string text)
        {
            _cmdlet.WriteVerbose(text);
        }

        public void WriteWarning(string text)
        {
            _cmdlet.WriteWarning(text);
        }
    }
}
