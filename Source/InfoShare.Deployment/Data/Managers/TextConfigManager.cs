using System.Linq;
using InfoShare.Deployment.Data.Managers.Interfaces;
using InfoShare.Deployment.Interfaces;

namespace InfoShare.Deployment.Data.Managers
{
    /// <summary>
    /// Performs different kinds of operations with text file
    /// </summary>
    /// <seealso cref="ITextConfigManager" />
    public class TextConfigManager : ITextConfigManager
    {
        /// <summary>
        /// The start comment symbols
        /// </summary>
        private const string CommentSymbols = "//";

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// The file manager
        /// </summary>
        private readonly IFileManager _fileManager;
        
        /// <summary>
        /// Returns new instance of the <see cref="TextConfigManager"/>
        /// </summary>
        /// <param name="logger">Instance of the <see cref="ILogger"/></param>
        public TextConfigManager(ILogger logger)
        {
            _logger = logger;
            _fileManager = ObjectFactory.GetInstance<IFileManager>();
        }

        /// <summary>
        /// Comments block of text file between two <paramref name="searchPattern"/> comments
        /// </summary>
        /// <param name="filePath">Path to the file that is modified</param>
        /// <param name="searchPattern">Comment pattern that is searched for</param>
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

        /// <summary>
        /// Uncomments block of text file between two <paramref name="searchPattern"/> comments
        /// </summary>
        /// <param name="filePath">Path to the file that is modified</param>
        /// <param name="searchPattern">Comment pattern that is searched for</param>
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

        /// <summary>
        /// Comments the block of text file.
        /// </summary>
        /// <param name="lines">List of lines that represent whole content of the text file.</param>
        /// <param name="startIndex">The line number at which comment will begin.</param>
        /// <param name="count">Number of lines that will be commented.</param>
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

        /// <summary>
        /// Uncomments the block of text file.
        /// </summary>
        /// <param name="lines">List of lines that represent whole content of the text file.</param>
        /// <param name="startIndex">The line number at which will be started to uncomment.</param>
        /// <param name="count">Number of lines that will be uncommented.</param>
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
