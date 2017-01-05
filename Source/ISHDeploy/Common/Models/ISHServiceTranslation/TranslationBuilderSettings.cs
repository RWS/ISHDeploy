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

using System.Xml.Serialization;

namespace ISHDeploy.Common.Models.ISHServiceTranslation
{

    /// <summary>
    /// <para type="description">Represents settings element of ~\App\TranslationBuilder\Bin\TranslationBuilder.exe.config file.</para>
    /// </summary>
    [XmlRoot("settings", Namespace = "")]
    public class Settings
    {
        /// <summary>
        /// The attribute configuration/trisoft.infoShare.translationBuilder/settings[maxObjectsInOnePushTranslation] in file ~\App\TranslationBuilder\Bin\TranslationBuilder.exe.config
        /// </summary>
        [XmlAttributeAttribute(AttributeName = "maxObjectsInOnePushTranslation")]
        public ushort MaxObjectsInOnePushTranslation { get; set; }

        /// <summary>
        /// The attribute configuration/trisoft.infoShare.translationBuilder/settings[maxTranslationJobItemsCreatedInOneCall] in file ~\App\TranslationBuilder\Bin\TranslationBuilder.exe.config
        /// </summary>
        [XmlAttributeAttribute(AttributeName = "maxTranslationJobItemsCreatedInOneCall")]
        public ushort MaxTranslationJobItemsCreatedInOneCall { get; set; }

        /// <summary>
        /// The attribute configuration/trisoft.infoShare.translationBuilder/settings[completedJobLifeSpan] in file ~\App\TranslationBuilder\Bin\TranslationBuilder.exe.config
        /// </summary>
        [XmlAttributeAttribute(AttributeName = "completedJobLifeSpan")]
        public string CompletedJobLifeSpan { get; set; }

        /// <summary>
        /// The attribute configuration/trisoft.infoShare.translationBuilder/settings[jobProcessingTimeout] in file ~\App\TranslationBuilder\Bin\TranslationBuilder.exe.config
        /// </summary>
        [XmlAttributeAttribute(AttributeName = "jobProcessingTimeout", DataType = "time")]
        public System.DateTime JobProcessingTimeout { get; set; }

        /// <summary>
        /// The attribute configuration/trisoft.infoShare.translationBuilder/settings[jobPollingInterval] in file ~\App\TranslationBuilder\Bin\TranslationBuilder.exe.config
        /// </summary>
        [XmlAttributeAttribute(AttributeName = "jobPollingInterval", DataType = "time")]
        public System.DateTime JobPollingInterval { get; set; }

        /// <summary>
        /// The attribute configuration/trisoft.infoShare.translationBuilder/settings[pendingJobPollingInterval] in file ~\App\TranslationBuilder\Bin\TranslationBuilder.exe.config
        /// </summary>
        [XmlAttributeAttribute(AttributeName = "pendingJobPollingInterval", DataType = "time")]
        public System.DateTime PendingJobPollingInterval { get; set; }

    }
}
