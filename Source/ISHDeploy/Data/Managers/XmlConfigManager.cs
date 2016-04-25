using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Interfaces;
using ISHDeploy.Data.Exceptions;
using System.Text.RegularExpressions;

namespace ISHDeploy.Data.Managers
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
        /// Returns dictionary with all parameters from inputparameters.xml file
        /// </summary>
        /// <param name="filePath">Path to inputparameters.xml file</param>
        /// <returns>Dictionary with parameters</returns>
        public Dictionary<string, string> GetAllInputParamsValues(string filePath)
        {
			_logger.WriteDebug($"Retrieveing input parameters from file `{filePath}`");

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
			_logger.WriteDebug($"Removing single node at `{xpath}` from file `{filePath}`");

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
		public void MoveBeforeNode(string filePath, string xpath, string insertBeforeXpath)
		{
			_logger.WriteDebug($"Moving node at `{xpath}` before `{insertBeforeXpath}` node in file `{filePath}`");

			var doc = _fileManager.Load(filePath);

			var nodes = this.SelectNodes(ref doc, xpath).ToArray();
			if (nodes.Length == 0)
			{
				_logger.WriteVerbose($"{filePath} does not contain nodes within the xpath {xpath}");
				return;
			}

			XNode insertBeforeNode = null;
			if (string.IsNullOrEmpty(insertBeforeXpath))
			{
				// In this case we need to add node to the top of the document
				var parentNode = nodes.First().Parent;
				if (parentNode != null)
				{
					insertBeforeNode = parentNode.FirstNode;
				}
			}
			else
			{
				insertBeforeNode = doc.XPathSelectElement(insertBeforeXpath);
			}

			if (insertBeforeNode == null)
			{
				_logger.WriteVerbose($"{filePath} does not contain target node to insert before");
				return;
			}

			foreach (var nodeToMove in nodes.Reverse())
			{
				if (nodeToMove != insertBeforeNode)
				{
					nodeToMove.Remove();
					insertBeforeNode.AddBeforeSelf(nodeToMove);

					insertBeforeNode = nodeToMove;
				}
			}

			_fileManager.Save(filePath, doc);
		}

        /// <summary>
	    /// Removes node in xml file that can be found by <paramref name="xpath"/>
	    /// </summary>
	    /// <param name="filePath">Path to the file that is modified</param>
	    /// <param name="xpath">XPath to searched node</param>
	    /// <param name="insertAfterXpath">XPath to searched node</param>
	    public void MoveAfterNode(string filePath, string xpath, string insertAfterXpath)
	    {
			_logger.WriteDebug($"Moving node at `{xpath}` after `{insertAfterXpath}` node in file `{filePath}`");

			var doc = _fileManager.Load(filePath);

			var nodes = this.SelectNodes(ref doc, xpath).ToArray();
			if (nodes.Length == 0)
			{
				_logger.WriteVerbose($"{filePath} does not contain nodes within the xpath {xpath}");
				return;
			}

	        XNode insertAfterNode = null;
	        if (string.IsNullOrEmpty(insertAfterXpath))
	        {
				// In this case we need to add node to the bottom of the document
		        var parentNode = nodes.First().Parent;
		        if (parentNode != null)
		        {
			        insertAfterNode = parentNode.LastNode;
		        }
	        }
	        else
	        {
				insertAfterNode = doc.XPathSelectElement(insertAfterXpath);
			}

			if (insertAfterNode == null)
			{
				_logger.WriteVerbose($"{filePath} does not contain target node to insert after");
				return;
			}

			foreach (var nodeToMove in nodes)
			{
				if (nodeToMove != insertAfterNode)
				{
					nodeToMove.Remove();
					insertAfterNode.AddAfterSelf(nodeToMove);

					insertAfterNode = nodeToMove;
				}
			}

			_fileManager.Save(filePath, doc);
		}

		/// <summary>
        /// Comments node in xml file that can be found by <paramref name="xpath"/>
        /// </summary>
        /// <param name="filePath">Path to the file that is modified</param>
        /// <param name="xpath">XPath to searched node</param>
        /// <param name="encodeInnerXml">True if content of the comment should be encoded; otherwise False.</param>
        public void CommentNode(string filePath, string xpath, bool encodeInnerXml = false)
        {
			_logger.WriteDebug($"Commenting {(encodeInnerXml ? "and encoding " : "")}xml node in `{filePath}` xml file that can be found by `{xpath}`");

			var doc = _fileManager.Load(filePath);

            var uncommentedNode = doc.XPathSelectElement(xpath);

            if (uncommentedNode == null)
            {
                _logger.WriteVerbose($"{filePath} does not contain uncommented node within the xpath {xpath}");
                return;
            }

            var uncommentText = uncommentedNode.ToString();

            if (encodeInnerXml)
            {
                uncommentText = XmlEncode(uncommentText);
            }

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
			_logger.WriteDebug($"Commenting all nodes in file `{filePath}` that has {searchPattern} right above it");

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

                var uncommentText = XmlEncode(uncommentedNode.ToString());

                commentedNode = new XComment(uncommentText);

                uncommentedNode.ReplaceWith(commentedNode);
            }

            if (commentedNode != null) // means that file was changed
            {
                _fileManager.Save(filePath, doc);
            }
        }

        /// <summary>
        /// Uncomments all nodes that has <paramref name="searchPattern"/> right above it.
        /// </summary>
        /// <param name="filePath">Path to the file that is modified.</param>
        /// <param name="searchPattern">Comment pattern that precedes the searched node.</param>
        public void UncommentNodesByPrecedingPattern(string filePath, string searchPattern)
        {
			_logger.WriteDebug($"Uncommenting all nodes in file `{filePath}` that has {searchPattern} right above it");

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
        /// <param name="decodeInnerXml">True if content of the comment should be decoded; otherwise False.</param>
        public void UncommentNodesByInnerPattern(string filePath, string searchPattern, bool decodeInnerXml = false)
        {
			_logger.WriteDebug($"Uncommenting {(decodeInnerXml ? "and decoding " : "")}all xml nodes in `{filePath}` xml file that can be found by `{searchPattern}`");

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
                if (!TryUncommentNode(commentedNode, ref doc, decodeInnerXml))
                {
                    throw new WrongXmlStructureException(filePath, $"Was not able to uncomment the following node: {commentedNode}");
                }
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
			_logger.WriteDebug($"Setting node `{xpath}` attribute `{attributeName}` value to `{value}` in file `{filePath}`");

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
		/// Set xml node
		/// </summary>
		/// <param name="filePath">Path to the file that is modified</param>
		/// <param name="xpath">XPath that is searched</param>
		/// <param name="xNode">The xml node from ISH configuration.</param>
		public void SetNode(string filePath, string xpath, IISHXmlNode xNode)
		{
			_logger.WriteDebug($"Setting node `{xpath}` to `{filePath}`");

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

        /// <summary>
        /// Inserts a new node before specified one.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="xpath">The xpath to the node before which we want to add a new node.</param>
        /// <param name="xmlString">The new node as a XML string.</param>
        /// <exception cref="WrongXPathException"></exception>
        public void InsertBeforeNode(string filePath, string xpath, string xmlString)
        {
			_logger.WriteDebug($"Inserting new node before `{xpath}` to file `{filePath}`");

			var doc = _fileManager.Load(filePath);

            var relativeElement = doc.XPathSelectElement(xpath);
            if (relativeElement == null)
            {
                throw new WrongXPathException(filePath, xpath);
            }

            var newElement = XElement.Parse(xmlString);
            XNodeEqualityComparer equalityComparer = new XNodeEqualityComparer();
            foreach (var node in relativeElement.Parent.Nodes())
            {
                if (equalityComparer.Equals(node, newElement))
                {
                    _logger.WriteWarning($"The element with xpath '{xpath}' already contains element '{xmlString}' before it.");
                    return;
                }
            }

            relativeElement.AddBeforeSelf(newElement);

            _fileManager.Save(filePath, doc);
        }

        /// <summary>
        /// Set element value.
        /// </summary>
        /// <param name="filePath">Path to the file that is modified.</param>
        /// <param name="xpath">XPath of searched element.</param>
        /// <param name="value">The new value of element.</param>
        public void SetElementValue(string filePath, string xpath, string value)
        {
            _logger.WriteDebug($"Setting `{xpath}` element new value `{value}` in file `{filePath}`");

            var doc = _fileManager.Load(filePath);

            var element = doc.XPathSelectElement(xpath);

            if (element == null)
            {
                _logger.WriteWarning($"{filePath} does not contain element '{xpath}'.");
                return;
            }
            element.SetValue(value);

            _fileManager.Save(filePath, doc);
        }

        #region private methods

		/// <summary>
		/// Tries to uncomment node.
		/// </summary>
		/// <param name="commentedNode">The commented node.</param>
		/// <param name="doc">The document where changes should take place.</param>
		/// <returns>True if operation succeeded; otherwise False.</returns>
		/// <param name="decodeInnerXml">True if content of the comment should be decoded; otherwise False.</param>
		private bool TryUncommentNode(XNode commentedNode, ref XDocument doc, bool decodeInnerXml = false)
        {
            var commentText = commentedNode.ToString().TrimStart('<').TrimEnd('>');
            var startIndex = commentText.IndexOf('<');
            var endIndex = commentText.LastIndexOf('>');

            if (startIndex < 0 || endIndex < 0)
            {
                return false;
            }
            
            commentText = commentText.Substring(startIndex, endIndex - startIndex + 1);

            if (decodeInnerXml)
            {
                commentText = XmlDecode(commentText);
            }

			try
            {
                // XElement.Replace cannot be used because of possible unknown namespaces inside the comment.
                var commentedDocXmlString = doc.ToString();
                var replacedDocXmlString = commentedDocXmlString.Replace(commentedNode.ToString(), commentText);
                
                doc = XDocument.Parse(replacedDocXmlString);
            }
			catch (XmlException ex)
			{ 
				_logger.WriteDebug($"Replaced content can`t be parsed as XML, with following message {ex.Message}" );
				return false;
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
			return this.SelectNodes(ref doc, xPath).SingleOrDefault();
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

        /// <summary>
        /// Replaces some characters that might cause issues when commenting/uncommenting xml fragment.
        /// </summary>
        /// <param name="text">The string that should be encoded.</param>
        /// <returns>Encoded string.</returns>
        private string XmlEncode(string text)
        {
            return text.Replace(@"\", @"\\").Replace(@"--", @"-\-");
        }

        /// <summary>
        /// Replaces some characters back that where changed by <see cref="XmlEncode"/>.
        /// </summary>
        /// <param name="text">The string that should be decoded.</param>
        /// <returns>Decoded string.</returns>
        private string XmlDecode(string text)
        {
            return text.Replace(@"-\-", @"--").Replace(@"\\", @"\");
        }
    }
}
