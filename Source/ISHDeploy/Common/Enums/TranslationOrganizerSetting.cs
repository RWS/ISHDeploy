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
    public enum TranslationOrganizerSetting
    {
        /// <summary>
        /// The DumpFolder attribute
        /// </summary>
        dumpFolder,

        /// <summary>
        /// The maxTranslationJobItemsUpdatedInOneCall attribute
        /// </summary>
        maxTranslationJobItemsUpdatedInOneCall,

        /// <summary>
        /// The jobPollingInterval attribute
        /// </summary>
        jobPollingInterval,

        /// <summary>
        /// The pendingJobPollingInterval attribute
        /// </summary>
        pendingJobPollingInterval,

        /// <summary>
        /// The systemTaskInterval attribute
        /// </summary>
        systemTaskInterval,

        /// <summary>
        /// The attemptsBeforeFailOnRetrieval attribute
        /// </summary>
        attemptsBeforeFailOnRetrieval,

        /// <summary>
        /// The updateLeasedByPerNumberOfItems attribute
        /// </summary>
        updateLeasedByPerNumberOfItems,

        /// <summary>
        /// The retriesOnTimeout attribute
        /// </summary>
        retriesOnTimeout,

        /// <summary>
        /// The synchronizeTemplates attribute
        /// </summary>
        synchronizeTemplates
    }
}
