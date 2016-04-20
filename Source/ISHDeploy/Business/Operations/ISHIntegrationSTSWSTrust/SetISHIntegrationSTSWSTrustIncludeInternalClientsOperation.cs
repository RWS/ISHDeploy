using ISHDeploy.Business.Invokers;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Business.Operations.ISHIntegrationSTSWSTrust
{
    /// <summary>
    /// 
    /// </summary>
    public class SetISHIntegrationSTSWSTrustIncludeInternalClientsOperation : IOperation
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetISHIntegrationSTSWSTrustIncludeInternalClientsOperation"/> class.
        /// </summary>
        public SetISHIntegrationSTSWSTrustIncludeInternalClientsOperation(ILogger logger, ISHPaths paths, string fileName, string fileContent)
        {
        }

        /// <summary>
        /// Runs current operation.
        /// </summary>
        public void Run()
        {

        }
    }
}
