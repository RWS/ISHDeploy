using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Data.Actions.File
{
	/// <summary>
	/// Implements file delete action.
	/// </summary>
    public class FileDeleteAction : BaseAction
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
        /// Initializes a new instance of the <see cref="FileDeleteAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="filePath">The file path.</param>
        public FileDeleteAction(ILogger logger, string filePath) 
			: base(logger)
        {
            _filePath = filePath;

            _fileManager = ObjectFactory.GetInstance<IFileManager>();
        }

		/// <summary>
		/// Executes the action.
		/// </summary>
		public override void Execute()
		{
            if (_fileManager.FileExists(_filePath))
            {
                _fileManager.Delete(_filePath);
            }
		}
	}
}
