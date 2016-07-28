/**
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
using ISHDeploy.Models;

namespace ISHDeploy.Business.Operations
{
    /// <summary>
    /// Provides absolute paths to all ISH files that are going to be used
    /// Also provides xpaths to XML elements and attributes in these files
    /// </summary>
    public partial class BasePathsOperation
    {
        /// <summary>
        /// The path to ~\Web\InfoShareSTS\App_Data\IdentityServerConfiguration-2.2.sdf
        /// </summary>
        protected static class InfoShareSTSDataBase
        {
            /// <summary>
            /// The path to ~\Web\InfoShareSTS\App_Data\IdentityServerConfiguration-2.2.sdf
            /// </summary>
            public static ISHFilePath Path => new ISHFilePath(ISHDeploymentInternal, ISHFilePath.IshDeploymentType.Web,
                @"InfoShareSTS\App_Data\IdentityServerConfiguration-2.2.sdf");

            /// <summary>
            /// Gets the connection string.
            /// </summary>
            /// <value>
            /// The connection string.
            /// </value>
            public static string ConnectionString => $"Data Source = {Path.AbsolutePath}";

            /// <summary>
            /// Relying Parties table name
            /// </summary>
            public const string RelyingPartiesTableName = "RelyingParties";

            /// <summary>
            /// The certificate update command in RelyingParties table
            /// </summary>
            public const string UpdateCertificateSQLCommandFormat = "UPDATE RelyingParties SET EncryptingCertificate='{0}' WHERE Realm IN ({1})";

            /// <summary>
            /// The select RelyingParties query
            /// </summary>
            public const string GetRelyingPartySQLCommandFormat = "SELECT Id, Name, Enabled, EncryptingCertificate FROM RelyingParties";

            /// <summary>
            /// Gets the array of SVC paths.
            /// </summary>
            /// <value>
            /// The list of SVC paths.
            /// </value>
            public static string[] SvcPaths => new[]
            {
                $"'{ISHDeploymentInternal.BaseUrl}/{ISHDeploymentInternal.WebAppNameWS}/'",
                $"'{ISHDeploymentInternal.BaseUrl}/{ISHDeploymentInternal.WebAppNameWS}/Wcf/API25/Application.svc'",
                $"'{ISHDeploymentInternal.BaseUrl}/{ISHDeploymentInternal.WebAppNameWS}/Wcf/API25/Baseline.svc'",
                $"'{ISHDeploymentInternal.BaseUrl}/{ISHDeploymentInternal.WebAppNameWS}/Wcf/API25/DocumentObj.svc'",
                $"'{ISHDeploymentInternal.BaseUrl}/{ISHDeploymentInternal.WebAppNameWS}/Wcf/API25/EDT.svc'",
                $"'{ISHDeploymentInternal.BaseUrl}/{ISHDeploymentInternal.WebAppNameWS}/Wcf/API25/EventMonitor.svc'",
                $"'{ISHDeploymentInternal.BaseUrl}/{ISHDeploymentInternal.WebAppNameWS}/Wcf/API25/Folder.svc'",
                $"'{ISHDeploymentInternal.BaseUrl}/{ISHDeploymentInternal.WebAppNameWS}/Wcf/API25/ListOfValues.svc'",
                $"'{ISHDeploymentInternal.BaseUrl}/{ISHDeploymentInternal.WebAppNameWS}/Wcf/API25/MetadataBinding.svc'",
                $"'{ISHDeploymentInternal.BaseUrl}/{ISHDeploymentInternal.WebAppNameWS}/Wcf/API25/OutputFormat.svc'",
                $"'{ISHDeploymentInternal.BaseUrl}/{ISHDeploymentInternal.WebAppNameWS}/Wcf/API25/PublicationOutput.svc'",
                $"'{ISHDeploymentInternal.BaseUrl}/{ISHDeploymentInternal.WebAppNameWS}/Wcf/API25/Search.svc'",
                $"'{ISHDeploymentInternal.BaseUrl}/{ISHDeploymentInternal.WebAppNameWS}/Wcf/API25/Settings.svc'",
                $"'{ISHDeploymentInternal.BaseUrl}/{ISHDeploymentInternal.WebAppNameWS}/Wcf/API25/TranslationJob.svc'",
                $"'{ISHDeploymentInternal.BaseUrl}/{ISHDeploymentInternal.WebAppNameWS}/Wcf/API25/TranslationTemplate.svc'",
                $"'{ISHDeploymentInternal.BaseUrl}/{ISHDeploymentInternal.WebAppNameWS}/Wcf/API25/User.svc'",
                $"'{ISHDeploymentInternal.BaseUrl}/{ISHDeploymentInternal.WebAppNameWS}/Wcf/API25/UserGroup.svc'",
                $"'{ISHDeploymentInternal.BaseUrl}/{ISHDeploymentInternal.WebAppNameWS}/Wcf/API25/UserRole.svc'",
                $"'{ISHDeploymentInternal.BaseUrl}/{ISHDeploymentInternal.WebAppNameWS}/Wcf/API20/Application.svc'",
                $"'{ISHDeploymentInternal.BaseUrl}/{ISHDeploymentInternal.WebAppNameWS}/Wcf/API20/DocumentObj.svc'",
                $"'{ISHDeploymentInternal.BaseUrl}/{ISHDeploymentInternal.WebAppNameWS}/Wcf/API20/EDT.svc'",
                $"'{ISHDeploymentInternal.BaseUrl}/{ISHDeploymentInternal.WebAppNameWS}/Wcf/API20/Folder.svc'",
                $"'{ISHDeploymentInternal.BaseUrl}/{ISHDeploymentInternal.WebAppNameWS}/Wcf/API20/MetaDataAssist.svc'",
                $"'{ISHDeploymentInternal.BaseUrl}/{ISHDeploymentInternal.WebAppNameWS}/Wcf/API20/OutputFormat.svc'",
                $"'{ISHDeploymentInternal.BaseUrl}/{ISHDeploymentInternal.WebAppNameWS}/Wcf/API20/Publication.svc'",
                $"'{ISHDeploymentInternal.BaseUrl}/{ISHDeploymentInternal.WebAppNameWS}/Wcf/API20/PublicationOutput.svc'",
                $"'{ISHDeploymentInternal.BaseUrl}/{ISHDeploymentInternal.WebAppNameWS}/Wcf/API20/Reports.svc'",
                $"'{ISHDeploymentInternal.BaseUrl}/{ISHDeploymentInternal.WebAppNameWS}/Wcf/API20/Search.svc'",
                $"'{ISHDeploymentInternal.BaseUrl}/{ISHDeploymentInternal.WebAppNameWS}/Wcf/API20/Settings.svc'",
                $"'{ISHDeploymentInternal.BaseUrl}/{ISHDeploymentInternal.WebAppNameWS}/Wcf/API20/Workflow.svc'",
                $"'{ISHDeploymentInternal.BaseUrl}/{ISHDeploymentInternal.WebAppNameWS}/Wcf/API/Application.svc'",
                $"'{ISHDeploymentInternal.BaseUrl}/{ISHDeploymentInternal.WebAppNameWS}/Wcf/API/ConditionManagement.svc'"
            };
        }
    }
}
