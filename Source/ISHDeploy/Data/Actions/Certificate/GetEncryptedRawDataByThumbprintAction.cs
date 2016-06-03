using System;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Data.Actions.Certificate
{
    /// <summary>
    /// Implements file read all text action.
    /// </summary>
    /// <seealso cref="BaseActionWithResult{TResult}" />
    public class GetEncryptedRawDataByThumbprintAction : BaseActionWithResult<string>
    {
        /// <summary>
        /// The certificate manager.
        /// </summary>
        private readonly ICertificateManager _certificateManager;

        /// <summary>
        /// The certificate thumbprint.
        /// </summary>
        private readonly string _thumbprint;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetEncryptedRawDataByThumbprintAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="thumbprint">The certificate thumbprint.</param>
        /// <param name="returnResult">The delegate that returns all text of file.</param>
        public GetEncryptedRawDataByThumbprintAction(ILogger logger, string thumbprint, Action<string> returnResult)
			: base(logger, returnResult)
        {
            _thumbprint = thumbprint;
            _certificateManager = ObjectFactory.GetInstance<ICertificateManager>();
        }

        /// <summary>
        /// Executes current action and returns result.
        /// </summary>
        /// <returns>File content</returns>
        protected override string ExecuteWithResult()
		{
            return _certificateManager.GetEncryptedRawDataByThumbprint(_thumbprint);
        }
	}
}
