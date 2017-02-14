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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Runtime.InteropServices;
using System.Text;
using ISHDeploy.Business.Operations.ISHDeployment;
using ISHDeploy.Cmdlets.ISHPackage;
using ISHDeploy.Common.Models.TranslationOrganizer;

namespace ISHDeploy.Cmdlets
{
    /// <summary>
    /// Base cmdlet class that writes cmdlet usage into history info
    /// </summary>
    public abstract class BaseHistoryEntryCmdlet : BaseISHDeploymentAdminRightsCmdlet
    {
        /// <summary>
        /// History prefix for Copy and Expand cmdlet
        /// </summary>
        private readonly string _prefix =
@"if($IncludeCustomFile)
{
     ";
        /// <summary>
        /// History postfix for Copy and Expand cmdlet
        /// </summary>
        private readonly string _postfix =
@"
}";

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
            // don't log if cmdlet was executed with WhatIf parameter
            if (MyInvocation.BoundParameters.ContainsKey("WhatIf"))
            {
                Logger.WriteVerbose("Commandlet was executed with `-WhatIf` parameter, no history logging required.");
                return;
            }

            // TS-11913
            string prefix = "", postfix = "";
            if (this.GetType() == typeof(CopyISHCMFileCmdlet) ||
                this.GetType() == typeof(ExpandISHCMPackageCmdlet))
            {
                prefix = _prefix;
                postfix = _postfix;
            }

            var operation = new AddHistoryEntryOperation(
                Logger,
                ISHDeployment,
                prefix + GetInvocationLine() + postfix,
                MyInvocation.MyCommand.Module.Version);

            operation.Run();
        }

        /// <summary>
        /// Describes which cmdlet was executed with which parameters
        /// </summary>
        private string GetInvocationLine()
        {
            var strBldr = new StringBuilder(MyInvocation.MyCommand.Name);

            // If default ISHDeployment without name was invoked
            if (MyInvocation.BoundParameters.All(a => a.Key != "ISHDeployment"))
            {
                strBldr.Append(" -ISHDeployment $deploymentName");
            }

            foreach (var boundParameter in MyInvocation.BoundParameters)
            {
                var historyParameter = ToHistoryParameter(boundParameter);

                if (historyParameter.HasValue)
                {
                    strBldr.Append($" -{historyParameter.Value.Key} {historyParameter.Value.Value}");
                }
            }

            return strBldr.ToString();
        }

        /// <summary>
        /// Converts boundParameter to the form how it will be presented in the history file.
        /// </summary>
        /// <param name="boundParameter">Parameter from cmdlet.</param>
        /// <returns>Converted boundParameter to history parameter.</returns>
        private KeyValuePair<string, object>? ToHistoryParameter(KeyValuePair<string, object> boundParameter)
        {
            if (string.Compare(boundParameter.Key, "ISHDeployment", StringComparison.OrdinalIgnoreCase) == 0)
            {
                return new KeyValuePair<string, object>(boundParameter.Key, "$deploymentName");
            }

            if (boundParameter.Value is SwitchParameter)
            {
                return ((SwitchParameter)boundParameter.Value).IsPresent
                    ? new KeyValuePair<string, object>(boundParameter.Key, string.Empty)
                    : (KeyValuePair<string, object>?)null;
            }

            if (boundParameter.Value is string)
            {
                var stringValue = boundParameter.Value.ToString().Replace("\"", "\"\"");
                return new KeyValuePair<string, object>(boundParameter.Key, $"\"{stringValue}\"");
            }

            if (boundParameter.Value is bool)
            {
                return new KeyValuePair<string, object>(boundParameter.Key, $"${boundParameter.Value}");
            }

            if (boundParameter.Value is IEnumerable)
            {
                var arrayStringValue = string.Join(", ", ((IEnumerable)boundParameter.Value).Cast<object>().Select(x => ModelToHistoryFormater.GetString(x, true)));
                return string.IsNullOrEmpty(arrayStringValue)
                    ? (KeyValuePair<string, object>?)null
                    : new KeyValuePair<string, object>(boundParameter.Key, $"@({arrayStringValue})");

            }

            if (boundParameter.Value is PSCredential)
            {
                var model = boundParameter.Value as PSCredential;
                var valueAsString = $"(New-Object System.Management.Automation.PSCredential (\"{model.UserName}\", (ConvertTo-SecureString \"{Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(model.Password))}\" -AsPlainText -Force)))";
                return new KeyValuePair<string, object>(boundParameter.Key, valueAsString);
            }

            if (boundParameter.Value is ISHFieldMetadata)
            {
                var model = boundParameter.Value as ISHFieldMetadata;
                var valueAsString = $"(New-ISHFieldMetadata -Name {model.Name} -Level {model.Level} -ValueType {model.ValueType})";
                return new KeyValuePair<string, object>(boundParameter.Key, valueAsString);
            }

            return boundParameter;
        }
    }
}
