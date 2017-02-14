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


using System.Management.Automation;
using System.Runtime.InteropServices;
using ISHDeploy.Common.Models.TranslationOrganizer;

namespace ISHDeploy.Cmdlets
{
    /// <summary>
    /// <para type="description">Converts an object into value or to PowerShell object by using appropriate cmdlet.</para>
    /// </summary>
    public class ModelToHistoryFormater
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj">The object that need to be converted</param>
        /// <param name="needQuotesIfObjectIsNotRecognized">Add quotes if needed</param>
        /// <returns></returns>
        public static string GetString<T>(T obj, bool needQuotesIfObjectIsNotRecognized = false)
        {
            if (obj is ISHLanguageToWorldServerLocaleIdMapping)
            {
                var model = obj as ISHLanguageToWorldServerLocaleIdMapping;
                return
                    $"(New-ISHIntegrationWorldServerMapping -ISHLanguage {model.ISHLanguage} -WSLocaleID {model.WSLocaleID})";
            }

            if (obj is ISHLanguageToTmsLanguageMapping)
            {
                var model = obj as ISHLanguageToTmsLanguageMapping;
                return
                    $"(New-ISHIntegrationTMSMapping -TmsLanguage {model.TmsLanguage} -ISHLanguage {model.ISHLanguage})";
            }

            if (obj is TmsTemplate)
            {
                var model = obj as TmsTemplate;
                return
                    $"(New-ISHIntegrationTMSTemplate -TemplateName {model.TemplateId} -TemplateId {model.TemplateName})";
            }

            return needQuotesIfObjectIsNotRecognized ? $"\"{obj}\"" : $"{obj}";
        }
    }
}
