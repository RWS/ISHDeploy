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
using ISHDeploy.Common;
using ISHDeploy.Common.Interfaces;
using ISHDeploy.Data.Managers;
using ISHDeploy.Data.Managers.Interfaces;
using System;
using System.Collections;
using System.Management.Automation;

namespace ISHDeploy.Cmdlets
{
    /// <summary>
    /// Provides base functionality for all cmdlets
    /// </summary>
    public abstract class BaseCmdlet : PSCmdlet
    {
        /// <summary>
        /// Logger
        /// </summary>
        public readonly ILogger Logger;

        /// <summary>
        /// Constructor
        /// </summary>
        protected BaseCmdlet()
        {
            Logger = CmdletsLogger.Instance();
            CmdletsLogger.Initialize(this);
            ObjectFactory.SetInstance<IFileManager>(new FileManager(Logger));
            ObjectFactory.SetInstance<IXmlConfigManager>(new XmlConfigManager(Logger));
            ObjectFactory.SetInstance<ITextConfigManager>(new TextConfigManager(Logger));
            ObjectFactory.SetInstance<IRegistryManager>(new RegistryManager(Logger));
            ObjectFactory.SetInstance<ICertificateManager>(new CertificateManager(Logger));
            ObjectFactory.SetInstance<ITemplateManager>(new TemplateManager(Logger));
            ObjectFactory.SetInstance<IWebAdministrationManager>(new WebAdministrationManager(Logger));
            ObjectFactory.SetInstance<IDataAggregateHelper>(new DataAggregateHelper(Logger));
            ObjectFactory.SetInstance<IWindowsServiceManager>(new WindowsServiceManager(Logger));
        }

        /// <summary>
        /// Will provide functionality - ForEach-Object  in powershell
        /// </summary>
        /// <param name="obj">object for output</param>
        protected void ISHWriteOutput(object obj)
        {
            var enumerable = obj as IEnumerable;

            if (enumerable != null)
            {
                WriteObject(obj, true);
            }
            else
            {
                WriteObject(obj);
            }
        }

        /// <summary>
        /// Method to be overridden instead of process record
        /// </summary>
        public abstract void ExecuteCmdlet();

        /// <summary>
        /// Overrides ProcessRecord from base Cmdlet class
        /// </summary>
        protected override void ProcessRecord()
        {
            try
            {
                base.ProcessRecord();
                ExecuteCmdlet();
            }
            catch (Exception ex)
            {
                ThrowTerminatingError(new ErrorRecord(ex, string.Empty, ErrorCategory.CloseError, null));
            }

        }

        /// <summary>
        /// Overrides BeginProcessing from base Cmdlet class with additinal debug information
        /// </summary>
        protected override void BeginProcessing()
        {
            Logger.WriteDebug("starts.");
            base.BeginProcessing();
        }

        /// <summary>
        /// Overrides EndProcessing from base Cmdlet class with additinal debug information
        /// </summary>
        protected override void EndProcessing()
        {
            Logger.WriteDebug("finished.");
            base.EndProcessing();
        }

    }
}
