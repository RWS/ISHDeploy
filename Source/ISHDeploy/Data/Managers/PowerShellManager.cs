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
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Reflection;
using System.Text;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Common.Interfaces;

namespace ISHDeploy.Data.Managers
{
    /// <summary>
    /// Allows to run powershell scripts from c# code.
    /// </summary>
    /// <seealso cref="IWebAdministrationManager" />
    public class PowerShellManager : IPowerShellManager
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebAdministrationManager"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public PowerShellManager(ILogger logger)
        {
            _logger = logger;

        }

        /// <summary>
        /// Invokes content of ps1 file as powershell script
        /// </summary>
        /// <param name="embeddedResourceName">Name of embedded ps1 resource with powershell script.</param>
        /// <param name="parameters">Parameters to be passed to the script. Is <c>null</c> by defult</param>
        /// <param name="actionDescription">Description of action to add this information if powershell script running is failed.</param>
        /// Return the result in the case of casting failure
        /// <returns>An object of specified type that converted from PSObject</returns>
        public object InvokeEmbeddedResourceAsScriptWithResult(string embeddedResourceName, Dictionary<string, string> parameters = null, string actionDescription = null)
        {
            var result = new List<object>();
            using (var ps = PowerShell.Create())
            {
                // Read Check-WindowsAuthenticationFeatureEnabled.ps1 script
                using (var resourceReader = Assembly.GetExecutingAssembly().GetManifestResourceStream(embeddedResourceName))
                {
                    using (var reader = new StreamReader(resourceReader))
                    {
                        var script = reader.ReadToEnd();

                        if (parameters != null)
                        {
                            foreach (var param in parameters)
                            {
                                script = script.Replace(param.Key, param.Value.Replace("\"", "`\""));
                            }
                        }
                        ps.AddScript(script);
                        ps.AddParameter("NoProfile");
                    }
                }
                
                // Add results
                result.AddRange(ps.Invoke());

                // Add Verboses, Warnings and Exceptions
                foreach (var verbose in ps.Streams.Verbose.ReadAll())
                {
                    _logger.WriteVerbose(verbose.Message);
                }

                foreach (var warning in ps.Streams.Warning.ReadAll())
                {
                    _logger.WriteWarning(warning.Message);
                }

                bool isFailed = false;
                var exceptions = new StringBuilder();
                foreach (var error in ps.Streams.Error.ReadAll())
                {
                    isFailed = true;
                    exceptions.AppendLine(error.Exception.Message);
                }

                if (isFailed)
                {
                    if (string.IsNullOrEmpty(actionDescription))
                    {
                        throw new Exception(exceptions.ToString());
                    }
                    else
                    {
                        _logger.WriteDebug($"{actionDescription} is failed.");
                        throw new Exception($"{exceptions}");
                    }
                }
            }

            return result.Count == 1 ? result[0] : result;
        }
    }
}
