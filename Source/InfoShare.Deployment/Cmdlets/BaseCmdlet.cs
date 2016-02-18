using System;
using System.Management.Automation;
using InfoShare.Deployment.Data.Managers;
using InfoShare.Deployment.Data.Managers.Interfaces;
using InfoShare.Deployment.Interfaces;

namespace InfoShare.Deployment.Cmdlets
{
    public abstract class BaseCmdlet : Cmdlet
    {
        public readonly ILogger Logger;
        protected BaseCmdlet()
        {
            Logger = CmdletsLogger.Instance();
            ObjectFactory.SetInstance<IFileManager>(new FileManager(Logger));
            ObjectFactory.SetInstance<IXmlConfigManager>(new XmlConfigManager(Logger));
            ObjectFactory.SetInstance<IRegistryManager>(new RegistryManager(Logger));
        }

        public abstract void ExecuteCmdlet();

        protected override void ProcessRecord()
        {
            try
            {
                CmdletsLogger.Initialize(this);
                base.ProcessRecord();

                ExecuteCmdlet();
            }
            catch (Exception ex)
            {
                ThrowTerminatingError(new ErrorRecord(ex, string.Empty, ErrorCategory.CloseError, null));
            }
        }
    }
}
