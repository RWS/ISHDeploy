using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using InfoShare.Deployment.Interfaces;

namespace InfoShare.Deployment.Data.Services
{
    public class XmlConfigManager : IXmlConfigManager
    {
        #region Private constants

        private const string InputConfigParamXmlPath = "inputconfig/param";
        private const string NameXmlAttr = "name";
        private const string CurrentValueXmlNode = "currentvalue";

        #endregion

        private readonly ILogger _logger;
        private readonly IFileManager _fileManager;

        public XmlConfigManager(ILogger logger)
        {
            _logger = logger;
            _fileManager = ObjectFactory.GetInstance<IFileManager>();
        }

        public Dictionary<string, string> GetAllInstallParamsValues(string filePath)
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

        public void Backup()
        {
        }

        public void CommentBlock(string filePath, string searchPattern)
        {
            var doc = _fileManager.Load(filePath);
            
            var startAndEndNodes = doc.DescendantNodes()
                .Where(node => node.NodeType == XmlNodeType.Comment && node.ToString().Contains(searchPattern));

            if (startAndEndNodes.Count() != 2)
            {
                _logger.WriteWarning($"{filePath} does not contain start and end pattern '{searchPattern}' where it's expected.");
                return;
            }

            XNode uncommentedNode = startAndEndNodes.First().NextNode;

            if (uncommentedNode.NodeType == XmlNodeType.Comment)
            {
                _logger.WriteVerbose($"{filePath} contains already commented part within the pattern {searchPattern}");
                return;
            }

            var uncommentText = uncommentedNode.ToString();

            var commentedNode = new XComment(uncommentText);

            uncommentedNode.ReplaceWith(commentedNode);

            _fileManager.Save(filePath, doc);
        }

        public void CommentNode(string filePath, string xpath)
        {
            var doc = _fileManager.Load(filePath);

            //var navigator = doc.Root.CreateNavigator();
            ////var namespaces = navigator.GetNamespacesInScope(XmlNamespaceScope.Local);
            //var namespaceManager = new XmlNamespaceManager(new NameTable());
            ////foreach (var ns in namespaces)
            ////{
            //    namespaceManager.AddNamespace("xmlns", "http://www.xopus.com/xmlns/config");
            ////}

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
        
        public void UncommentBlock(string filePath, string searchPattern)
        {
            var doc = _fileManager.Load(filePath);

            var startAndEndNodes = doc.DescendantNodes()
                .Where(node => node.NodeType == XmlNodeType.Comment && node.ToString().Contains(searchPattern));

            if (startAndEndNodes.Count() != 2)
            {
                _logger.WriteWarning($"{filePath} does not contain start and end pattern '{searchPattern}' where it's expected.");
                return;
            }

            XNode commentedNode = startAndEndNodes.First().NextNode;

            XElement uncommentedNode;
            if (commentedNode != null && commentedNode.NodeType == XmlNodeType.Comment && TryParseCommentedNode(commentedNode, out uncommentedNode))
            {
                commentedNode.ReplaceWith(uncommentedNode);
            }
            else
            {
                _logger.WriteVerbose($"{filePath} dose not contain commented part within the start and end pattern {searchPattern}");
                return;
            }

            _fileManager.Save(filePath, doc);
        }

        public void UncommentNode(string filePath, string searchPattern)
        {
            var doc = _fileManager.Load(filePath);

            var startAndEndNodes = doc.DescendantNodes()
                .Where(node => node.NodeType == XmlNodeType.Comment && node.ToString().Contains(searchPattern));

            if (!startAndEndNodes.Any())
            {
                _logger.WriteWarning($"{filePath} does not contain pattern '{searchPattern}' where it's expected.");
                return;
            }

            XNode commentedNode = startAndEndNodes.FirstOrDefault();

            XElement uncommentedNode;
            if (TryParseCommentedNode(commentedNode, out uncommentedNode))
            {
                commentedNode.ReplaceWith(uncommentedNode);
            }
            else
            {
                _logger.WriteVerbose($"{filePath} could not uncomment {commentedNode}");
                return;
            }

            _fileManager.Save(filePath, doc);
        }

        public void RestoreOriginal()
        {
        }

        private bool TryParseCommentedNode(XNode commentedNode, out XElement uncommentedNode)
        {
            uncommentedNode = null;
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
                uncommentedNode = XElement.Parse(commentText);
                return true;
            }
            catch
            {
                return false;
            }
        }

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
    }
}
