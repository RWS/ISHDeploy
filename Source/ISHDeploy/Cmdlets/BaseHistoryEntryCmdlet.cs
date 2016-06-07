using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using ISHDeploy.Business.Operations.ISHDeployment;

namespace ISHDeploy.Cmdlets
{
    /// <summary>
    /// Base cmdlet class that writes cmdlet usage into history info
    /// </summary>
    public abstract class BaseHistoryEntryCmdlet : BaseISHDeploymentCmdlet
    {
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
				Logger.WriteVerbose($"Commandlet was executed with `-WhatIf` parameter, no history logging required.");
				return;
            }
            
            var operation = new AddHistoryEntryOperation(Logger, ISHDeployment, GetInvocationLine());

            operation.Run();
        }

        /// <summary>
        /// Describes which cmdlet was executed with which parameters
        /// </summary>
        private string GetInvocationLine()
        {
            var strBldr = new StringBuilder(MyInvocation.MyCommand.Name);

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
                return new KeyValuePair<string, object>(boundParameter.Key, "$deployment");
            }

            if (boundParameter.Value is SwitchParameter)
            {
                return ((SwitchParameter) boundParameter.Value).IsPresent
                    ? new KeyValuePair<string, object>(boundParameter.Key, string.Empty)
                    : (KeyValuePair<string, object>?)null;
            }

            if (boundParameter.Value is string)
            {
                var stringValue = boundParameter.Value.ToString().Replace("\"", "\"\"");
                return new KeyValuePair<string, object>(boundParameter.Key, $"\"{stringValue}\"");
            }

			if (boundParameter.Value is IEnumerable)
			{
				var arrayStringValue = string.Join(", ", ((IEnumerable<string>)boundParameter.Value).Select(x => $"\"{x.Replace("\"", "\"\"")}\""));
				return string.IsNullOrEmpty(arrayStringValue)
					? (KeyValuePair<string, object>?) null
					: new KeyValuePair<string, object>(boundParameter.Key, $"@({arrayStringValue})");

			}

			return boundParameter;
        }
    }
}
