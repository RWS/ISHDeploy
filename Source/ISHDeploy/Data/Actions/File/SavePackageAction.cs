using System.Collections.Generic;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Data.Actions.File
{
    /// <summary>
    /// Saves files to package.
    /// </summary>
    /// <seealso cref="SingleFileCreationAction" />
    public class SavePackageAction : SingleFileCreationAction
    {
        /// <summary>
        /// Files to pack.
        /// </summary>
        private readonly IEnumerable<string> _filesToPack;

        /// <summary>
        /// Identifies if original files should be removed after they are packed.
        /// </summary>
        private readonly bool _clearArtifacts;

        /// <summary>
        /// Initializes a new instance of the <see cref="SavePackageAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="filesToPack">The files to pack.</param>
        /// <param name="clearArtifacts">True if original files should be removed after they are packed; otherwise false.</param>
        public SavePackageAction(ILogger logger, string filePath, IEnumerable<string> filesToPack, bool clearArtifacts)
            : base(logger, filePath)
        {
            _filesToPack = filesToPack;
            _clearArtifacts = clearArtifacts;
        }

        /// <summary>
        /// Executes current action.
        /// </summary>
        public override void Execute()
        {
            _fileManager.PackageFiles(_filePath, _filesToPack);

            if (_clearArtifacts)
            {
                foreach (var file in _filesToPack)
                {
                    _fileManager.Delete(file);
                }
            }
        }
    }
}
