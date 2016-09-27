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
﻿using System;

namespace ISHDeploy.Data.Exceptions
{
    /// <summary>
    /// The exception that is thrown when user wants to enable or disable windows authentication on IIS but WindowsAuthentication module has not been installed
    /// </summary>
    /// <seealso cref="System.Exception" />
    [Serializable]
    public class WindowsAuthenticationModuleIsNotInstalledException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsAuthenticationModuleIsNotInstalledException"/> class.
        /// </summary>
        public WindowsAuthenticationModuleIsNotInstalledException()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsAuthenticationModuleIsNotInstalledException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public WindowsAuthenticationModuleIsNotInstalledException(string message)
            : base(message)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsAuthenticationModuleIsNotInstalledException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="inner">The inner exception.</param>
        public WindowsAuthenticationModuleIsNotInstalledException(string message, Exception inner)
            : base(message, inner)
        { }
    }
}
