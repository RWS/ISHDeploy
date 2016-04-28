using System;
using ISHDeploy.Business.Invokers;
using ISHDeploy.Data.Actions.XmlFile;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Business.Operations.ISHSTS
{
    /// <summary>
    /// Removes certificate based on a issuer name
    /// </summary>
    /// <seealso cref="IOperation" />
    public class RemoveISHIntegrationSTSCertificateOperation : OperationPaths, IOperation
	{
		/// <summary>
		/// The actions invoker
		/// </summary>
		private readonly IActionInvoker _invoker;

		/// <summary>
		/// Initializes a new instance of the class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="issuer">The certificate issuer.</param>
		public RemoveISHIntegrationSTSCertificateOperation(ILogger logger, string issuer)
		{
			_invoker = new ActionInvoker(logger, "Removing certificate credentials based on issuer name");

			// Author web Config
			_invoker.AddAction(new RemoveNodesAction(logger, InfoShareAuthorWebConfig.Path, 
				String.Format(InfoShareAuthorWebConfig.IdentityTrustedIssuersXPath, issuer)));
        
			// WS web Config
			_invoker.AddAction(new RemoveNodesAction(logger, InfoShareWSWebConfig.Path, 
				String.Format(InfoShareWSWebConfig.IdentityTrustedIssuersPath, issuer)));

            // STS web Config
            _invoker.AddAction(new RemoveNodesAction(logger, InfoShareSTSWebConfig.Path,
				String.Format(InfoShareSTSWebConfig.ServiceBehaviorsTrustedUserXPath, issuer)));
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
