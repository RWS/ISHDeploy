using System;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Interfaces;
using ISHDeploy.Models;

namespace ISHDeploy.Data.Actions.File
{
    /// <summary>
	/// Action that waits until file becomes unlocked.
    /// </summary>
    /// <seealso cref="BaseAction" />
    public class FileWaitUnlockAction : BaseAction
    {
        /// <summary>
        /// The file manager.
        /// </summary>
        private readonly IFileManager _fileManager;

        /// <summary>
        /// The path to the file.
        /// </summary>
        private readonly string _filePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCreateAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="filePath">The path to the file.</param>
        public FileWaitUnlockAction(ILogger logger, ISHFilePath filePath) 
			: base(logger)
        {
            _filePath = filePath.AbsolutePath;

            _fileManager = ObjectFactory.GetInstance<IFileManager>();
        }

        /// <summary>
        /// Executes current action.
        /// </summary>
        public override void Execute()
		{
            int i = 0;
            while (_fileManager.IsFileLocked(_filePath))
            {
                System.Threading.Thread.Sleep(100);
                i++;

                if (i > 100)
                {
                    throw new TimeoutException($"The process cannot access the file {_filePath} for a long timebecause. It is being used by another process");
                }
            }
        }
        
	}
}
