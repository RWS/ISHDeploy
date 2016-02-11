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

            var paramElements = doc.XPathSelectElements("inputconfig/param");

            foreach (var paramElement in paramElements)
            {
                var name = paramElement.Attribute(XName.Get("name")).Value;
                var currentValue = paramElement.XPathSelectElement("currentvalue").Value;

                dictionary.Add(name, currentValue);
        }

            return dictionary;
        }

        public void CommentNode(string filePath, string commentPattern)
        {
            var doc = _fileManager.Load(filePath);
            
            var startAndEndNodes = doc.DescendantNodes()
                .Where(node => node.NodeType == XmlNodeType.Comment && node.ToString().Contains(commentPattern));

            if (startAndEndNodes == null || startAndEndNodes.Count() != 2)
            {
                _logger.WriteWarning($"{filePath} does not contains start or end pattern {commentPattern}.");
                return;
            }

            var startPatternNode = startAndEndNodes.First();
            var uncommentedNode = startPatternNode.NextNode;
            var endPatternNode = uncommentedNode.NextNode;

            if (uncommentedNode.NodeType == XmlNodeType.Comment)
            {
                _logger.WriteVerbose($"{filePath} contains already commented part within the pattern {commentPattern}");
                return;
            }

            if (endPatternNode == null || !endPatternNode.ToString().Contains(commentPattern))
            {
                _logger.WriteWarning($"{filePath} does not contain ending pattern '{commentPattern}' where it's expected.");
                return;
            }

            var uncommentText = uncommentedNode.ToString();

            var commentedNode = new XComment(uncommentText);

            uncommentedNode.ReplaceWith(commentedNode);

            _fileManager.Save(filePath, doc);
        }

        public void UncommentNode(string filePath, string commentPattern)
        {
            var doc = _fileManager.Load(filePath);

            var startPatternNode = doc.DescendantNodes().
                FirstOrDefault(node => node.NodeType == XmlNodeType.Comment && node.ToString().Contains(commentPattern));

            if (startPatternNode == null)
            {
                _logger.WriteWarning($"{filePath} does not contains pattern {commentPattern}.");
                return;
            }

            XElement uncommentedNode;
            if (TryParseCommentedNode(startPatternNode, out uncommentedNode))
            {
                startPatternNode.ReplaceWith(uncommentedNode);
            }
            else if (startPatternNode.NextNode != null && startPatternNode.NextNode.NodeType == XmlNodeType.Comment && TryParseCommentedNode(startPatternNode.NextNode, out uncommentedNode))
            {
                startPatternNode.NextNode.AddBeforeSelf(new XComment(commentPattern));
                startPatternNode.NextNode.ReplaceWith(uncommentedNode);
                startPatternNode.NextNode.AddAfterSelf(new XComment(commentPattern));
            }
            else
            {
                _logger.WriteVerbose($"{filePath} dose not contain commented part within the pattern {commentPattern}");
                return;
            }

            _fileManager.Save(filePath, doc);
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
    }
}
