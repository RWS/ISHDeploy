using System.Collections.Generic;

namespace InfoShare.Deployment.Data.Managers.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IXmlConfigManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        Dictionary<string, string> GetAllInstallParamsValues(string filePath);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="searchPattern"></param>
        void CommentBlock(string filePath, string searchPattern);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="xpath"></param>
        void CommentNode(string filePath, string xpath);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="xpath"></param>
        /// <param name="internalPatternElem"></param>
        /// <returns></returns>
        void CommentNodeWithInternalPattern(string filePath, string xpath, string internalPatternElem);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="searchPattern"></param>
        void UncommentBlock(string filePath, string searchPattern);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="searchPattern"></param>
        void UncommentNode(string filePath, string searchPattern);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="xpath"></param>
        /// <returns></returns>
        bool XPathExists(string filePath, string xpath);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="xpath"></param>
        /// <param name="attributeName"></param>
        /// <param name="value"></param>
        void SetAttributeValue(string filePath, string xpath, string attributeName, string value);
    }
}