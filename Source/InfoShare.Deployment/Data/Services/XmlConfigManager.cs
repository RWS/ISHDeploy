using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using InfoShare.Deployment.Interfaces;

namespace InfoShare.Deployment.Data.Services
{
    public class XmlConfigManager : IXmlConfigManager
    {
        private readonly ILogger _logger;
        private readonly string _filePath;
        private string _backupFilePath;

        public XmlConfigManager(ILogger logger, string filePath)
        {
            _logger = logger;
            _filePath = filePath;
        }

        public void Backup()
        {
            _backupFilePath = _filePath + ".backup";

            File.Copy(_filePath, _backupFilePath);
        }

        public void CommentNode(string commentPattern)
        {
            if (string.IsNullOrWhiteSpace(_backupFilePath))
            {
                //TODO: ????
            }

            XDocument doc = XDocument.Load(_backupFilePath);
            var startPatternNode = doc.DescendantNodes()
                .FirstOrDefault(node => node.NodeType == XmlNodeType.Comment && node.ToString().Contains(commentPattern));

            if (startPatternNode == null)
            {
                _logger.WriteWarning($"{_filePath} does not contains pattern {commentPattern}.");
                return;
            }

            var commentedNode = startPatternNode.NextNode;
            var endPatternNode = commentedNode.NextNode;

            if (commentedNode.NodeType == XmlNodeType.Comment)
            {
                _logger.WriteDetail($"{_filePath} contains already commented part within the pattern {commentPattern}");
                return;
            }

            if (endPatternNode.ToString().Contains(commentPattern))
            {
                _logger.WriteWarning($"{_filePath} does not contain ending pattern '{commentPattern}' where it's expected.");
                return;
            }

            var commentText = commentedNode.ToString().TrimStart('<').TrimEnd('>');
            var startIndex = commentText.IndexOf('<');
            var endIndex = commentText.LastIndexOf('>');

            commentText = commentText.Substring(startIndex, endIndex - startIndex + 1);

            var uncommentedNode = XElement.Parse(commentText);

            commentedNode.ReplaceWith(uncommentedNode);

            doc.Save(_filePath);
        }

        public void UncommentNode(string commentPattern)
        {
            if (string.IsNullOrWhiteSpace(_backupFilePath))
            {
                //TODO: ????
            }

            XDocument doc = XDocument.Load(_backupFilePath);
            var startPatternNode = doc.DescendantNodes().
                FirstOrDefault(node => node.NodeType == XmlNodeType.Comment && node.ToString().Contains(commentPattern));

            if (startPatternNode == null)
            {
                _logger.WriteWarning($"{_filePath} does not contains pattern {commentPattern}.");
                return;
            }

            var commentedNode = startPatternNode.NextNode;

            if (commentedNode.NodeType != XmlNodeType.Comment)
            {
                _logger.WriteDetail($"{_filePath} contains already uncommented  part within the pattern {commentPattern}");
                return;
            }

            var commentText = commentedNode.ToString().TrimStart('<').TrimEnd('>');
            var startIndex = commentText.IndexOf('<');
            var endIndex = commentText.LastIndexOf('>');

            commentText = commentText.Substring(startIndex, endIndex - startIndex + 1);

            var uncommentedNode = XElement.Parse(commentText);

            commentedNode.ReplaceWith(uncommentedNode);

            doc.Save(_filePath);
        }
        
        public void RestoreOriginal()
        {
            if (string.IsNullOrWhiteSpace(_backupFilePath))
            {
                _logger.WriteWarning("File was not restored because backup file path is empty");
                return;
            }

            File.Copy(_backupFilePath, _filePath);
        }
    }
}
