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

using System.Collections.Generic;

namespace ISHDeploy.Data.Managers.Interfaces
{
    /// <summary>
    /// Implements windows service management.
    /// </summary>
    public interface IPowerShellManager
    {
        /// <summary>
        /// Invokes content of ps1 file as powershell script
        /// </summary>
        /// <param name="embeddedResourceName">Name of embedded ps1 resource with powershell script.</param>
        /// <param name="parameters">Parameters to be passed to the script. Is <c>null</c> by defult</param>
        /// Return the result in the case of casting failure
        /// <returns>An object of specified type that converted from PSObject</returns>
        object InvokeEmbeddedResourceAsScriptWithResult(string embeddedResourceName, Dictionary<string, string> parameters = null);
    }
}
