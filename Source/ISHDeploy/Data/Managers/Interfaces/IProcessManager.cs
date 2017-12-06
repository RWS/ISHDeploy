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

namespace ISHDeploy.Data.Managers.Interfaces
{
    /// <summary>
    /// Implements processes management.
    /// </summary>
    public interface IProcessManager
    {
        /// <summary>
        /// Starts executive file with parameters
        /// </summary>
        /// <param name="filePath">The path to executive file.</param>
        /// <param name="arguments">The arguments</param>
        void Start(string filePath, string arguments = "");
        /// <summary>
        /// Kills a process if it has the given id and name
        /// </summary>
        /// <param name="processID">Process Id</param>
        /// <param name="processName">Process Name</param>
        void Kill(int processID, string processName);
    }
}
