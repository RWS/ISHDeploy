﻿using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using InfoShare.Deployment.Data.Managers.Interfaces;
using InfoShare.Deployment.Interfaces;
using InfoShare.Deployment.Data.Exceptions;

namespace InfoShare.Deployment.Data.Managers
{
    /// <summary>
    /// Performs different kinds of operations with xml file
    /// </summary>
    /// <seealso cref="IXmlConfigManager" />
    public class XmlConfigManager : IXmlConfigManager
    {
        #region Private constants

        /// <summary>
        /// The input configuration parameter xml path
        /// </summary>
        private const string InputConfigParamXmlPath = "inputconfig/param";

        /// <summary>
        /// The name xml attribute
        /// </summary>
        private const string NameXmlAttr = "name";

        /// <summary>
        /// The current value xml node
        /// </summary>
        private const string CurrentValueXmlNode = "currentvalue";

        #endregion

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// The file manager
        /// </summary>
        private readonly IFileManager _fileManager;

        /// <summary>
        /// Returns new instance of the <see cref="XmlConfigManager"/>
        /// </summary>
        /// <param name="logger">Instance of the <see cref="ILogger"/></param>
        public XmlConfigManager(ILogger logger)
        {
            _logger = logger;
            _fileManager = ObjectFactory.GetInstance<IFileManager>();
        }

        /// <summary>
        /// Returns dictionary with all parameters fron inputparameters.xml file
        /// </summary>
        /// <param name="filePath">Path to inputparameters.xml file</param>
        /// <returns>Dictionary with parameters</returns>
        public Dictionary<string, string> GetAllInputParamsValues(string filePath)
        {
            var doc = _fileManager.Load(filePath);
            var dictionary = new Dictionary<string, string>();

            var paramElements = doc.XPathSelectElements(InputConfigParamXmlPath);

            foreach (var paramElement in paramElements)
            {
                var name = paramElement.Attribute(XName.Get(NameXmlAttr)).Value;
                var currentValue = paramElement.XPathSelectElement(CurrentValueXmlNode).Value;

                dictionary.Add(name, currentValue);
            }

            return dictionary;
        }
        
        /// <summary>
        /// Removes single node or comment in xml file that can be found by <paramref name="xpath"/>
        /// </summary>
        /// <param name="filePath">Path to the file that is modified</param>
        /// <param name="xpath">XPath to searched node</param>
        public void RemoveSingleNode(string filePath, string xpath)
        {
            var doc = _fileManager.Load(filePath);

	        var node = this.SelectSingleNode(ref doc, xpath);
			if (node == null)
            {
                _logger.WriteVerbose($"{filePath} does not contain node within the xpath {xpath}");
                return;
            }

			node.Remove();

            _fileManager.Save(filePath, doc);
        }

		/// <summary>
		/// Removes node in xml file that can be found by <paramref name="xpath"/>
		/// </summary>
		/// <param name="filePath">Path to the file that is modified</param>
		/// <param name="xpath">XPath to searched node</param>
		/// <param name="insertBeforeXpath">XPath to searched node</param>
		public void InsertBeforeNode(string filePath, string xpath, string insertBeforeXpath)
		{
			var doc = _fileManager.Load(filePath);

			var nodes = this.SelectNodes(ref doc, xpath).ToArray();
			if (nodes.Length == 0)
			{
				_logger.WriteVerbose($"{filePath} does not contain nodes within the xpath {xpath}");
				return;
			}

			var node2 = !string.IsNullOrEmpty(insertBeforeXpath) ? doc.XPathSelectElement(insertBeforeXpath) : nodes.First().Parent.FirstNode;
			if (node2 == null)
			{
				_logger.WriteVerbose($"{filePath} does not contain node to insert after");
				return;
			}

			foreach (var nodeToMove in nodes)
			{
				nodeToMove.Remove();
				node2.AddBeforeSelf(nodeToMove);
			}

			_fileManager.Save(filePath, doc);
		}

	    /// <summary>
	    /// Removes node in xml file that can be found by <paramref name="xpath"/>
	    /// </summary>
	    /// <param name="filePath">Path to the file that is modified</param>
	    /// <param name="xpath">XPath to searched node</param>
	    /// <param name="insertAfterXpath">XPath to searched node</param>
	    public void InsertAfterNode(string filePath, string xpath, string insertAfterXpath)
	    {
		    var doc = _fileManager.Load(filePath);

			var nodes = this.SelectNodes(ref doc, xpath).ToArray();
			if (nodes.Length == 0)
			{
				_logger.WriteVerbose($"{filePath} does not contain nodes within the xpath {xpath}");
				return;
			}

			var node2 = !string.IsNullOrEmpty(insertAfterXpath) ? doc.XPathSelectElement(insertAfterXpath) : nodes.First().Parent.LastNode;
			if (node2 == null)
			{
				_logger.WriteVerbose($"{filePath} does not contain node to insert after");
				return;
			}

			foreach (var nodeToMove in nodes.Reverse())
			{
				nodeToMove.Remove();
				node2.AddAfterSelf(nodeToMove);
			}

			_fileManager.Save(filePath, doc);
		}

		/// <summary>
		/// Comments node in xml file that can be found by <paramref name="xpath"/>
		/// </summary>
		/// <param name="filePath">Path to the file that is modified</param>
		/// <param name="xpath">XPath to searched node</param>
		public void CommentNode(string filePath, string xpath)
        {
            var doc = _fileManager.Load(filePath);

            var uncommentedNode = doc.XPathSelectElement(xpath);

            if (uncommentedNode == null)
            {
                _logger.WriteVerbose($"{filePath} does not contain uncommented node within the xpath {xpath}");
                return;
            }

            var uncommentText = uncommentedNode.ToString();

            var commentedNode = new XComment(uncommentText);

            uncommentedNode.ReplaceWith(commentedNode);

            _fileManager.Save(filePath, doc);
        }

        /// <summary>
        /// Comments all nodes that has <paramref name="searchPattern"/> right above it
        /// </summary>
        /// <param name="filePath">Path to the file that is modified</param>
        /// <param name="searchPattern">Comment pattern that precedes the searched node</param>
        public void CommentNodesByPrecedingPattern(string filePath, string searchPattern)
        {
            var doc = _fileManager.Load(filePath);

            var patternNodes = doc.DescendantNodes()
                .Where(node => node.NodeType == XmlNodeType.Comment && node.ToString().Contains(searchPattern)).ToArray();

            if (!patternNodes.Any())
            {
                throw new WrongXmlStructureException(filePath, $"No searched patterns '{searchPattern}' were found.");
            }

            XComment commentedNode = null;

            foreach (var patternNode in patternNodes)
            {
                var uncommentedNode = patternNode.NextNode;

                if (uncommentedNode == null)
                {
                    throw new WrongXmlStructureException(filePath, $"Comment pattern has no following nodes after searched comment '{searchPattern}'");
                }

                if (uncommentedNode.NodeType == XmlNodeType.Comment)
                {
                    _logger.WriteVerbose($"{filePath} contains already commented node following after pattern {searchPattern}");
                    continue;
                }

                var uncommentText = uncommentedNode.ToString();

                commentedNode = new XComment(uncommentText);

                uncommentedNode.ReplaceWith(commentedNode);
            }

            if (commentedNode != null) // means that file was changed
            {
                _fileManager.Save(filePath, doc);
            }
        }

        /// <summary>
        /// Comments all nodes that has <paramref name="searchPattern"/> right above it
        /// </summary>
        /// <param name="filePath">Path to the file that is modified</param>
        /// <param name="searchPattern">Comment pattern that precedes the searched node</param>
        public void UncommentNodesByPrecedingPattern(string filePath, string searchPattern)
        {
            var doc = _fileManager.Load(filePath);

            var patternNodes = doc.DescendantNodes()
                .Where(node => node.NodeType == XmlNodeType.Comment && node.ToString().Contains(searchPattern)).ToArray();

            if (!patternNodes.Any())
            {
                throw new WrongXmlStructureException(filePath, $"No searched patterns '{searchPattern}' were found.");
            }

            var isFileChanged = false;

            foreach (var patternNode in patternNodes)
            {
                XNode commentedNode = patternNode.NextNode;

                if (commentedNode == null)
                {
                    throw new WrongXmlStructureException(filePath, $"Comment pattern has no following nodes after searched comment '{searchPattern}'");
                }

                if (commentedNode.NodeType != XmlNodeType.Comment)
                {
                    _logger.WriteVerbose($"{filePath} contains already uncommented node following after pattern {searchPattern}");
                    continue;
                }

                if (!TryUncommentNode(commentedNode, ref doc))
                {
                    throw new WrongXmlStructureException(filePath, $"Was not able to uncomment the following node: {commentedNode}");
                }
                isFileChanged = true;
            }

            if (isFileChanged)
            {
                _fileManager.Save(filePath, doc);
            }
        }

        /// <summary>
        /// Uncomment multiple nodes that can be found by inner pattern
        /// </summary>
        /// <param name="filePath">Path to the file that is modified</param>
        /// <param name="searchPattern">Comment pattern that is inside commented node</param>
        public void UncommentNodesByInnerPattern(string filePath, string searchPattern)
        {
            var doc = _fileManager.Load(filePath);

            var searchedNodes = doc.DescendantNodes()
                .Where(node => node.ToString().Contains(searchPattern)).ToArray();

            if (!searchedNodes.Any())
            {
                throw new WrongXmlStructureException(filePath, $"No searched patterns '{searchPattern}' were found.");
            }

            var commentedNodes = searchedNodes.Where(node => node.NodeType == XmlNodeType.Comment).ToArray();

            if (!commentedNodes.Any())
            {
                _logger.WriteVerbose($"{filePath} contains already uncommented node by searched pattern '{searchPattern}'.");
                return;
            }

            foreach (var commentedNode in commentedNodes)
            {
                if (!TryUncommentNode(commentedNode, ref doc))
                {
                    throw new WrongXmlStructureException(filePath, $"Was not able to uncomment the following node: {commentedNode}");
                }
            }

            _fileManager.Save(filePath, doc);
        }

        /// <summary>
        /// Moves comment node outside of it's parent node found by xpath
        /// </summary>
        /// <param name="filePath">Path to the file that is modified</param>
        /// <param name="xpath">XPath to the searched node</param>
        /// <param name="internalPatternElem">Internal comment pattern that marks searched node</param>
        public void MoveOutsideInnerComment(string filePath, string xpath, string internalPatternElem)
        {
            var doc = _fileManager.Load(filePath);

            var searchedXPath = $"{xpath}[comment()[contains(., '{internalPatternElem}')]]";
            var uncommentedNodes = doc.XPathSelectElements(searchedXPath).ToArray();

            if (!uncommentedNodes.Any())
            {
                // should not throw exception
                _logger.WriteDebug($"{filePath} does not contain inner patterns any more. Searched xpath: {searchedXPath}");
                return;
            }

            foreach (var uncommentedNode in uncommentedNodes)
            {
                // Move internal searched pattern before node
                var internalComment =
                    uncommentedNode.DescendantNodes()
                        .FirstOrDefault(node => node.NodeType == XmlNodeType.Comment && node.ToString().Contains(internalPatternElem));

                uncommentedNode.AddBeforeSelf(internalComment);

                internalComment?.Remove();
            }

            _fileManager.Save(filePath, doc);
        }

        /// <summary>
        /// Set attribute value
        /// </summary>
        /// <param name="filePath">Path to the file that is modified</param>
        /// <param name="xpath">XPath that is searched</param>
        /// <param name="attributeName">Name of the attribute that will be modified</param>
        /// <param name="value">Attribute new value</param>
        public void SetAttributeValue(string filePath, string xpath, string attributeName, string value)
        {
            var doc = _fileManager.Load(filePath);

            var element = doc.XPathSelectElement(xpath);

            if (element == null)
            {
                _logger.WriteWarning($"{filePath} does not contain element '{xpath}'.");
                return;
            }
            element.SetAttributeValue(attributeName, value);

            _fileManager.Save(filePath, doc);
        }

		/// <summary>
		/// Set attribute value
		/// </summary>
		/// <param name="filePath">Path to the file that is modified</param>
		/// <param name="xpath">XPath that is searched</param>
		/// <param name="xNode">The xml node from ISH configuration.</param>
		public void SetNode(string filePath, string xpath, IISHXmlNode xNode)
		{
			var doc = _fileManager.Load(filePath);
			XNode newNode = xNode.ToXElement();
			var existingElement = doc.XPathSelectElement(xpath);
			if (existingElement == null)
			{
				// We need to take the parent node
				var parentXPath = Regex.Replace(xpath, @"\/([^\/])*$", "");
				var parentNode = doc.XPathSelectElement(parentXPath);
				if (parentNode == null)
				{
					throw new WrongXmlStructureException(filePath, $"There are no parent node for XPath '{xpath}' were found.");
				}

				parentNode.Add(newNode);
			}
			else
			{
				existingElement.ReplaceWith(newNode);
			}

			// Check if node does not have a comment
			if (newNode.PreviousNode.NodeType != XmlNodeType.Comment)
			{
				var comment = xNode.GetNodeComment();
				if (comment != null)
				{
					newNode.AddBeforeSelf(comment);
				}
			}

			_fileManager.Save(filePath, doc);
		}		
		
		#region private methods

		/// <summary>
		/// Tries to uncomment node.
		/// </summary>
		/// <param name="commentedNode">The commented node.</param>
		/// <param name="doc">The document where changes should take place.</param>
		/// <returns>True if operation succeeded; otherwise False.</returns>
		private bool TryUncommentNode(XNode commentedNode, ref XDocument doc)
        {
            var commentText = commentedNode.ToString().TrimStart('<').TrimEnd('>');
            var startIndex = commentText.IndexOf('<');
            var endIndex = commentText.LastIndexOf('>');

            if (startIndex < 0 || endIndex < 0)
            {
                return false;
            }
            
            commentText = commentText.Substring(startIndex, endIndex - startIndex + 1);

            try
            {
                // We cannnot use XNode.Replace method to replace just single node, because it cannot resolve namespaces inside uncommented node
                var commentedDocXmlString = doc.ToString();
                var replacedDocXmlString = commentedDocXmlString.Replace(commentedNode.ToString(), commentText);
                doc = XDocument.Parse(replacedDocXmlString);
            }
            catch
            {
                return false;
            }

            return true;
        }

		/// <summary>
		/// Evaluates node from XPath
		/// </summary>
		/// <param name="doc">The document to node lookup.</param>
		/// <param name="xPath">The xPath of node to be evaluated.</param>
		/// <returns>
		/// XNode instance from document.
		/// </returns>
		private XNode SelectSingleNode(ref XDocument doc, string xPath)
		{
			return this.SelectNodes(ref doc, xPath).Single();
		}

		/// <summary>
		/// Evaluates nodes from XPath
		/// </summary>
		/// <param name="doc">The document to node lookup.</param>
		/// <param name="xPath">The xPath of node to be evaluated.</param>
		/// <returns>
		/// IEnumerable of XNodes from document.
		/// </returns>
		private IEnumerable<XNode> SelectNodes(ref XDocument doc, string xPath)
		{
			return ((IEnumerable<object>)doc.XPathEvaluate(xPath)).OfType<XNode>();
		}

		#endregion
	}
}
