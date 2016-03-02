using System.Collections.Generic;

namespace InfoShare.Deployment.Data.Managers.Interfaces
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
        /// Comments node in xml file that can be found by <paramref name="xpath"/>
        /// </summary>
        /// <param name="filePath">Path to the file that is modified</param>
        /// <param name="xpath">XPath to searched node</param>
        void CommentNode(string filePath, string xpath);

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
        void UncommentNodesByInnerPattern(string filePath, string searchPattern);

        /// <summary>
        /// Removes pattern from node inside and put it right before node
        /// </summary>
        /// <param name="filePath">Path to the file that is modified</param>
        /// <param name="xpath">XPath to the searched node</param>
        /// <param name="internalPatternElem">Internal comment pattern that marks searched node</param>
        void MoveOutInnerPattern(string filePath, string xpath, string internalPatternElem);

        /// <summary>
        /// Set attribute value
        /// </summary>
        /// <param name="filePath">Path to the file that is modified</param>
        /// <param name="xpath">XPath that is searched</param>
        /// <param name="attributeName">Name of the attribute that will be modified</param>
        /// <param name="value">Attribute new value</param>
        void SetAttributeValue(string filePath, string xpath, string attributeName, string value);
    }
}