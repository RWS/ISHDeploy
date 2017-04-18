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
            /// The pattern of xpath to attribute
            /// </summary>
            public const string TranslationBuilderSettingAttributeXPathPattern = "configuration/trisoft.infoShare.translationBuilder/settings/@{0}";

            /// <summary>
            /// Mapping between names of attributes and their xpaths
            /// </summary>
            public static Dictionary<TranslationBuilderSetting, string> AttributeXPaths = new Dictionary<TranslationBuilderSetting, string>
            {
                {TranslationBuilderSetting.maxObjectsInOnePushTranslation, string.Format(TranslationBuilderSettingAttributeXPathPattern, TranslationBuilderSetting.maxObjectsInOnePushTranslation) },
                {TranslationBuilderSetting.maxTranslationJobItemsCreatedInOneCall, string.Format(TranslationBuilderSettingAttributeXPathPattern, TranslationBuilderSetting.maxTranslationJobItemsCreatedInOneCall)  },
                {TranslationBuilderSetting.completedJobLifeSpan, string.Format(TranslationBuilderSettingAttributeXPathPattern, TranslationBuilderSetting.completedJobLifeSpan)  },
                {TranslationBuilderSetting.jobProcessingTimeout, string.Format(TranslationBuilderSettingAttributeXPathPattern, TranslationBuilderSetting.jobProcessingTimeout)  },
                {TranslationBuilderSetting.jobPollingInterval, string.Format(TranslationBuilderSettingAttributeXPathPattern, TranslationBuilderSetting.jobPollingInterval)  },
                {TranslationBuilderSetting.pendingJobPollingInterval, string.Format(TranslationBuilderSettingAttributeXPathPattern, TranslationBuilderSetting.pendingJobPollingInterval)  },
            };
        }
    }
}
