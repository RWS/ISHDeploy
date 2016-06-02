using System;
using System.Management.Automation;
using ISHDeploy.Data.Managers;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Interfaces;
using System.Linq;

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
            var cmdLetType = this.GetType().ToString().Split('.').Last();
            Logger.WriteDebug("Commandlet '" + cmdLetType + "' starts.");
            try
            {
                base.ProcessRecord();
                ExecuteCmdlet();
            }
            catch (Exception ex)
            {
                ThrowTerminatingError(new ErrorRecord(ex, string.Empty, ErrorCategory.CloseError, null));
            }
            Logger.WriteDebug("Commandlet '" + cmdLetType + "' finished.");
        }
    }
}
