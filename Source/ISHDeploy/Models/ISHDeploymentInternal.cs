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
﻿using System;
using System.IO;

namespace ISHDeploy.Models
{
    /// <summary>
    ///	<para type="description">Represents the installed Content Manager deployment extended.</para>
    /// </summary>
    public class ISHDeploymentInternal : ISHDeployment
    {
        /// <summary>
        /// Gets the all parameters from inputparameter.xml file
        /// </summary>
        /// <value>
        /// The parameters.
        /// </value>
        public InputParameters InputParameters { get; }

        #region File paths

        /// <summary>
        /// The path to generated History.ps1 file
        /// </summary>
        public string HistoryFilePath { get; }

        /// <summary>
        /// The path to C:\ProgramData\ISHDeploy.X.X.X folder
        /// </summary>
        public string ISHDeploymentProgramDataFolderPath { get; }

        /// <summary>
        /// The path to ~\Web\Author\ASP\Tree.htm
        /// </summary>
        public ISHFilePath AuthorASPTreeHtmPath { get; }

        /// <summary>
        /// The path to ~\Web\Author\ASP\XSL\EventMonitorMenuBar.xml
        /// </summary>
        public ISHFilePath EventMonitorMenuBarXmlPath { get; }

        /// <summary>
        /// The path to ~\Data\PublishingService\Tools\FeedSDLLiveContent.ps1.config
        /// </summary>
        public ISHFilePath FeedSDLLiveContentConfigPath { get; }

        /// <summary>
        /// The path to ~\Web\Author\ASP\XSL\FolderButtonbar.xml
        /// </summary>
        public ISHFilePath FolderButtonBarXmlPath { get; }

        /// <summary>
        /// The path to packages folder location for deployment
        /// </summary>
        public string PackagesFolderPath { get; }

        /// <summary>
        /// The UNC path to packages folder
        /// </summary>
        public string PackagesFolderUNCPath { get; }

        /// <summary>
        /// The path to back up folder location for deployment
        /// </summary>
        public string BackupFolderPath { get; }

        /// <summary>
        /// The path to Web back up folder
        /// </summary>
        public string BackupWebFolderPath { get; }

        /// <summary>
        /// The path to Data back up folder
        /// </summary>
        public string BackupDataFolderPath { get; }

        /// <summary>
        /// The path to App back up folder
        /// </summary>
        public string BackupAppFolderPath { get; }


        /// <summary>
        /// The path to ~\Web\Author\ASP\Editors\Xopus\license\ folder
        /// </summary>
        public ISHFilePath LicenceFolderPath { get; }

        /// <summary>
        /// The path to ~\Web\Author\ASP\XSL\InboxButtonBar.xml
        /// </summary>
        public ISHFilePath InboxButtonBarXmlPath { get; }

        /// <summary>
        /// The path to ~\Web\Author\ASP\Web.config
        /// </summary>
        public ISHFilePath InfoShareAuthorWebConfigPath { get; }

        /// <summary>
        /// The path to ~\Web\InfoShareSTS\Configuration\infoShareSTS.config.
        /// </summary>
        public ISHFilePath InfoShareSTSConfigPath { get; }

        /// <summary>
        /// The path to ~\Web\InfoShareSTS\App_Data\IdentityServerConfiguration-2.2.sdf
        /// </summary>
        public ISHFilePath InfoShareSTSDataBasePath { get; }

        /// <summary>
        /// Gets the connection string to ~\Web\InfoShareSTS\App_Data\IdentityServerConfiguration-2.2.sdf.
        /// </summary>
        /// <value>
        /// The connection string.
        /// </value>
        public string InfoShareSTSDataBaseConnectionString { get; }

        /// <summary>
        /// The path to ~\Web\InfoShareSTS\Web.config
        /// </summary>
        public ISHFilePath InfoShareSTSWebConfigPath { get; }

        /// <summary>
        /// The path to ~\Web\InfoShareWS\connectionconfiguration.xml
        /// </summary>
        public ISHFilePath InfoShareWSConnectionConfigPath { get; }

        /// <summary>
        /// The path to ~\Web\InfoShareWS\Web.config
        /// </summary>
        public ISHFilePath InfoShareWSWebConfigPath { get; }

        /// <summary>
        /// The path to inputparameters.xml
        /// </summary>
        public ISHFilePath InputParametersFilePath { get; }

        /// <summary>
        /// The path to ~\Web\Author\ASP\XSL\LanguageDocumentButtonbar.xml
        /// </summary>
        public ISHFilePath LanguageDocumentButtonbarXmlPath { get; }

        /// <summary>
        /// The path to ~\App\Utilities\SynchronizeToLiveContent\SynchronizeToLiveContent.ps1.config
        /// </summary>
        public ISHFilePath SynchronizeToLiveContentConfigPath { get; }

        /// <summary>
        /// The path to ~\Web\Author\ASP\XSL\TopDocumentButtonbar.xml
        /// </summary>
        public ISHFilePath TopDocumentButtonBarXmlPath { get; }

        /// <summary>
        /// The path to ~\App\TranslationOrganizer\Bin\TranslationOrganizer.exe.config
        /// </summary>
        public ISHFilePath TranslationOrganizerConfigPath { get; }

        /// <summary>
        /// The path to ~\Web\Author\ASP\Trisoft.InfoShare.Client.config
        /// </summary>
        public ISHFilePath TrisoftInfoShareClientConfigPath { get; }

        /// <summary>
        /// The path to ~\Web\Author\ASP\Editors\Xopus\config\bluelion-config.xml
        /// </summary>
        public ISHFilePath XopusBluelionConfigXmlPath { get; }

        /// <summary>
        /// The path to ~\Web\Author\ASP\Editors\Xopus\BlueLion-Plugin\web.config
        /// </summary>
        public ISHFilePath XopusBlueLionPluginWebCconfigPath { get;  }

        /// <summary>
        /// The path to ~\Web\Author\ASP\Editors\Xopus\config\config.xml
        /// </summary>
        public ISHFilePath XopusConfigXmlPath { get; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ISHDeploymentInternal"/> class.
        /// </summary>
        /// <param name="inputParametersFilePath">The inputparameters.xml file path.</param>
        /// <param name="parameters">The dictionary with all parameters from inputparameters.xml file.</param>
        /// <param name="softwareVersion">The deployment version.</param>
        public ISHDeploymentInternal(string inputParametersFilePath, InputParameters parameters, Version softwareVersion) 
            : base(parameters, softwareVersion)
        {
            InputParameters = parameters;
            #region File paths
            var programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            var moduleName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            ISHDeploymentProgramDataFolderPath = Path.Combine(programData, moduleName, Name);
            PackagesFolderPath = Path.Combine(ISHDeploymentProgramDataFolderPath, "Packages");
            PackagesFolderUNCPath = ConvertLocalFolderPathToUNCPath(PackagesFolderPath);
            BackupFolderPath = Path.Combine(ISHDeploymentProgramDataFolderPath, "Backup");
            BackupWebFolderPath = Path.Combine(BackupFolderPath, "Web");
            BackupAppFolderPath = Path.Combine(BackupFolderPath, "App");
            BackupDataFolderPath = Path.Combine(BackupFolderPath, "Data");
            InputParametersFilePath = new ISHFilePath(inputParametersFilePath, BackupFolderPath, "inputparameters.xml");
            HistoryFilePath = Path.Combine(ISHDeploymentProgramDataFolderPath, "History.ps1");
            AuthorASPTreeHtmPath = new ISHFilePath(InputParameters.AuthorFolderPath, BackupWebFolderPath, @"Author\ASP\Tree.htm");
            EventMonitorMenuBarXmlPath = new ISHFilePath(InputParameters.AuthorFolderPath, BackupWebFolderPath, @"Author\ASP\XSL\EventMonitorMenuBar.xml");
            FeedSDLLiveContentConfigPath = new ISHFilePath(InputParameters.DataFolderPath, BackupWebFolderPath, @"PublishingService\Tools\FeedSDLLiveContent.ps1.config");
            FolderButtonBarXmlPath = new ISHFilePath(InputParameters.AuthorFolderPath, BackupWebFolderPath, @"Author\ASP\XSL\FolderButtonbar.xml");
            LicenceFolderPath = new ISHFilePath(InputParameters.AuthorFolderPath, BackupWebFolderPath, @"Author\ASP\Editors\Xopus\license\");
            InboxButtonBarXmlPath = new ISHFilePath(InputParameters.AuthorFolderPath, BackupWebFolderPath, @"Author\ASP\XSL\InboxButtonBar.xml");
            InfoShareAuthorWebConfigPath = new ISHFilePath(InputParameters.AuthorFolderPath, BackupWebFolderPath, @"Author\ASP\Web.config");
            InfoShareSTSConfigPath = new ISHFilePath(InputParameters.AuthorFolderPath, BackupWebFolderPath, @"InfoShareSTS\Configuration\infoShareSTS.config");
            InfoShareSTSDataBasePath = new ISHFilePath(InputParameters.AuthorFolderPath, BackupWebFolderPath, @"InfoShareSTS\App_Data\IdentityServerConfiguration-2.2.sdf");
            InfoShareSTSDataBaseConnectionString = $"Data Source = {InfoShareSTSDataBasePath.AbsolutePath}";
            InfoShareSTSWebConfigPath = new ISHFilePath(InputParameters.AuthorFolderPath, BackupWebFolderPath, @"InfoShareSTS\Web.config");
            InfoShareWSConnectionConfigPath = new ISHFilePath(InputParameters.AuthorFolderPath, BackupWebFolderPath, @"InfoShareWS\connectionconfiguration.xml");
            InfoShareWSWebConfigPath = new ISHFilePath(InputParameters.AuthorFolderPath, BackupWebFolderPath, @"InfoShareWS\Web.config");
            LanguageDocumentButtonbarXmlPath = new ISHFilePath(InputParameters.AuthorFolderPath, BackupWebFolderPath, @"Author\ASP\XSL\LanguageDocumentButtonbar.xml");
            SynchronizeToLiveContentConfigPath = new ISHFilePath(InputParameters.AppFolderPath, BackupAppFolderPath, @"Utilities\SynchronizeToLiveContent\SynchronizeToLiveContent.ps1.config");
            TopDocumentButtonBarXmlPath = new ISHFilePath(InputParameters.AuthorFolderPath, BackupWebFolderPath, @"Author\ASP\XSL\TopDocumentButtonbar.xml");
            TranslationOrganizerConfigPath = new ISHFilePath(InputParameters.AppFolderPath, BackupAppFolderPath, @"TranslationOrganizer\Bin\TranslationOrganizer.exe.config");
            TrisoftInfoShareClientConfigPath = new ISHFilePath(InputParameters.AuthorFolderPath, BackupWebFolderPath, @"Author\ASP\Trisoft.InfoShare.Client.config");
            XopusBluelionConfigXmlPath = new ISHFilePath(InputParameters.AuthorFolderPath, BackupWebFolderPath, @"Author\ASP\Editors\Xopus\config\bluelion-config.xml");
            XopusBlueLionPluginWebCconfigPath = new ISHFilePath(InputParameters.AuthorFolderPath, BackupWebFolderPath, @"Author\ASP\Editors\Xopus\BlueLion-Plugin\web.config");
            XopusConfigXmlPath = new ISHFilePath(InputParameters.AuthorFolderPath, BackupWebFolderPath, @"Author\ASP\Editors\Xopus\config\config.xml");

            #endregion
        }


        /// <summary>
        /// Converts the local folder path to UNC path.
        /// </summary>
        /// <param name="localPath">The local path.</param>
        /// <returns>Path to folder in UTC format</returns>
        private string ConvertLocalFolderPathToUNCPath(string localPath)
        {
            return $@"\\{Environment.MachineName}\{localPath.Replace(":", "$")}";
        }
    }
}
