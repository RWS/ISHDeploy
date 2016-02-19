using System;
using System.IO;
using System.Threading;

namespace InfoShare.Deployment.Cmdlets
{
    /// <summary>
    /// Base cmdlet class that writes cmdlet usage into history info
    /// </summary>
    public abstract class BaseHistoryEntryCmdlet : BaseCmdlet
    {
        /// <summary>
        /// History file name format
        /// </summary>
        private const string HistoryFileFormat = "{0}_history.ps1";

        /// <summary>
        /// Mutex that prevents several PowerShell processes to write into same history file simultaneously
        /// </summary>
        public static readonly Mutex HistoryFileMutex = new Mutex(false, @"Global\InfoShareDeploymentHistory");

        /// <summary>
        /// Provides the path to history file by deployment suffix
        /// </summary>
        /// <param name="deploymentSuffix">Deployment suffix</param>
        /// <returns>Full path to history file</returns>
        public static string GetHistoryFilePath(string deploymentSuffix)
        {
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "InfoShare.Deployment", "CmdletsHistory", HistoryFileFormat);

            return string.Format(filePath, deploymentSuffix);
        }

        /// <summary>
        /// Abstract property that describes cmdlet usage and all parameters it was called with
        /// </summary>
        protected abstract string HistoryEntry { get; }

        /// <summary>
        /// Deployment Suffix
        /// </summary>
        protected abstract string DeploymentSuffix { get; }

        /// <summary>
        /// Overrides ProcessRecord from Cmdlet class
        /// </summary>
        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            AddHistoryEntry();
        }

        /// <summary>
        /// Appends new record to history file
        /// </summary>
        private void AddHistoryEntry()
        {
            if (string.IsNullOrEmpty(HistoryEntry))
            {
                throw new ArgumentException($"History entry is empty. Cmdlet {this.GetType()} should provide history entry.");
            }

            bool isNewFile = !File.Exists(GetHistoryFilePath(DeploymentSuffix));

            if (isNewFile)
            {
                var directory = Path.GetDirectoryName(GetHistoryFilePath(DeploymentSuffix));

                if (directory != null && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }

            HistoryFileMutex.WaitOne();
            try
            {
                using (var fileStream = new StreamWriter(GetHistoryFilePath(DeploymentSuffix), true))
                {
                    if (isNewFile)
                    {
                        fileStream.WriteLine($"$deployment = Get-ISHDeployment -Deployment '{DeploymentSuffix}'");
                    }

                    fileStream.WriteLine(HistoryEntry + " -ISHDeployment $deployment");
                }
            }
            finally
            {
                HistoryFileMutex.ReleaseMutex();
            }
        }
    }
}
