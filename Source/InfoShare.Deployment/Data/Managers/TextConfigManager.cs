using System.Linq;
using InfoShare.Deployment.Data.Managers.Interfaces;
using InfoShare.Deployment.Interfaces;

namespace InfoShare.Deployment.Data.Managers
{
    public class TextConfigManager : ITextConfigManager
    {
        private const string CommentSymbols = "//";

        private readonly ILogger _logger;
        private readonly IFileManager _fileManager;
        
        public TextConfigManager(ILogger logger)
        {
            _logger = logger;
            _fileManager = ObjectFactory.GetInstance<IFileManager>();
        }
        
        public void CommentBlock(string filePath, string searchPattern)
        {
            var strLines = _fileManager.ReadAllLines(filePath);

            var patternIndex = -2;

            for (var i = 0; i < strLines.Length; i++)
            {
                if (!strLines[i].Contains(searchPattern)) continue;

                if (patternIndex < 0)
                {
                    patternIndex = i + 1; // take next line after searched pattern
                    continue;
                }

                CommentBlock(strLines, patternIndex, i - patternIndex);
                patternIndex = -1;
            }

            if (patternIndex >= 0)
            {
                _logger.WriteWarning($"Cannot not find end of the comment pattern '{searchPattern}' in the file: {filePath}");
            }
            else if (patternIndex == -2)
            {
                _logger.WriteWarning($"No comment patterns were found in the file {filePath}");
                return;
            }

            _fileManager.WriteAllLines(filePath, strLines);
        }

        public void UncommentBlock(string filePath, string searchPattern)
        {
            var strLines = _fileManager.ReadAllLines(filePath);

            var patternIndex = -2;

            for (var i = 0; i < strLines.Length; i++)
            {
                if (!strLines[i].Contains(searchPattern)) continue;

                if (patternIndex < 0)
                {
                    patternIndex = i + 1; // take next line after searched pattern
                    continue;
                }

                UncommentBlock(strLines, patternIndex, i - patternIndex);
                patternIndex = -1;
            }

            if (patternIndex >= 0)
            {
                _logger.WriteWarning($"Cannot not find end of the comment pattern '{searchPattern}' in the file: {filePath}");
            }
            else if (patternIndex == -2)
            {
                _logger.WriteWarning($"No comment patterns were found in the file {filePath}");
                return;
            }

            _fileManager.WriteAllLines(filePath, strLines);
        }

        private void CommentBlock(string[] lines, int startIndex, int count)
        {
            var isAnyUncommented = lines
                .Skip(startIndex)
                .Take(count)
                .Any(line => !line.TrimStart().StartsWith(CommentSymbols) && !string.IsNullOrWhiteSpace(line));
            
            if (!isAnyUncommented)
            {
                _logger.WriteVerbose("Text block is already fully commented");
                return;
            }
            
            for (var i = startIndex; i < startIndex + count; i++)
            {
                lines[i] = CommentSymbols + lines[i];
            }
        }

        private void UncommentBlock(string[] lines, int startIndex, int count)
        {
            var isAllCommented = lines
                .Skip(startIndex)
                .Take(count)
                .All(line => line.TrimStart().StartsWith(CommentSymbols) && !string.IsNullOrWhiteSpace(line));
            
            if (!isAllCommented)
            {
                _logger.WriteVerbose("Text block contains uncommented lines");
                return;
            }

            for (var i = startIndex; i < startIndex + count; i++)
            {
                lines[i] = lines[i].TrimStart().Substring(CommentSymbols.Length);
            }
        }
    }
}
