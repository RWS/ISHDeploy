using System.Collections.Generic;
using ISHDeploy.Interfaces;

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
        /// Set attribute value
        /// </summary>
        /// <param name="filePath">Path to the file that is modified</param>
        /// <param name="xpath">XPath that is searched</param>
        /// <param name="attributeName">Name of the attribute that will be modified</param>
        /// <param name="value">Attribute new value</param>
        void SetAttributeValue(string filePath, string xpath, string attributeName, string value);

		/// <summary>
		/// Set attribute value
		/// </summary>
		/// <param name="filePath">Path to the file that is modified</param>
		/// <param name="xpath">XPath that is searched</param>
		/// <param name="value">Node fron IshConfiguration.</param>
		void SetNode(string filePath, string xpath, IISHXmlNode value);

        /// <summary>
        /// Inserts a new node before specified one.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="xpath">The xpath to the node before which we want to add a new node.</param>
        /// <param name="xmlString">The new node as a XML string.</param>
        void InsertBeforeNode(string filePath, string xpath, string xmlString);
    }
}