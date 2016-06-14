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
﻿using System;

namespace ISHDeploy.Data.Exceptions
{
    /// <summary>
    /// The exception that is thrown when xml file has wrong and unexpected structure.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class WrongXmlStructureException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WrongXmlStructureException"/> class.
        /// </summary>
        /// <param name="filePath">The xml file path that caused the exception.</param>
        /// <param name="message">The message that describes the error.</param>
        public WrongXmlStructureException(string filePath, string message)
            : this(filePath, message, null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="WrongXmlStructureException"/> class.
        /// </summary>
        /// <param name="filePath">The xml file path that caused the exception.</param>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The inner exception.</param>
        public WrongXmlStructureException(string filePath, string message, Exception innerException)
            : base($"File '{filePath}' has wrong structure. {message}", innerException)
        { }
    }
}
