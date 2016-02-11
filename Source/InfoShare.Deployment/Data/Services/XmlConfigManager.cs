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
        private readonly ILogger _logger;
        private readonly string _filePath;
        private string _backupFilePath;
        private readonly IFileManager _fileManager;

        public XmlConfigManager(ILogger logger, string filePath)
        {
            _logger = logger;
            _filePath = filePath;
            _fileManager = ObjectFactory.GetInstance<IFileManager>();
        }

        public void Backup()
        {
            _backupFilePath = _filePath + ".backup";
            _fileManager.Backup(_filePath, _backupFilePath);
        }

        public void CommentBlock(string searchPattern)
        {
            var doc = _fileManager.Load(_filePath);

            var startAndEndNodes = doc.DescendantNodes()
                .Where(node => node.NodeType == XmlNodeType.Comment && node.ToString().Contains(searchPattern));

            if (startAndEndNodes.Count() != 2)
            {
                _logger.WriteWarning($"{_filePath} does not contain start and end pattern '{searchPattern}' where it's expected.");
                return;
            }

            XNode uncommentedNode = startAndEndNodes.First().NextNode;

            if (uncommentedNode.NodeType == XmlNodeType.Comment)
            {
                _logger.WriteVerbose($"{_filePath} contains already commented part within the pattern {searchPattern}");
                return;
            }

            var uncommentText = uncommentedNode.ToString();

            var commentedNode = new XComment(uncommentText);

            uncommentedNode.ReplaceWith(commentedNode);

            _fileManager.Save(_filePath, doc);
        }

        public void CommentNode(string xpath)
        {

            var doc = _fileManager.Load(_filePath);

            var uncommentedNode = doc.XPathSelectElement(xpath);

            if (uncommentedNode == null)
            {
                _logger.WriteVerbose($"{_filePath} does not contain uncommented node within the xpath {xpath}");
                return;
            }

            var uncommentText = uncommentedNode.ToString();

            var commentedNode = new XComment(uncommentText);

            uncommentedNode.ReplaceWith(commentedNode);

            _fileManager.Save(_filePath, doc);
        }

        public void UncommentBlock(string searchPattern)
        {
            var doc = _fileManager.Load(_filePath);

            var startAndEndNodes = doc.DescendantNodes()
                .Where(node => node.NodeType == XmlNodeType.Comment && node.ToString().Contains(searchPattern));

            if (startAndEndNodes.Count() != 2)
            {
                _logger.WriteWarning($"{_filePath} does not contain start and end pattern '{searchPattern}' where it's expected.");
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
                _logger.WriteVerbose($"{_filePath} dose not contain commented part within the start and end pattern {searchPattern}");
                return;
            }

            _fileManager.Save(_filePath, doc);
        }

        public void UncommentNode(string searchPattern)
        {
            var doc = _fileManager.Load(_filePath);

            var startAndEndNodes = doc.DescendantNodes()
                .Where(node => node.NodeType == XmlNodeType.Comment && node.ToString().Contains(searchPattern));

            if (!startAndEndNodes.Any())
            {
                _logger.WriteWarning($"{_filePath} does not contain pattern '{searchPattern}' where it's expected.");
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
                _logger.WriteVerbose($"{_filePath} could not uncomment {commentedNode}");
                return;
            }

            _fileManager.Save(_filePath, doc);
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

        public void RestoreOriginal()
        {
            _fileManager.RestoreOriginal(_backupFilePath, _filePath);
        }

        public void SetAttributeValue(string filePath, string xpath, string attributeName, string value)
        {
            var doc = _fileManager.Load(_filePath);

            var element = doc.XPathSelectElement(xpath);

            if (element == null)
            {
                _logger.WriteWarning($"{_filePath} does not contain element '{xpath}'.");
                return;
            }
            element.SetAttributeValue(attributeName, value);

            _fileManager.Save(_filePath, doc);
        }
    }
}
