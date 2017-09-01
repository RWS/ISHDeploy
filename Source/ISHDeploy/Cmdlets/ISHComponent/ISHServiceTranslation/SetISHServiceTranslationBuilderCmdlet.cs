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
using ISHDeploy.Business.Operations.ISHComponent;
using ISHDeploy.Business.Operations.ISHComponent.ISHServiceTranslation;
using ISHDeploy.Common.Enums;

namespace ISHDeploy.Cmdlets.ISHComponent.ISHServiceTranslation
{
    /// <summary>
    /// <para type="synopsis">Sets translation builder windows service.</para>
    /// <para type="description">The Set-ISHServiceTranslationBuilder cmdlet sets translation builder windows service.</para>
    /// <para type="link">Enable-ISHExternalPreview</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\>Set-ISHServiceTranslationBuilder -ISHDeployment $deployment -MaxObjectsInOnePush 1000</code>
    /// <para>This command enables the translation builder windows service.
    /// Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    /// <example>
    /// <code>PS C:\>Set-ISHServiceTranslationBuilder -ISHDeployment $deployment -Count 2</code>
    /// <para>This command changes the amount of instances of services.
    /// Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    [Cmdlet(VerbsCommon.Set, "ISHServiceTranslationBuilder")]
    public sealed class SetISHServiceTranslationBuilderCmdlet : BaseHistoryEntryCmdlet
    {
        /// <summary>
        /// <para type="description">The maximum number of objects in one push.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "The maximum number of objects in one push", ParameterSetName = "TranslationBuilderSettings")]
        [ValidateNotNullOrEmpty]
        [ValidateRange(1, 10000)]
        public int MaxObjectsInOnePush { get; set; }

        /// <summary>
        /// <para type="description">The maximum number of objects in one push.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "The maximum number of job items cteated in one call", ParameterSetName = "TranslationBuilderSettings")]
        [ValidateNotNullOrEmpty]
        [ValidateRange(1, 100000)]
        public int MaxJobItemsCreatedInOneCall { get; set; }

        /// <summary>
        /// <para type="description">The life period of completed job.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "The life period of completed job", ParameterSetName = "TranslationBuilderSettings")]
        [ValidateNotNullOrEmpty]
        public TimeSpan CompletedJobLifeSpan { get; set; }

        /// <summary>
        /// <para type="description">The timeout of processing of job.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "The timeout of processing of job", ParameterSetName = "TranslationBuilderSettings")]
        [ValidateNotNullOrEmpty]
        public TimeSpan JobProcessingTimeout { get; set; }

        /// <summary>
        /// <para type="description">The interval of polling of job.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "The interval of polling of job", ParameterSetName = "TranslationBuilderSettings")]
        [ValidateNotNullOrEmpty]
        public TimeSpan JobPollingInterval { get; set; }

        /// <summary>
        /// <para type="description">The interval of polling of pending job.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "The interval of polling of pending job", ParameterSetName = "TranslationBuilderSettings")]
        [ValidateNotNullOrEmpty]
        public TimeSpan PendingJobPollingInterval { get; set; }

        /// <summary>
        /// <para type="description">The number of TranslationBuilder services in the system.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "The number of TranslationBuilder services in the system", ParameterSetName = "TranslationBuilderCount")]
        [ValidateNotNullOrEmpty]
        [ValidateRange(1, 10)]
        public int Count { get; set; }

        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {

            if (ParameterSetName == "TranslationBuilderSettings")
            {
                var parameters = new Dictionary<TranslationBuilderSetting, object>();

                foreach (var cmdletParameter in MyInvocation.BoundParameters)
                {
                    switch (cmdletParameter.Key)
                    {
                        case "MaxObjectsInOnePush":
                            parameters.Add(TranslationBuilderSetting.maxObjectsInOnePushTranslation, MaxObjectsInOnePush);
                            break;
                        case "MaxJobItemsCreatedInOneCall":
                            parameters.Add(TranslationBuilderSetting.maxTranslationJobItemsCreatedInOneCall,
                                MaxJobItemsCreatedInOneCall);
                            break;
                        case "CompletedJobLifeSpan":
                            parameters.Add(TranslationBuilderSetting.completedJobLifeSpan, CompletedJobLifeSpan);
                            break;
                        case "JobProcessingTimeout":
                            parameters.Add(TranslationBuilderSetting.jobProcessingTimeout, JobProcessingTimeout);
                            break;
                        case "JobPollingInterval":
                            parameters.Add(TranslationBuilderSetting.jobPollingInterval, JobPollingInterval);
                            break;
                        case "PendingJobPollingInterval":
                            parameters.Add(TranslationBuilderSetting.pendingJobPollingInterval,
                                PendingJobPollingInterval);
                            break;
                    }
                }

                if (parameters.Count > 0)
                {
                    var operation = new SetISHServiceTranslationBuilderOperation(Logger, ISHDeployment, parameters);

                    operation.Run();
                }
                else
                {
                    Logger.WriteWarning("The input parameter are not specified");
                }
            }
            else if (ParameterSetName == "TranslationBuilderCount")
            {
                var operation = new SetISHServiceAmountOperation(Logger, ISHDeployment, Count, ISHWindowsServiceType.TranslationBuilder);

                operation.Run();
            }
        }
    }
}
