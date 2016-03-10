using System;

namespace InfoShare.Deployment.Interfaces
{
    /// <summary>
    /// Represents logging functionality.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Writes verbose message.
        /// </summary>
        /// <param name="message">Verbose message.</param>
        void WriteVerbose(string message);

        /// <summary>
        /// Reports progress.
        /// </summary>
        /// <param name="activity">Activity that takes place.</param>
        /// <param name="statusDescription">Activity description.</param>
        /// <param name="percentComplete">Complete progress in percent equivalent.</param>
        void WriteProgress(string activity, string statusDescription, int percentComplete = -1);

        /// <summary>
        /// Reports parent progress.
        /// </summary>
        /// <param name="activity">Activity that takes place.</param>
        /// <param name="statusDescription">Activity description.</param>
        /// <param name="percentComplete">Complete progress in percent equivalent.</param>
        void WriteParentProgress(string activity, string statusDescription, int percentComplete);

        /// <summary>
        /// Writes debug-useful information.
        /// </summary>
        /// <param name="message">Debug message.</param>
        void WriteDebug(string message);

        /// <summary>
        /// Writes warning message.
        /// </summary>
        /// <param name="message">Warning message.</param>
        void WriteWarning(string message);

        /// <summary>
        /// Writes non-terminating error.
        /// </summary>
        /// <param name="ex">Exception as a result of the error.</param>
        /// <param name="errorObject">Object that caused error.</param>
        void WriteError(Exception ex, object errorObject = null);
    }
}
