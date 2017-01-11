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
using System.Management.Automation;
﻿using ISHDeploy.Business.Operations.ISHServiceTranslation;
using ISHDeploy.Common.Enums;

namespace ISHDeploy.Cmdlets.ISHServiceTranslation
{
    /// <summary>
    /// <para type="synopsis">Sets translation builder windows service.</para>
    /// <para type="description">The Set-ISHServiceTranslationOrganizer cmdlet sets translation builder windows service.</para>
    /// <para type="link">Enable-ISHExternalPreview</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\>Set-ISHServiceTranslationOrganizer -ISHDeployment $deployment -MaxObjectsInOnePush 1000</code>
    /// <para>This command enables the translation builder windows service.
    /// Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    [Cmdlet(VerbsCommon.Set, "ISHServiceTranslationOrganizer")]
    public sealed class SetISHServiceTranslationOrganizerCmdlet : BaseHistoryEntryCmdlet
    {
        /// <summary>
        /// <para type="description">The path to dump folder.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "The path to dump folder")]
        [ValidateNotNullOrEmpty]
        [ValidateRange(1, 10000)]
        public string DumpFolder { get; set; }

        

        /// <summary>
        /// <para type="description">The interval of polling of pending job.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "The interval of polling of pending job")]
        [ValidateNotNullOrEmpty]
        public TimeSpan PendingJobPollingInterval { get; set; }

        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            var parameters = new Dictionary<TranslationOrganizerSetting, object>();

            foreach (var cmdletParameter in MyInvocation.BoundParameters)
            {
                switch (cmdletParameter.Key)
                {
                    case "DumpFolder":
                        parameters.Add(TranslationOrganizerSetting.dumpFolder, DumpFolder);
                        break;
                }
            }

            if (parameters.Count > 0)
            {
                //var operation = new SetISHServiceTranslationBuilderOperation(Logger, ISHDeployment, parameters);

                //operation.Run();
            }
            else
            {
                Logger.WriteWarning("The input parameter are not specified");
            }
        }
    }
}
