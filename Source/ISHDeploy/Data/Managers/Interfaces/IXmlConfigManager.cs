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
﻿using System.Collections.Generic;
﻿using ISHDeploy.Data.Exceptions;
﻿using ISHDeploy.Interfaces;

namespace ISHDeploy.Data.Managers.Interfaces
{
    /// <summary>
    /// Performs different kinds of operations with xml file
    /// </summary>
    public interface IXmlConfigManager
    {
        /// <summary>
        /// Returns dictionary with all parameters fron inputparameters.xml file
        /// </summary>
        /// <param name="filePath">Path to inputparameters.xml file</param>
        /// <returns>Dictionary with parameters</returns>
        Dictionary<string, string> GetAllInputParamsValues(string filePath);

        /// <summary>
		/// Removes node from xml file that can be found by <paramref name="xpath"/>
		/// </summary>
		/// <param name="filePath">Path to the file that is modified</param>
		/// <param name="xpath">XPath to searched node</param>
		void RemoveSingleNode(string filePath, string xpath);

        /// <summary>
        /// Removes nodes in xml file that can be found by <paramref name="xpath"/>
        /// </summary>
        /// <param name="filePath">Path to the file that is modified</param>
        /// <param name="xpath">XPath to searched nodes</param>
        void RemoveNodes(string filePath, string xpath);

        /// <summary>
        /// Removes node from xml file that can be found by <paramref name="xpath"/>
        /// </summary>
        /// <param name="filePath">Path to the file that is modified</param>
        /// <param name="xpath">XPath to searched node</param>
        /// <param name="insertBeforeXpath">XPath to searched node</param>
        void MoveBeforeNode(string filePath, string xpath, string insertBeforeXpath = null);

		/// <summary>
		/// Removes node from xml file that can be found by <paramref name="xpath"/>
		/// </summary>
		/// <param name="filePath">Path to the file that is modified</param>
		/// <param name="xpath">XPath to searched node</param>
		/// <param name="insertAfterXpath">XPath to searched node</param>
		void MoveAfterNode(string filePath, string xpath, string insertAfterXpath = null);

		/// <summary>
		/// Comments node in xml file that can be found by <paramref name="xpath"/>
        /// Comments node in xml file that can be found by <paramref name="xpath" />
        /// </summary>
        /// <param name="filePath">Path to the file that is modified</param>
        /// <param name="xpath">XPath to searched node</param>
        /// <param name="encodeInnerXml">True if content of the comment should be encoded; otherwise False.</param>
        void CommentNode(string filePath, string xpath, bool encodeInnerXml = false);

        /// <summary>
        /// Comments all nodes that has <paramref name="searchPattern"/> right above it
        /// </summary>
        /// <param name="filePath">Path to the file that is modified</param>
        /// <param name="searchPattern">Comment pattern that precedes the searched node</param>
        void CommentNodesByPrecedingPattern(string filePath, string searchPattern);
        
        /// <summary>
        /// Comments all nodes that has <paramref name="searchPattern"/> right above it
        /// </summary>
        /// <param name="filePath">Path to the file that is modified</param>
        /// <param name="searchPattern">Comment pattern that precedes the searched node</param>
        void UncommentNodesByPrecedingPattern(string filePath, string searchPattern);
        
        /// <summary>
        /// Uncomment multiple nodes that can be found by inner pattern
        /// </summary>
        /// <param name="filePath">Path to the file that is modified</param>
        /// <param name="searchPattern">Comment pattern that is inside commented node</param>
        /// <param name="decodeInnerXml">True if content of the comment should be decoded; otherwise False.</param>
        void UncommentNodesByInnerPattern(string filePath, string searchPattern, bool decodeInnerXml = false);
        
        /// <summary>
		/// Set attribute value by attribute xPath
        /// </summary>
        /// <param name="filePath">Path to the file that is modified</param>
		/// <param name="attributeXpath">XPath the attribute that will be modified</param>
        /// <param name="value">Attribute new value</param>
		void SetAttributeValue(string filePath, string attributeXpath, string value);

		/// <summary>
		/// Set attribute value
		/// </summary>
		/// <param name="filePath">Path to the file that is modified</param>
		/// <param name="xpath">XPath that is searched</param>
		/// <param name="value">Node fron IshConfiguration.</param>
		/// <param name="replaceIfExists">if set to <c>true</c> replaces existing node if exists, otherwise does nothing.</param>
		void SetNode(string filePath, string xpath, IISHXmlNode value, bool replaceIfExists = true);

        /// <summary>
        /// Inserts a new node before specified one.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="xpath">The xpath to the node before which we want to add a new node.</param>
        /// <param name="xmlString">The new node as a XML string.</param>
        /// <exception cref="WrongXPathException"></exception>
        void InsertBeforeNode(string filePath, string xpath, string xmlString);

        /// <summary>
        /// Set element value.
        /// </summary>
        /// <param name="filePath">Path to the file that is modified.</param>
        /// <param name="xpath">XPath of searched element.</param>
        /// <param name="value">The new value of element.</param>
        void SetElementValue(string filePath, string xpath, string value);

        /// <summary>
        /// Gets the value from element found by xpath.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="xpath">The xpath to the element.</param>
        /// <returns>The element value.</returns>
        string GetValue(string filePath, string xpath);
    }
}