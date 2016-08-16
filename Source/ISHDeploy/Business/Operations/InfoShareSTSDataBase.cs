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

namespace ISHDeploy.Business.Operations
{
    /// <summary>
    /// Provides xpaths, search patterns and constants of deployment files
    /// </summary>
    partial class BaseOperationPaths
    {
        /// <summary>
        /// Provides constants related to ~\Web\InfoShareSTS\App_Data\IdentityServerConfiguration-2.2.sdf file
        /// </summary>
        protected class InfoShareSTSDataBase
        {
            /// <summary>
            /// Relying Parties table name
            /// </summary>
            public const string RelyingPartiesTableName = "RelyingParties";

            /// <summary>
            /// The certificate update command in KeyMaterialConfiguration table
            /// </summary>
            public const string UpdateCertificateInKeyMaterialConfigurationSQLCommandFormat =
                "UPDATE KeyMaterialConfiguration SET SigningCertificateName = '{0}' Where  SymmetricSigningKey in (SELECT TOP 1 SymmetricSigningKey FROM KeyMaterialConfiguration)";


            /// <summary>
            /// The certificate update command in RelyingParties table
            /// </summary>
            public const string UpdateCertificateInRelyingPartiesSQLCommandFormat =
                "UPDATE RelyingParties SET EncryptingCertificate='{0}' WHERE Realm IN ({1})";

            /// <summary>
            /// The select RelyingParties query
            /// </summary>
            public const string GetRelyingPartySQLCommandFormat = "SELECT Realm, Name, Enabled, EncryptingCertificate FROM RelyingParties";

            /// <summary>
            /// Gets the array of SVC paths.
            /// </summary>
            /// <value>
            /// The list of SVC paths.
            /// </value>
            /// <param name="baseUrl">The base URL.</param>
            /// <param name="webAppNameWS">The web name of WS application.</param>
            /// <returns></returns>
            public static string[] GetSvcPaths(string baseUrl, string webAppNameWS)
            {
                return new[]
                {
                    $"'{baseUrl}/{webAppNameWS}/'",
                    $"'{baseUrl}/{webAppNameWS}/Wcf/API25/Application.svc'",
                    $"'{baseUrl}/{webAppNameWS}/Wcf/API25/Baseline.svc'",
                    $"'{baseUrl}/{webAppNameWS}/Wcf/API25/DocumentObj.svc'",
                    $"'{baseUrl}/{webAppNameWS}/Wcf/API25/EDT.svc'",
                    $"'{baseUrl}/{webAppNameWS}/Wcf/API25/EventMonitor.svc'",
                    $"'{baseUrl}/{webAppNameWS}/Wcf/API25/Folder.svc'",
                    $"'{baseUrl}/{webAppNameWS}/Wcf/API25/ListOfValues.svc'",
                    $"'{baseUrl}/{webAppNameWS}/Wcf/API25/MetadataBinding.svc'",
                    $"'{baseUrl}/{webAppNameWS}/Wcf/API25/OutputFormat.svc'",
                    $"'{baseUrl}/{webAppNameWS}/Wcf/API25/PublicationOutput.svc'",
                    $"'{baseUrl}/{webAppNameWS}/Wcf/API25/Search.svc'",
                    $"'{baseUrl}/{webAppNameWS}/Wcf/API25/Settings.svc'",
                    $"'{baseUrl}/{webAppNameWS}/Wcf/API25/TranslationJob.svc'",
                    $"'{baseUrl}/{webAppNameWS}/Wcf/API25/TranslationTemplate.svc'",
                    $"'{baseUrl}/{webAppNameWS}/Wcf/API25/User.svc'",
                    $"'{baseUrl}/{webAppNameWS}/Wcf/API25/UserGroup.svc'",
                    $"'{baseUrl}/{webAppNameWS}/Wcf/API25/UserRole.svc'",
                    $"'{baseUrl}/{webAppNameWS}/Wcf/API20/Application.svc'",
                    $"'{baseUrl}/{webAppNameWS}/Wcf/API20/DocumentObj.svc'",
                    $"'{baseUrl}/{webAppNameWS}/Wcf/API20/EDT.svc'",
                    $"'{baseUrl}/{webAppNameWS}/Wcf/API20/Folder.svc'",
                    $"'{baseUrl}/{webAppNameWS}/Wcf/API20/MetaDataAssist.svc'",
                    $"'{baseUrl}/{webAppNameWS}/Wcf/API20/OutputFormat.svc'",
                    $"'{baseUrl}/{webAppNameWS}/Wcf/API20/Publication.svc'",
                    $"'{baseUrl}/{webAppNameWS}/Wcf/API20/PublicationOutput.svc'",
                    $"'{baseUrl}/{webAppNameWS}/Wcf/API20/Reports.svc'",
                    $"'{baseUrl}/{webAppNameWS}/Wcf/API20/Search.svc'",
                    $"'{baseUrl}/{webAppNameWS}/Wcf/API20/Settings.svc'",
                    $"'{baseUrl}/{webAppNameWS}/Wcf/API20/Workflow.svc'",
                    $"'{baseUrl}/{webAppNameWS}/Wcf/API/Application.svc'",
                    $"'{baseUrl}/{webAppNameWS}/Wcf/API/ConditionManagement.svc'"
                };
            }
        }
    }
}
