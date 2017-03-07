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
    /// The exception that document already contains element.
    /// </summary>
    /// <seealso cref="Exception" />
    [Serializable]
    public class DocumentAlreadyContainsElementException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentAlreadyContainsElementException"/> class.
        /// </summary>
        /// <param name="exceptionMessage">The error message.</param>
        public DocumentAlreadyContainsElementException(string exceptionMessage)
            : base(exceptionMessage)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentAlreadyContainsElementException"/> class.
        /// </summary>
        /// <param name="filePath">The ISH windows service name.</param>
        /// <param name="xpath">The inner exception.</param>
        public DocumentAlreadyContainsElementException(string filePath, string xpath)
            : base($"The document {filePath} already contains element with xpath {xpath}.")
        { }
    }
}
