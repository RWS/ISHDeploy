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
using ISHDeploy.Common.Interfaces;
using ISHDeploy.Data.Managers.Interfaces;
using NetFwTypeLib;

namespace ISHDeploy.Data.Managers
{
    /// <summary>
    /// Implements network management.
    /// </summary>
    /// <seealso cref="INetworkManager" />
    public class NetworkManager : INetworkManager
    {

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkManager"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public NetworkManager(ILogger logger)
        {
            _logger = logger;
        }
        
        /// <summary>
        /// Opens port
        /// </summary>
        /// <param name="port">The number of port.</param>
        public void GloballyOpenPort(int port)
        {
            _logger.WriteDebug("Open port", port);

            var netFwMgr = (INetFwMgr)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwMgr"));

            var netFwOpenPort = (INetFwOpenPort)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWOpenPort"));

            // Get the current profile
            var profile = netFwMgr.LocalPolicy.CurrentProfile;

            // Set the port properties
            netFwOpenPort.Scope = NET_FW_SCOPE_.NET_FW_SCOPE_ALL;
            netFwOpenPort.Enabled = true;
            netFwOpenPort.Protocol = NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP;
            netFwOpenPort.Name = "ISHDeploy port opening";
            netFwOpenPort.Port = port;

            // Add the port to the ICF Permissions List
            profile.GloballyOpenPorts.Add(netFwOpenPort);

            _logger.WriteVerbose($"The port `{port}` has been opened");
        }
    }
}
