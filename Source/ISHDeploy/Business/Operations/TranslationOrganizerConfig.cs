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
        /// Provides constants related to ~\App\TranslationOrganizer\Bin\TranslationOrganizer.exe.config file
        /// </summary>
        protected class TranslationOrganizerConfig
        {
            /// <summary>
            /// The xpath of "configuration/trisoft.utilities.serviceReferences/serviceUser/issuer" element in file ~\App\TranslationOrganizer\Bin\TranslationOrganizer.exe.config
            /// </summary>
            public const string WSTrustEndpointUrlXPath = "configuration/trisoft.utilities.serviceReferences/serviceUser/issuer";

            /// <summary>
            /// The attribute name of "configuration/trisoft.utilities.serviceReferences/serviceUser/issuer" element in file ~\App\TranslationOrganizer\Bin\TranslationOrganizer.exe.config where endpoint url should be updated
            /// </summary>
            public const string WSTrustEndpointUrlAttributeName = "wsTrustEndpoint";

            /// <summary>
            /// The attribute name of "configuration/trisoft.utilities.serviceReferences/serviceUser/issuer" element in file ~\App\TranslationOrganizer\Bin\TranslationOrganizer.exe.config where BindingType should be updated
            /// </summary>
            public const string WSTrustBindingTypeAttributeName = "wsTrustBindingType";

            /// <summary>
            /// The xpath of "configuration/trisoft.utilities.serviceReferences/serviceUser/uri/@infoShareWSServiceCertificateValidationMode" element in ~\App\TranslationOrganizer\Bin\TranslationOrganizer.exe.config file
            /// </summary>
            public const string InfoShareWSServiceCertificateValidationModeAttributeXPath = "configuration/trisoft.utilities.serviceReferences/serviceUser/uri/@infoShareWSServiceCertificateValidationMode";

            /// <summary>
            /// The pattern of xpath to attribute
            /// </summary>
            public const string TranslationOrganizerSettingsAttributeXPathPattern = "configuration/trisoft.infoShare.translationOrganizer/settings/@{0}";

            /// <summary>
            /// Mapping between names of attributes and their xpaths
            /// </summary>
            public static Dictionary<TranslationOrganizerSetting, string> AttributeXPaths = new Dictionary<TranslationOrganizerSetting, string>
            {
                {TranslationOrganizerSetting.dumpFolder, string.Format(TranslationOrganizerSettingsAttributeXPathPattern, TranslationOrganizerSetting.dumpFolder) },
                {TranslationOrganizerSetting.maxTranslationJobItemsUpdatedInOneCall, string.Format(TranslationOrganizerSettingsAttributeXPathPattern, TranslationOrganizerSetting.maxTranslationJobItemsUpdatedInOneCall) },
                {TranslationOrganizerSetting.jobPollingInterval, string.Format(TranslationOrganizerSettingsAttributeXPathPattern, TranslationOrganizerSetting.jobPollingInterval) },
                {TranslationOrganizerSetting.pendingJobPollingInterval, string.Format(TranslationOrganizerSettingsAttributeXPathPattern, TranslationOrganizerSetting.pendingJobPollingInterval) },
                {TranslationOrganizerSetting.systemTaskInterval, string.Format(TranslationOrganizerSettingsAttributeXPathPattern, TranslationOrganizerSetting.systemTaskInterval) },
                {TranslationOrganizerSetting.attemptsBeforeFailOnRetrieval, string.Format(TranslationOrganizerSettingsAttributeXPathPattern, TranslationOrganizerSetting.attemptsBeforeFailOnRetrieval) },
                {TranslationOrganizerSetting.updateLeasedByPerNumberOfItems, string.Format(TranslationOrganizerSettingsAttributeXPathPattern, TranslationOrganizerSetting.updateLeasedByPerNumberOfItems) },
                {TranslationOrganizerSetting.retriesOnTimeout, string.Format(TranslationOrganizerSettingsAttributeXPathPattern, TranslationOrganizerSetting.retriesOnTimeout) },
                {TranslationOrganizerSetting.synchronizeTemplates, string.Format(TranslationOrganizerSettingsAttributeXPathPattern, TranslationOrganizerSetting.synchronizeTemplates) }
            };

            /// <summary>
            /// The xpath of World Server Node
            /// </summary>
            public const string WorldServerNodeXPath = "configuration/trisoft.infoShare.translationOrganizer/worldServer/instances/add";
        }
    }
}
