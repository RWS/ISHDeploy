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

using System.Collections.Generic;
using ISHDeploy.Common.Enums;

namespace ISHDeploy.Business.Operations
{
    /// <summary>
    /// Provides xpaths, search patterns and constants of deployment files
    /// </summary>
    partial class BaseOperationPaths
    {
        /// <summary>
        /// Provides constants related to ~\App\TranslationBuilder\Bin\TranslationBuilder.exe.config file
        /// </summary>
        protected class TranslationBuilderConfig
        {
            /// <summary>
            /// The xpath of "configuration/trisoft.infoShare.translationBuilder/settings/@maxObjectsInOnePushTranslation" attribute in file ~\App\TranslationBuilder\Bin\TranslationBuilder.exe.config
            /// </summary>
            public const string TranslationBuilderConfigMaxObjectsInOnePushAttributeXPath = "configuration/trisoft.infoShare.translationBuilder/settings/@maxObjectsInOnePushTranslation";

            /// <summary>
            /// The xpath of "configuration/trisoft.infoShare.translationBuilder/settings/@maxTranslationJobItemsCreatedInOneCall" attribute in file ~\App\TranslationBuilder\Bin\TranslationBuilder.exe.config
            /// </summary>
            public const string TranslationBuilderConfigMaxTranslationJobItemsCreatedInOneCallAttributeXPath = "configuration/trisoft.infoShare.translationBuilder/settings/@maxTranslationJobItemsCreatedInOneCall";

            /// <summary>
            /// The xpath of "configuration/trisoft.infoShare.translationBuilder/settings/@completedJobLifeSpan" attribute in file ~\App\TranslationBuilder\Bin\TranslationBuilder.exe.config
            /// </summary>
            public const string TranslationBuilderConfigCompletedJobLifeSpanAttributeXPath = "configuration/trisoft.infoShare.translationBuilder/settings/@completedJobLifeSpan";

            /// <summary>
            /// The xpath of "configuration/trisoft.infoShare.translationBuilder/settings/@jobProcessingTimeout" attribute in file ~\App\TranslationBuilder\Bin\TranslationBuilder.exe.config
            /// </summary>
            public const string TranslationBuilderConfigJobProcessingTimeoutAttributeXPath = "configuration/trisoft.infoShare.translationBuilder/settings/@jobProcessingTimeout";

            /// <summary>
            /// The xpath of "configuration/trisoft.infoShare.translationBuilder/settings/@jobPollingInterval" attribute in file ~\App\TranslationBuilder\Bin\TranslationBuilder.exe.config
            /// </summary>
            public const string TranslationBuilderConfigJobPollingIntervalAttributeXPath = "configuration/trisoft.infoShare.translationBuilder/settings/@jobPollingInterval";

            /// <summary>
            /// The xpath of "configuration/trisoft.infoShare.translationBuilder/settings/@pendingJobPollingInterval" attribute in file ~\App\TranslationBuilder\Bin\TranslationBuilder.exe.config
            /// </summary>
            public const string TranslationBuilderConfigPendingJobPollingIntervalAttributeXPath = "configuration/trisoft.infoShare.translationBuilder/settings/@pendingJobPollingInterval";

            /// <summary>
            /// Mapping between names of attributes and their xpaths
            /// </summary>
            public static Dictionary<TranslationBuilderSetting, string> AttributeXPaths = new Dictionary<TranslationBuilderSetting, string>
            {
                {TranslationBuilderSetting.MaxObjectsInOnePushTranslation, TranslationBuilderConfigMaxObjectsInOnePushAttributeXPath },
                {TranslationBuilderSetting.MaxTranslationJobItemsCreatedInOneCall, TranslationBuilderConfigMaxTranslationJobItemsCreatedInOneCallAttributeXPath },
                {TranslationBuilderSetting.CompletedJobLifeSpan, TranslationBuilderConfigCompletedJobLifeSpanAttributeXPath },
                {TranslationBuilderSetting.JobProcessingTimeout, TranslationBuilderConfigJobProcessingTimeoutAttributeXPath },
                {TranslationBuilderSetting.JobPollingInterval, TranslationBuilderConfigJobPollingIntervalAttributeXPath },
                {TranslationBuilderSetting.PendingJobPollingInterval, TranslationBuilderConfigPendingJobPollingIntervalAttributeXPath },
            };
        }
    }
}
