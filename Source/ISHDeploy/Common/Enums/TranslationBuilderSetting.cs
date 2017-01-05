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
namespace ISHDeploy.Common.Enums
{
    /// <summary>
    /// Names of attributes of element "configuration/trisoft.infoShare.translationBuilder/settings" in file ~\App\TranslationBuilder\Bin\TranslationBuilder.exe.config
    ///	<para type="description">Enumeration of Binding Types.</para>
    /// </summary>
    public enum TranslationBuilderSetting
    {
        /// <summary>
        /// The MaxObjectsInOnePushTranslation attribute
        /// </summary>
        MaxObjectsInOnePushTranslation,

        /// <summary>
        /// The MaxTranslationJobItemsCreatedInOneCall attribute
        /// </summary>
        MaxTranslationJobItemsCreatedInOneCall,

        /// <summary>
        /// The CompletedJobLifeSpan attribute
        /// </summary>
        CompletedJobLifeSpan,

        /// <summary>
        /// The JobProcessingTimeout attribute
        /// </summary>
        JobProcessingTimeout,

        /// <summary>
        /// The JobPollingInterval attribute
        /// </summary>
        JobPollingInterval,

        /// <summary>
        /// The PendingJobPollingInterval attribute
        /// </summary>
        PendingJobPollingInterval
    }
}
