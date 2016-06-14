using ISHDeploy.Business.Invokers;
using ISHDeploy.Data.Actions.License;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Business.Operations.ISHContentEditor
{
    /// <summary>
    /// Tests if license for specific host name exists
    /// </summary>
    /// <seealso cref="IOperation{TResult}" />
    public class TestISHContentEditorOperation : BasePathsOperation, IOperation<bool>
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// The operation result.
        /// </summary>
        private bool _isLicenceValid = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestISHContentEditorOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="domain">The host name is checked for the existence of the license file.</param>
        public TestISHContentEditorOperation(ILogger logger, Models.ISHDeployment ishDeployment, string domain) :
            base(logger, ishDeployment)
        {
            _invoker = new ActionInvoker(logger, "Testing of license for specific host name");
            _invoker.AddAction(new LicenseTestAction(logger, FoldersPaths.LicenceFolderPath, domain, isValid => { _isLicenceValid = isValid; }));
        }

        /// <summary>
        /// Runs current operation.
        /// </summary>
        /// <returns>List of installed Content Manager deployments.</returns>
        public bool Run()
        {
            _invoker.Invoke();

            return _isLicenceValid;
        }
    }
}