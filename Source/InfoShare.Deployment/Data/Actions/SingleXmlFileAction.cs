using InfoShare.Deployment.Data.Managers.Interfaces;
using InfoShare.Deployment.Interfaces;
using InfoShare.Deployment.Models;

namespace InfoShare.Deployment.Data.Actions
{
    public abstract class SingleXmlFileAction : SingleFileAction
	{
		protected readonly IXmlConfigManager XmlConfigManager;

		protected SingleXmlFileAction(ILogger logger, ISHFilePath ishFilePath)
			: base(logger, ishFilePath)
        {
			XmlConfigManager = ObjectFactory.GetInstance<IXmlConfigManager>();
		}
	}
}
