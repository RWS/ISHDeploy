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
﻿namespace ISHDeploy.Business.Operations
{
    /// <summary>
    /// Provides absolute paths to all ISH files that are going to be used
    /// Also provides xpaths to XML elements and attributes in these files
    /// </summary>
    public partial class BasePathsOperation
    {
        /// <summary>
        /// Temporary names, used to locating SQL templates script for DataBase integration
        /// </summary>
        protected static class TemporaryDBConfigurationFileNames
        {
            /// <summary>
            /// The template for Account Permissions Grant `SQL` script
            /// </summary>
            public const string GrantComputerAccountPermissionsSQLTemplate = "SQL.GrantComputerAccountPermissions.sql";

            /// <summary>
            /// The template for Account Permissions Grant `PS1` script
            /// </summary>
            public const string GrantComputerAccountPermissionsPSTemplate = "SQL.GrantComputerAccountPermissions.ps1";
        }
    }
}
