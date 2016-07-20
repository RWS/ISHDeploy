/**
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
﻿
namespace ISHDeploy.Data.Managers.Interfaces
{
    /// <summary>
    /// Operates on registered certificates.
    /// </summary>
    public interface ICertificateManager
    {
        /// <summary>
        /// Gets the certificate public key.
        /// </summary>
        /// <param name="thumbprint">The certificate thumbprint.</param>
        /// <returns>Certificate public key.</returns>
        string GetCertificatePublicKey(string thumbprint);

        /// <summary>
        /// Gets the encrypted raw data by thumbprint.
        /// </summary>
        /// <param name="thumbprint">The thumbprint.</param>
        /// <returns></returns>
        string GetEncryptedRawDataByThumbprint(string thumbprint);

        /// <summary>
        /// Gets the path to certificate by thumbprint
        /// </summary>
        /// <param name="thumbprint">The thumbprint.</param>
        /// <returns></returns>
        string GetPathToCertificateByThumbprint(string thumbprint);
    }
}
