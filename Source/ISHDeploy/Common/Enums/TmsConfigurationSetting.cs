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
namespace ISHDeploy.Common.Enums
{
    /// <summary>
    /// Names of attributes of element "/configuration/trisoft.infoShare.translationOrganizer/tms/instances/add" in file ~\App\TranslationOrganizer\Bin\TranslationOrganizer.exe.config
    /// </summary>
    public enum TmsConfigurationSetting
    {
        /// <summary>
        /// The external job max total uncompressed size
        /// </summary>
        externalJobMaxTotalUncompressedSizeBytes,

        /// <summary>
        /// The number of retries on timeout
        /// </summary>
        retriesOnTimeout,

        /// <summary>
        /// The destination port number
        /// </summary>
        destinationPortNumber,

        /// <summary>
        /// The location of Isapi filter
        /// </summary>
        isapiFilterLocation,

        /// <summary>
        /// Use compression
        /// </summary>
        useCompression,

        /// <summary>
        /// Use SSL
        /// </summary>
        useSsl,

        /// <summary>
        /// Use default proxy credentials
        /// </summary>
        useDefaultProxyCredentials,

        /// <summary>
        /// The proxy server
        /// </summary>
        proxyServer,

        /// <summary>
        /// The port number of proxy server
        /// </summary>
        proxyPort
    }
}
