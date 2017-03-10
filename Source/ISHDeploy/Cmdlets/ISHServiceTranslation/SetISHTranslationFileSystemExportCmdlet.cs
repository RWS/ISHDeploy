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
using ISHDeploy.Business.Operations.ISHServiceTranslation;
using ISHDeploy.Common.Models.TranslationOrganizer;

namespace ISHDeploy.Cmdlets.ISHServiceTranslation
{
    /// <summary>
    /// <para type="synopsis">Sets configuration of FileSystem.</para>
    /// <para type="description">The Set-ISHTranslationFileSystemExport cmdlet sets configuration of FileSystem.</para>
    /// <para type="link">Set-ISHServiceTranslationOrganizer</para>
    /// <para type="link">Remove-ISHTranslationFileSystemExport</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\>Set-ISHTranslationFileSystemExport -ISHDeployment $deployment -Name "FileSystem1" -ExportFolderPath "c:\ExportFolder" -MaximumJobSize 5242880</code>
    /// <para>This command enables the translation organizer windows service.
    /// Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    [Cmdlet(VerbsCommon.Set, "ISHTranslationFileSystemExport")]
    public sealed class SetISHTranslationFileSystemExportCmdlet : BaseHistoryEntryCmdlet
    {
        /// <summary>
        /// <para type="description">The name (alias) of FileSystem.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "The name (alias) of FileSystem")]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        /// <summary>
        /// <para type="description">The path to export folder.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "The path to export folder")]
        [ValidateNotNullOrEmpty]
        public string ExportFolderPath { get; set; }

        /// <summary>
        /// <para type="description">The max value of total size in bytes of uncompressed external job.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "The max value of total size in bytes of uncompressed external job")]
        [ValidateNotNullOrEmpty]
        public int MaximumJobSize { get; set; }

        /// <summary>
        /// <para type="description">The requested metadata.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "The requested metadata.")]
        public ISHFieldMetadata[] RequestedMetadata { get; set; }

        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            var configuration = new FileSystemConfigurationSection(
                Name,
                MaximumJobSize,
                ExportFolderPath,
                RequestedMetadata);

            var operation = new SetISHTranslationFileSystemExportOperation(
                Logger, 
                ISHDeployment,
                configuration,
                "TranslationOrganizer.exe.config already contains settings for FileSystem. You should remove FileSystem configuration section first. To do this you can use Remove-ISHTranslationFileSystemExport cmdlet.");

            operation.Run();
        }
    }
}
