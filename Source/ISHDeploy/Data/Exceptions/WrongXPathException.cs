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
    /// The exception that is thrown when XPath should return a result but does not return anything.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class WrongXPathException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WrongXPathException"/> class.
        /// </summary>
        /// <param name="filePath">The xml file path that caused the exception.</param>
        /// <param name="xpath">The xpath to the element.</param>
        public WrongXPathException(string filePath, string xpath)
            : this(filePath, xpath, null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="WrongXPathException"/> class.
        /// </summary>
        /// <param name="filePath">The xml file path that caused the exception.</param>
        /// <param name="xpath">The xpath to the element.</param>
        /// <param name="innerException">The inner exception.</param>
        public WrongXPathException(string filePath, string xpath, Exception innerException)
            : base($"File '{filePath}' does not contain nodes within the xpath: {xpath}.", innerException)
        { }
    }
}
