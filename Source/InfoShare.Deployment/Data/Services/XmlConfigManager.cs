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

        public void CommentNode(string commentPattern)
        {
            var doc = _fileManager.Load(_filePath);
            
            var startPatternNode = doc.DescendantNodes()
                .FirstOrDefault(node => node.NodeType == XmlNodeType.Comment && node.ToString().Contains(commentPattern));

            if (startPatternNode == null)
            {
                _logger.WriteWarning($"{_filePath} does not contains pattern {commentPattern}.");
                return;
            }

            var uncommentedNode = startPatternNode.NextNode;
            var endPatternNode = uncommentedNode.NextNode;

            if (uncommentedNode.NodeType == XmlNodeType.Comment)
            {
                _logger.WriteDetail($"{_filePath} contains already commented part within the pattern {commentPattern}");
                return;
            }

            if (!endPatternNode.ToString().Contains(commentPattern))
            {
                _logger.WriteWarning($"{_filePath} does not contain ending pattern '{commentPattern}' where it's expected.");
                return;
            }

            var uncommentText = uncommentedNode.ToString();

            var commentedNode = new XComment(uncommentText);

            uncommentedNode.ReplaceWith(commentedNode);

            _fileManager.Save(_filePath, doc);
        }

        public void UncommentNode(string commentPattern)
        {
            var doc = _fileManager.Load(_filePath);

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

            _fileManager.Save(_filePath, doc);
        }
        
        public void RestoreOriginal()
        {
            _fileManager.RestoreOriginal(_backupFilePath, _filePath);
        }
    }
}
