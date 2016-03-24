
namespace ISHDeploy.Data.Managers.Interfaces
{
    /// <summary>
    /// Performs different kinds of operations with text file
    /// </summary>
    public interface ITextConfigManager
    {
        /// <summary>
        /// Comments block of text file between two <paramref name="searchPattern"/> comments
        /// </summary>
        /// <param name="filePath">Path to the file that is modified</param>
        /// <param name="searchPattern">Comment pattern that is searched for</param>
        void CommentBlock(string filePath, string searchPattern);

        /// <summary>
        /// Uncomments block of text file between two <paramref name="searchPattern"/> comments
        /// </summary>
        /// <param name="filePath">Path to the file that is modified</param>
        /// <param name="searchPattern">Comment pattern that is searched for</param>
        void UncommentBlock(string filePath, string searchPattern);
    }
}
