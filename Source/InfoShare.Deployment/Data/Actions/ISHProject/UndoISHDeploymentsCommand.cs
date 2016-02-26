using System.IO;
using System.Linq;
using InfoShare.Deployment.Data.Managers.Interfaces;
using InfoShare.Deployment.Interfaces;

namespace InfoShare.Deployment.Data.Actions.ISHProjectActions
{
	/// <summary>
	/// Action responcible for rollong back changes to vanilla state
	/// </summary>
    public class UndoISHDeploymentsAction : BaseAction
	{
        private readonly IRegistryManager _registryManager;
        private readonly IXmlConfigManager _xmlConfigManager;
        private readonly IFileManager _fileManager;
        private readonly string _backUpFolder;
        private readonly string _deploymentFolder;


		/// <summary>
		/// Reverts changes to vanilla state
		/// </summary>
		/// <param name="logger">Logger object</param>
		/// <param name="backUpFolder">Folder chich contains backups</param>
		/// <param name="deploymentFolder">Folder at deployment, where to put the reverted changes</param>
		public UndoISHDeploymentsAction(ILogger logger, string deploymentFolder, string backUpFolder)
            : base(logger)
        {
            _registryManager = ObjectFactory.GetInstance<IRegistryManager>();
            _xmlConfigManager = ObjectFactory.GetInstance<IXmlConfigManager>();
            _fileManager = ObjectFactory.GetInstance<IFileManager>();

			_backUpFolder = backUpFolder;
			_deploymentFolder = deploymentFolder;
		}

		/// <summary>
		/// Reverts changes to vanilla state
		/// </summary>
		/// <param name="logger">Logger Object</param>
		/// <param name="deploymentFolder">Folder at deployment, where to put the reverted changes</param>
		public UndoISHDeploymentsAction(ILogger logger, string deploymentFolder)
			: base(logger)
		{
			_registryManager = ObjectFactory.GetInstance<IRegistryManager>();
			_xmlConfigManager = ObjectFactory.GetInstance<IXmlConfigManager>();
			_fileManager = ObjectFactory.GetInstance<IFileManager>();

			_deploymentFolder = deploymentFolder;
		}

		/// <summary>
		/// Executed Undo action. Copies all files from back up folder to folder per deployment
		/// And cleans up the licenses folder as by Vanilla there are no licenses by default
		/// </summary>
		public override void Execute()
	    {
			if (_backUpFolder != null)
			{
				int l = _backUpFolder.Length;
				foreach (
					string backUpFilePath in
						Directory.GetFiles(_backUpFolder, "*", SearchOption.AllDirectories).Select(x => x.Substring(l)))
				{
					// Add per deployment
					_fileManager.Copy(Path.Combine(_backUpFolder, backUpFilePath), Path.Combine(_deploymentFolder, backUpFilePath));
				}
			}
			else
			{
				// Means there is no backup folder needed, and we just have to clean the related folder on deployment
				foreach (string filePath in Directory.GetFiles(_deploymentFolder))
				{
					_fileManager.Delete(filePath);
				}
			}
		}
	}
}
