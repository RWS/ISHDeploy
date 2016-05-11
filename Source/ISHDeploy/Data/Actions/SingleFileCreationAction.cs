using System;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Interfaces;
using ISHDeploy.Interfaces.Actions;

namespace ISHDeploy.Data.Actions
{
    /// <summary>
	/// Does single file operations that create new file.
	/// </summary>
    public abstract class SingleFileCreationAction : BaseAction, IRestorableAction
    {
        /// <summary>
        /// The file path.
        /// </summary>
        protected string FilePath;

        /// <summary>
        /// The file manager.
        /// </summary>
        protected readonly IFileManager FileManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleFileCreationAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="filePath">The file path.</param>
        protected SingleFileCreationAction(ILogger logger, string filePath)
            : base(logger)
        {
            FilePath = filePath;
            FileManager = ObjectFactory.GetInstance<IFileManager>();
        }

        /// <summary>
		/// Does nothing for this kind of actions.
		/// </summary>
        public void Backup()
        {
            // for this kind of operation no backup is needed
        }

        /// <summary>
        /// Reverts an asset to initial state.
        /// </summary>
        public void Rollback()
        {
            Logger.WriteDebug($"Reverting changes. Removing file {FilePath}.");
            if (FileManager.FileExists(FilePath))
            {
                FileManager.Delete(FilePath);
                Logger.WriteDebug($"File {FilePath} removed.");
            }
        }
    }
}
