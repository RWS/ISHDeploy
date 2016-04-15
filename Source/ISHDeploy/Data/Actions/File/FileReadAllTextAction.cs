using System;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Data.Actions.File
{
    /// <summary>
    /// Implements file read all text action.
    /// </summary>
    /// <seealso cref="BaseActionWithResult{TResult}" />
    public class FileReadAllTextAction : BaseActionWithResult<string>
    {
        /// <summary>
        /// The file path.
        /// </summary>
        private readonly string _filePath;

        /// <summary>
        /// The file manager.
        /// </summary>
        private readonly IFileManager _fileManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileReadAllTextAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="returnResult">The delegate that returns if license is found for host name.</param>
        public FileReadAllTextAction(ILogger logger, string filePath, Action<string> returnResult)
			: base(logger, returnResult)
        {
            _filePath = filePath;

            _fileManager = ObjectFactory.GetInstance<IFileManager>();
        }

        /// <summary>
        /// Executes current action and returns result.
        /// </summary>
        /// <returns>File content</returns>
        protected override string ExecuteWithResult()
		{
            if (!_fileManager.FileExists(_filePath))
            {
				Logger.WriteDebug($"File `{_filePath}` was not found.");
				return string.Empty;
            }

            return _fileManager.ReadAllText(_filePath);
        }
	}
}
