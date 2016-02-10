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
            
            var startAndEndNodes = doc.DescendantNodes()
                .Where(node => node.NodeType == XmlNodeType.Comment && node.ToString().Contains(commentPattern));

            if (startAndEndNodes == null || startAndEndNodes.Count() != 2)
            {
                _logger.WriteWarning($"{_filePath} does not contains start or end pattern {commentPattern}.");
                return;
            }

            var startPatternNode = startAndEndNodes.First();
            var uncommentedNode = startPatternNode.NextNode;
            var endPatternNode = uncommentedNode.NextNode;

            if (uncommentedNode.NodeType == XmlNodeType.Comment)
            {
                _logger.WriteVerbose($"{_filePath} contains already commented part within the pattern {commentPattern}");
                return;
            }

            if (endPatternNode == null || !endPatternNode.ToString().Contains(commentPattern))
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
                _logger.WriteVerbose($"{_filePath} dose not contain commented part within the pattern {commentPattern}");
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
    }
}
