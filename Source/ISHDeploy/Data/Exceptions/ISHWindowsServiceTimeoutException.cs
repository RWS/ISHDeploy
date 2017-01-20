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

namespace ISHDeploy.Data.Exceptions
{
    /// <summary>
    /// The exception that is thrown when ISH windows service doesn't change state .
    /// </summary>
    /// <seealso cref="System.Exception" />
    [Serializable]
    public class ISHWindowsServiceTimeoutException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WrongXPathException"/> class.
        /// </summary>
        /// <param name="serviceName">The ISH windows service name.</param>
        /// <param name="innerException">The inner exception.</param>
        public ISHWindowsServiceTimeoutException(string serviceName, System.ServiceProcess.TimeoutException innerException)
            : base($"Wasn't able to start service '{serviceName}'.", innerException)
        { }
    }
}
