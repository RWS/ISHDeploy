/*
 * Copyright (c) 2014 All Rights Reserved by the SDL Group.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using ISHDeploy.Common;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Common.Interfaces;

namespace ISHDeploy.Data.Actions.Certificate
{
    /// <summary>
    /// Gets path to certificate file by thumbprint.
    /// </summary>
    /// <seealso cref="BaseActionWithResult{TResult}" />
    public class GetPathToCertificateByThumbprintAction : BaseActionWithResult<string>
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
        public GetPathToCertificateByThumbprintAction(ILogger logger, string thumbprint, Action<string> returnResult)
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
            return _certificateManager.GetPathToCertificateByThumbprint(_thumbprint);
        }
	}
}
