/*
 * Copyright (c) 2014 All Rights Reserved by the SDL Group.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Diagnostics;
using System.Text;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Common.Interfaces;

namespace ISHDeploy.Data.Managers
{
    /// <summary>
    /// Implements processes management.
    /// </summary>
    /// <seealso cref="IProcessManager" />
    public class ProcessManager : IProcessManager
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="IProcessManager"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public ProcessManager(ILogger logger)
        {
            _logger = logger;

        }

        /// <summary>
        /// Starts an executive file with parameters
        /// </summary>
        /// <param name="filePath">The path to executive file.</param>
        /// <param name="arguments">The arguments</param>
        public void Start(string filePath, string arguments = "")
        {
            _logger.WriteDebug($"Start execute of file `{filePath}` with parameters `{arguments}`");

            using (var process = new Process())
            {
                process.StartInfo.FileName = filePath;
                process.StartInfo.Arguments = arguments;
                process.StartInfo.CreateNoWindow = false;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.EnableRaisingEvents = true;

                var errors = new StringBuilder();
                var output = new StringBuilder();
                var hadErrors = false;

                process.Start();

                while (!process.StandardOutput.EndOfStream)
                {
                    string line = process.StandardOutput.ReadLine();
                    if (!string.IsNullOrEmpty(line))
                    {
                        if (!hadErrors && !string.IsNullOrEmpty(line) &&
                        !line.ToLower().Contains("exception:"))
                        {
                            output.AppendLine(line);
                        }
                        else
                        {
                            if (!hadErrors)
                            {
                                errors.AppendLine(process.StandardError.ReadToEnd());
                            }
                            hadErrors = true;
                            errors.AppendLine(line);
                        }
                    }
                }
                process.WaitForExit();

                _logger.WriteDebug(output.ToString());

                string stderr = errors.ToString();

                if (process.ExitCode != 0 || hadErrors)
                {
                    throw new Exception(stderr);
                }
            }
            _logger.WriteVerbose($"The process `{filePath}` has been finished");
        }

        /// <summary>
        /// Kills a process if it has the given id and name
        /// </summary>
        /// <param name="processID">Process ID</param>
        /// <param name="processName">Process Name</param>
        public void Kill(int processID, string processName)
        {
            _logger.WriteDebug($"Trying to stop process '{processName}' (processId={processID}) ");

            try
            {
                var process = Process.GetProcessById(processID);
                if (process != null && string.Compare(process.ProcessName, processName, true) == 0)
                {
                    _logger.WriteDebug($"Stopping process '{processName}' (processId={processID}) ");
                    process.Kill();
                    _logger.WriteVerbose($"Stopped process '{processName}' (processId={processID}) ");
                }
                else
                {
                    _logger.WriteDebug($"The process '{processName}' with processId={processID} was already stopped.");
                }
            }
            catch (ArgumentException)
            {
                _logger.WriteDebug($"The process '{processName}' with processId={processID} was already stopped.");
            }
        }
    }
}
