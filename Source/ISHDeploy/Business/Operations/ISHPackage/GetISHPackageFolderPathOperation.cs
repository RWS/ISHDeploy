using ISHDeploy.Business.Invokers;
using ISHDeploy.Data.Actions.Directory;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Business.Operations.ISHPackage
{
    /// <summary>
    /// Gets the path to the packages folder
    /// </summary>
    /// <seealso cref="BasePathsOperation" />
    /// <seealso cref="IOperation" />
    public class GetISHPackageFolderPathOperation : BasePathsOperation, IOperation<string>
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// Return path in UNC format
        /// </summary>
        private bool _isUNCFormat;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetISHPackageFolderPathOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        public GetISHPackageFolderPathOperation(ILogger logger, Models.ISHDeployment ishDeployment, bool isUNCFormat = false) :
            base(logger, ishDeployment)
        {
            _isUNCFormat = isUNCFormat;

            _invoker = new ActionInvoker(logger, "Getting the path to the packages folder");

            _invoker.AddAction(new DirectoryEnsureExistsAction(logger, FoldersPaths.PackagesFolderPath));
        }

        /// <summary>
        /// Runs current operation.
        /// </summary>
        public string Run()
        {
            _invoker.Invoke();

            return _isUNCFormat ? FoldersPaths.PackagesFolderUNCPath : FoldersPaths.PackagesFolderPath; ;
        }
    }
}
