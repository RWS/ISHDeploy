using ISHDeploy.Business.Invokers;
using ISHDeploy.Data.Actions.File;
using ISHDeploy.Interfaces;
using ISHDeploy.Models;

namespace ISHDeploy.Business.Operations.ISHContentEditor
{
    /// <summary>
    /// Sets new license for Content Editor
    /// </summary>
    public class SetISHContentEditorOperation : IOperation
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetISHContentEditorOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="licenceFolderPath">The destination path for licence file.</param>
        /// <param name="fileName">Name of the file that will be created.</param>
        /// <param name="fileContent">Content of the new file.</param>
        public SetISHContentEditorOperation(ILogger logger, ISHFilePath licenceFolderPath, string fileName, string fileContent)
        {
            _invoker = new ActionInvoker(logger, "Setting of new license for Content Editor");

            _invoker.AddAction(new FileCreateAction(logger, licenceFolderPath, fileName, fileContent));
        }

        /// <summary>
        /// Runs current operation.
        /// </summary>
        public void Run()
        {
            _invoker.Invoke();
        }
    }
}