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

using System;
using System.IO;
using System.Linq;
using ISHDeploy.Common;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Common.Interfaces;
using ISHDeploy.Common.Models;
using Models = ISHDeploy.Common.Models;

namespace ISHDeploy.Business.Operations
{
    /// <summary>
    /// Provides xpaths, search patterns and constants of deployment files
    /// </summary>
    public abstract partial class BaseOperationPaths
    {
        /// <summary>
        /// The logger.
        /// </summary>
        protected ILogger Logger;

        /// <summary>
        /// Input parameters
        /// </summary>
        protected InputParameters InputParameters { get; }

        #region Paths

        /// <summary>
        /// Gets the path to the Web+Suffix Author folder.
        /// </summary>
        protected string WebFolderPath { get; }

        /// <summary>
        /// Gets the path to ~\Web\Author\ASP\bin folder.
        /// </summary>
        protected string AuthorAspBinFolderPath { get; }

        /// <summary>
        /// Gets the path to ~\Web\Author\ASP\Custom folder.
        /// </summary>
        protected string AuthorAspCustomFolderPath { get; }

        /// <summary>
        /// Gets the path to the App+Suffix Author folder.
        /// </summary>
        protected string AppFolderPath { get; }

        /// <summary>
        /// Gets the path to the Data+Suffix Author folder.
        /// </summary>
        protected string DataFolderPath { get; }

        /// <summary>
        /// Gets the path to the InfoShareSTS folder.
        /// </summary>
        protected string WebNameSTS { get; }

        /// <summary>
        /// Gets the path to the InfoShareSTS Application Data folder.
        /// </summary>
        protected string WebNameSTSAppData { get; }

        /// <summary>
        /// The path to generated History.ps1 file
        /// </summary>
        protected string HistoryFilePath { get; }

        /// <summary>
        /// The path to C:\ProgramData\ISHDeploy.X.X.X folder
        /// </summary>
        protected string ISHDeploymentProgramDataFolderPath { get; }

        /// <summary>
        /// Url for STS path
        /// </summary>
        protected string InternalSTSLoginUrlSTS
        {
            get
            {
                return InputParameters.BaseUrl + "/" + InputParameters.WebAppNameSTS + "/";
            }
        }
        
        /// <summary>
        /// Url for CM path
        /// </summary>
        protected string InternalSTSLoginUrlCM
        {
            get
            {
                return InputParameters.BaseUrl + "/" + InputParameters.WebAppNameCM + "/";
            }
        }

        /// <summary>
        /// Url for WS path with new folder
        /// </summary>
        protected string InternalSTSLoginUrlWSWithNewFolder
        {
            get
            {
                return InputParameters.BaseUrl + "/" + InputParameters.WebAppNameWS + "/" + InternalSTSLogin.TargetFolderName;
            }
        }
        
        /// <summary>
        /// Url for WS path with new folder
        /// </summary>
        protected string InternalSTSSouceFolder
        {
            get
            {
                return Path.Combine(WebFolderPath, "InfoShareWS");
            }
        }

        /// <summary>
        /// File which will be copied
        /// </summary>
        protected string InternalSTSSourceConnectionConfigurationFile
        {
            get
            {
                return InternalSTSSouceFolder + InternalSTSLogin.FileToCopy;
            }
        }

        /// <summary>
        /// Create path for new derectory for 2 files
        /// </summary>
        protected ISHFilePath InternalSTSFilelPath
        {
            get
            {
                return new ISHFilePath(InternalSTSSouceFolder, BackupWebFolderPath, InternalSTSLogin.TargetFolderName);
            }
        }

        /// <summary>
        /// New connection file path
        /// </summary>
        protected ISHFilePath InternalSTSNewConnectionConfigPath
        {
            get
            {
                return new ISHFilePath(InternalSTSFolderToChange, BackupWebFolderPath, InternalSTSFileToChange);
            }
        }

        /// <summary>
        /// Folder will be created
        /// </summary>
        protected string InternalSTSFolderToChange
        {
            get
            {
                return Path.Combine(InternalSTSSouceFolder, InternalSTSLogin.TargetFolderName);
            }
        }

        /// <summary>
        /// XML file will be changed in new directory
        /// </summary>
        protected string InternalSTSFileToChange
        {
            get
            {
                return InternalSTSFolderToChange + InternalSTSLogin.FileToCopy;
            }
        }

        /// <summary>
        /// The path to ~\Web\Author\ASP\Tree.htm
        /// </summary>
        protected ISHFilePath AuthorASPTreeHtmPath { get; }

        /// <summary>
        /// The path to ~\Web\Author\ASP\XSL\EventMonitorMenuBar.xml
        /// </summary>
        protected ISHFilePath EventMonitorMenuBarXmlPath { get; }

        /// <summary>
        /// The path to ~\Data\PublishingService\Tools\FeedSDLLiveContent.ps1.config
        /// </summary>
        protected ISHFilePath FeedSDLLiveContentConfigPath { get; }

        /// <summary>
        /// The path to ~\Web\Author\ASP\XSL\FolderButtonbar.xml
        /// </summary>
        protected ISHFilePath FolderButtonBarXmlPath { get; }

        /// <summary>
        /// The path to packages folder location for deployment
        /// </summary>
        protected string PackagesFolderPath { get; }

        /// <summary>
        /// The UNC path to packages folder
        /// </summary>
        protected string PackagesFolderUNCPath { get; }

        /// <summary>
        /// The path to back up folder location for deployment
        /// </summary>
        protected string BackupFolderPath { get; }

        /// <summary>
        /// The path to file with list of vanilla files in ~\Web\Author\ASP\bin folder.
        /// </summary>
        protected string ListOfVanillaFilesOfWebAuthorAspBinFolderFilePath { get; }

        /// <summary>
        /// The path to Web back up folder
        /// </summary>
        protected string BackupWebFolderPath { get; }

        /// <summary>
        /// The path to Data back up folder
        /// </summary>
        protected string BackupDataFolderPath { get; }

        /// <summary>
        /// The path to App back up folder
        /// </summary>
        protected string BackupAppFolderPath { get; }

        /// <summary>
        /// The path to ~\Web\Author\ASP\Editors\Xopus\license\ folder
        /// </summary>
        protected ISHFilePath LicenceFolderPath { get; }

        /// <summary>
        /// The path to ~\Web\Author\ASP\XSL\InboxButtonBar.xml
        /// </summary>
        protected ISHFilePath InboxButtonBarXmlPath { get; }

        /// <summary>
        /// The path to ~\Web\Author\ASP\Web.config
        /// </summary>
        protected ISHFilePath InfoShareAuthorWebConfigPath { get; }

        /// <summary>
        /// The path to ~\Web\InfoShareSTS\Configuration\infoShareSTS.config.
        /// </summary>
        protected ISHFilePath InfoShareSTSConfigPath { get; }

        /// <summary>
        /// The path to ~\Web\InfoShareSTS\App_Data\IdentityServerConfiguration-2.2.sdf
        /// </summary>
        protected ISHFilePath InfoShareSTSDataBasePath { get; }

        /// <summary>
        /// Gets the connection string to ~\Web\InfoShareSTS\App_Data\IdentityServerConfiguration-2.2.sdf.
        /// </summary>
        /// <value>
        /// The connection string.
        /// </value>
        protected string InfoShareSTSDataBaseConnectionString { get; }

        /// <summary>
        /// The path to ~\Web\InfoShareSTS\Web.config
        /// </summary>
        protected ISHFilePath InfoShareSTSWebConfigPath { get; }

        /// <summary>
        /// The path to ~\Web\InfoShareWS\connectionconfiguration.xml
        /// </summary>
        protected ISHFilePath InfoShareWSConnectionConfigPath { get; }

        /// <summary>
        /// The path to ~\Web\InfoShareWS\Web.config
        /// </summary>
        protected ISHFilePath InfoShareWSWebConfigPath { get; }

        /// <summary>
        /// The path to inputparameters.xml
        /// </summary>
        protected ISHFilePath InputParametersFilePath { get; }

        /// <summary>
        /// The path to ~\Web\Author\ASP\XSL\LanguageDocumentButtonbar.xml
        /// </summary>
        protected ISHFilePath LanguageDocumentButtonbarXmlPath { get; }

        /// <summary>
        /// The path to ~\App\Utilities\SynchronizeToLiveContent\SynchronizeToLiveContent.ps1.config
        /// </summary>
        protected ISHFilePath SynchronizeToLiveContentConfigPath { get; }

        /// <summary>
        /// The path to ~\Web\Author\ASP\XSL\TopDocumentButtonbar.xml
        /// </summary>
        protected ISHFilePath TopDocumentButtonBarXmlPath { get; }

        /// <summary>
        /// The path to ~\App\TranslationOrganizer\Bin\TranslationOrganizer.exe.config
        /// </summary>
        protected ISHFilePath TranslationOrganizerConfigPath { get; }

        /// <summary>
        /// The path to ~\Web\Author\ASP\Trisoft.InfoShare.Client.config
        /// </summary>
        protected ISHFilePath TrisoftInfoShareClientConfigPath { get; }

        /// <summary>
        /// The path to ~\Web\Author\ASP\Editors\Xopus\config\bluelion-config.xml
        /// </summary>
        protected ISHFilePath XopusBluelionConfigXmlPath { get; }

        /// <summary>
        /// The path to ~\Web\Author\ASP\Editors\Xopus\BlueLion-Plugin\web.config
        /// </summary>
        protected ISHFilePath XopusBlueLionPluginWebCconfigPath { get; }

        /// <summary>
        /// The path to ~\Web\Author\ASP\Editors\Xopus\config\config.xml
        /// </summary>
        protected ISHFilePath XopusConfigXmlPath { get; }

        /// <summary>
        /// Gets the path to the ~\Author\ASP\UI folder.
        /// </summary>
        protected string AuthorAspUIFolderPath { get; }

        /// <summary>
        /// Gets the path to ~\Web\Author\ASP\UI\Extensions\_config.xml
        /// </summary>
        protected ISHFilePath CUIFConfigFilePath { get; }


        /// <summary>
        /// Gets the path to ~\App\TranslationBuilder\Bin\TranslationBuilder.exe.config
        /// </summary>
        protected ISHFilePath TranslationBuilderConfigFilePath { get; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseOperationPaths"/> class.
        /// </summary>
        /// <param name="ishDeployment">The ish deployment.</param>
        /// <param name="logger"></param>
        protected BaseOperationPaths(ILogger logger, Models.ISHDeployment ishDeployment)
        {
            Logger = logger;

            var dataAggregateHelper = ObjectFactory.GetInstance<IDataAggregateHelper>();
            InputParameters = dataAggregateHelper.GetInputParameters(ishDeployment.Name);

            #region Paths
            var programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            var moduleName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            ISHDeploymentProgramDataFolderPath = Path.Combine(programData, moduleName, ishDeployment.Name);
            PackagesFolderPath = Path.Combine(ISHDeploymentProgramDataFolderPath, "Packages");
            PackagesFolderUNCPath = ConvertLocalFolderPathToUNCPath(PackagesFolderPath);
            BackupFolderPath = Path.Combine(ISHDeploymentProgramDataFolderPath, "Backup");
            BackupWebFolderPath = Path.Combine(BackupFolderPath, "Web");
            BackupAppFolderPath = Path.Combine(BackupFolderPath, "App");
            BackupDataFolderPath = Path.Combine(BackupFolderPath, "Data");
            ListOfVanillaFilesOfWebAuthorAspBinFolderFilePath = Path.Combine(BackupFolderPath, "vanilla.web.author.asp.bin.xml");
            WebFolderPath = Path.Combine(InputParameters.WebPath, $"Web{InputParameters.ProjectSuffix}");
            AuthorAspUIFolderPath = Path.Combine(WebFolderPath, @"Author\ASP\UI");
            AuthorAspBinFolderPath = Path.Combine(WebFolderPath, @"Author\ASP\bin");
            AuthorAspCustomFolderPath = Path.Combine(WebFolderPath, @"Author\ASP\Custom");
            AppFolderPath = Path.Combine(InputParameters.AppPath, $"App{InputParameters.ProjectSuffix}");
            DataFolderPath = Path.Combine(InputParameters.DataPath, $"Data{InputParameters.ProjectSuffix}");
            WebNameSTS = Path.Combine(WebFolderPath, "InfoShareSTS");
            WebNameSTSAppData = Path.Combine(WebNameSTS, "App_Data");
            InputParametersFilePath = new ISHFilePath(InputParameters.FilePath.Replace("inputparameters.xml", string.Empty), BackupFolderPath, "inputparameters.xml");
            HistoryFilePath = Path.Combine(ISHDeploymentProgramDataFolderPath, "History.ps1");
            AuthorASPTreeHtmPath = new ISHFilePath(WebFolderPath, BackupWebFolderPath, @"Author\ASP\Tree.htm");
            EventMonitorMenuBarXmlPath = new ISHFilePath(WebFolderPath, BackupWebFolderPath, @"Author\ASP\XSL\EventMonitorMenuBar.xml");
            FeedSDLLiveContentConfigPath = new ISHFilePath(DataFolderPath, BackupDataFolderPath, @"PublishingService\Tools\FeedSDLLiveContent.ps1.config");
            FolderButtonBarXmlPath = new ISHFilePath(WebFolderPath, BackupWebFolderPath, @"Author\ASP\XSL\FolderButtonbar.xml");
            LicenceFolderPath = new ISHFilePath(WebFolderPath, BackupWebFolderPath, @"Author\ASP\Editors\Xopus\license\");
            InboxButtonBarXmlPath = new ISHFilePath(WebFolderPath, BackupWebFolderPath, @"Author\ASP\XSL\InboxButtonBar.xml");
            InfoShareAuthorWebConfigPath = new ISHFilePath(WebFolderPath, BackupWebFolderPath, @"Author\ASP\Web.config");
            InfoShareSTSConfigPath = new ISHFilePath(WebFolderPath, BackupWebFolderPath, @"InfoShareSTS\Configuration\infoShareSTS.config");
            InfoShareSTSDataBasePath = new ISHFilePath(WebFolderPath, BackupWebFolderPath, @"InfoShareSTS\App_Data\IdentityServerConfiguration-2.2.sdf");
            InfoShareSTSDataBaseConnectionString = $"Data Source = {InfoShareSTSDataBasePath.AbsolutePath}";
            InfoShareSTSWebConfigPath = new ISHFilePath(WebFolderPath, BackupWebFolderPath, @"InfoShareSTS\Web.config");
            InfoShareWSConnectionConfigPath = new ISHFilePath(WebFolderPath, BackupWebFolderPath, @"InfoShareWS\connectionconfiguration.xml");
            InfoShareWSWebConfigPath = new ISHFilePath(WebFolderPath, BackupWebFolderPath, @"InfoShareWS\Web.config");
            LanguageDocumentButtonbarXmlPath = new ISHFilePath(WebFolderPath, BackupWebFolderPath, @"Author\ASP\XSL\LanguageDocumentButtonbar.xml");
            SynchronizeToLiveContentConfigPath = new ISHFilePath(AppFolderPath, BackupAppFolderPath, @"Utilities\SynchronizeToLiveContent\SynchronizeToLiveContent.ps1.config");
            TopDocumentButtonBarXmlPath = new ISHFilePath(WebFolderPath, BackupWebFolderPath, @"Author\ASP\XSL\TopDocumentButtonbar.xml");
            TranslationOrganizerConfigPath = new ISHFilePath(AppFolderPath, BackupAppFolderPath, @"TranslationOrganizer\Bin\TranslationOrganizer.exe.config");
            TrisoftInfoShareClientConfigPath = new ISHFilePath(WebFolderPath, BackupWebFolderPath, @"Author\ASP\Trisoft.InfoShare.Client.config");
            XopusBluelionConfigXmlPath = new ISHFilePath(WebFolderPath, BackupWebFolderPath, @"Author\ASP\Editors\Xopus\config\bluelion-config.xml");
            XopusBlueLionPluginWebCconfigPath = new ISHFilePath(WebFolderPath, BackupWebFolderPath, @"Author\ASP\Editors\Xopus\BlueLion-Plugin\web.config");
            XopusConfigXmlPath = new ISHFilePath(WebFolderPath, BackupWebFolderPath, @"Author\ASP\Editors\Xopus\config\config.xml");
            CUIFConfigFilePath = new ISHFilePath(WebFolderPath, BackupWebFolderPath, @"Author\ASP\UI\Extensions\_config.xml");
            TranslationBuilderConfigFilePath = new ISHFilePath(AppFolderPath, BackupAppFolderPath, @"TranslationBuilder\Bin\TranslationBuilder.exe.config");

            #endregion
        }

        /// <summary>
        /// Gets the normalized thumbprint.
        /// </summary>
        /// <param name="thumbprint">The thumbprint.</param>
        /// <returns>Normalized thumbprint</returns>
        protected string GetNormalizedThumbprint(string thumbprint)
        {
            var normalizedThumbprint = new string(thumbprint.ToCharArray().Where(char.IsLetterOrDigit).ToArray());

            if (normalizedThumbprint.Length != thumbprint.Length)
            {
                Logger.WriteWarning($"The thumbprint '{thumbprint}' has been normalized to '{normalizedThumbprint}'");
            }

            return normalizedThumbprint;
        }

        /// <summary>
        /// Converts the local folder path to UNC path.
        /// </summary>
        /// <param name="localPath">The local path.</param>
        /// <returns>Path to folder in UTC format</returns>
        private string ConvertLocalFolderPathToUNCPath(string localPath)
        {
            return $@"\\{System.Net.Dns.GetHostName()}\{localPath.Replace(":", "$")}";
        }
    }
}
