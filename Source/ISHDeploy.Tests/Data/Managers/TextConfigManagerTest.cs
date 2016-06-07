using System.Linq;
using ISHDeploy.Business.Operations;
using ISHDeploy.Data.Managers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace ISHDeploy.Tests.Data.Managers
{
    [TestClass]
    public class TextConfigManagerTest : BaseUnitTest
    {
        private TextConfigManager _textConfigManager;

        #region Test Data
        private const string FilePath = "C:\\SomeDummyFilePath.txt";
        
        private readonly string[] UncommentedOneBlock =  {
            "    //",
            "    //Translation Jobs hack",
            "    //",
            "    sNode = sNode + \"<table>Text</table>\"",
            "    ",
            "    sNode = sNode + \"<p>Text</p>\"",
            "    //",
            "    //Translation Jobs hack",
            "    //"};

        private readonly string[] CommentedOneBlock = {
            "    //",
            "    //Translation Jobs hack",
            "//    //",
            "//    sNode = sNode + \"<table>Text</table>\"",
            "//    ",
            "//    sNode = sNode + \"<p>Text</p>\"",
            "//    //",
            "    //Translation Jobs hack",
            "    //"};

        private readonly string[] UncommentedFewBlocks =  {
            "    //",
            "    //Translation Jobs hack",
            "    //",
            "    sNode = sNode + \"<table>Text</table>\"",
            "    //",
            "    //Translation Jobs hack",
            "    //",
            "    //Translation Jobs hack",
            "    sNode = sNode + \"<table>Another text</table>\" /*comment at the end*/",
            "    //Translation Jobs hack",
            "    //Translation Jobs hack",
            "    sNode = sNode + \"<table>And Another text</table>\" // some comment",
            "    //Translation Jobs hack"};

        private readonly string[] CommentedFewBlocks = {
            "    //",
            "    //Translation Jobs hack",
            "//    //",
            "//    sNode = sNode + \"<table>Text</table>\"",
            "//    //",
            "    //Translation Jobs hack",
            "    //",
            "    //Translation Jobs hack",
            "//    sNode = sNode + \"<table>Another text</table>\" /*comment at the end*/",
            "    //Translation Jobs hack",
            "    //Translation Jobs hack",
            "//    sNode = sNode + \"<table>And Another text</table>\" // some comment",
            "    //Translation Jobs hack"};

        private readonly string[] NoPatternBlock ={
            "    //",
            "    // No Pattern here. Translation Jobs hack",
            "    sNode = sNode + \"<table>Text</table>\"",
            "    // No Pattern here. Translation Jobs hack",
            "    //"};

        private readonly string[] UncommentedBrokenPatternBlock =  {
            "    //Translation Jobs hack",
            "    sNode = sNode + \"<table>Text</table>\"",
            "    //Translation Jobs hack",
            "    //Translation Jobs hack",
            "    sNode = sNode + \"<table>And Another text</table>\" // some comment",};

        private readonly string[] CommentedBrokenPatternBlock = {
            "    //Translation Jobs hack",
            "//    sNode = sNode + \"<table>Text</table>\"",
            "    //Translation Jobs hack",
            "    //Translation Jobs hack",
            "    sNode = sNode + \"<table>And Another text</table>\" // some comment",};
        #endregion

        [TestInitialize]
        public void TestInitializer()
        {
            _textConfigManager = new TextConfigManager(Logger);
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void CommentBlock_One_block_commented()
        {
            // Arrange
            string[] result = null;
            FileManager.ReadAllLines(FilePath).Returns(UncommentedOneBlock);
            FileManager.WriteAllLines(FilePath, Arg.Do<string[]>(lines => result = lines));

            // Act
            _textConfigManager.CommentBlock(FilePath, "//Translation Jobs hack");

            // Assert
            FileManager.Received(1).WriteAllLines(FilePath, Arg.Any<string[]>());
            Assert.IsTrue(result.SequenceEqual(CommentedOneBlock));
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void CommentBlock_Few_blocks_commented()
        {
            // Arrange
            string[] result = null;
            FileManager.ReadAllLines(FilePath).Returns(UncommentedFewBlocks);
            FileManager.WriteAllLines(FilePath, Arg.Do<string[]>(lines => result = lines));

            // Act
            _textConfigManager.CommentBlock(FilePath, "//Translation Jobs hack");

            // Assert
            FileManager.Received(1).WriteAllLines(FilePath, Arg.Any<string[]>());
            Assert.IsTrue(result.SequenceEqual(CommentedFewBlocks));
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void CommentBlock_None_patterns_found()
        {
            // Arrange
            FileManager.ReadAllLines(FilePath).Returns(NoPatternBlock);

            // Act
            _textConfigManager.CommentBlock(FilePath, "//Translation Jobs hack");

            // Assert
            FileManager.DidNotReceive().WriteAllLines(Arg.Any<string>(), Arg.Any<string[]>());
            Logger.Received(1).WriteWarning(Arg.Is($"[{FilePath}][No comment patterns were found]"));
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void CommentBlock_Broken_comment_pattern()
        {
            // Arrange
            string[] result = null;
            FileManager.ReadAllLines(FilePath).Returns(UncommentedBrokenPatternBlock);
            FileManager.WriteAllLines(FilePath, Arg.Do<string[]>(lines => result = lines));

            // Act
            _textConfigManager.CommentBlock(FilePath, "//Translation Jobs hack");

            // Assert
            FileManager.Received(1).WriteAllLines(FilePath, Arg.Any<string[]>());
            Logger.Received(1).WriteWarning(Arg.Is($"[{FilePath}][Cannot not find end of the comment pattern '{"//Translation Jobs hack"}']"));
            Assert.IsTrue(result.SequenceEqual(CommentedBrokenPatternBlock));
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void UncommentBlock_One_block_commented()
        {
            // Arrange
            string[] result = null;
            FileManager.ReadAllLines(FilePath).Returns(CommentedOneBlock);
            FileManager.WriteAllLines(FilePath, Arg.Do<string[]>(lines => result = lines));

            // Act
            _textConfigManager.UncommentBlock(FilePath, "//Translation Jobs hack");

            // Assert
            FileManager.Received(1).WriteAllLines(FilePath, Arg.Any<string[]>());
            Assert.IsTrue(result.SequenceEqual(UncommentedOneBlock));
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void UncommentBlock_Few_blocks_commented()
        {
            // Arrange
            string[] result = null;
            FileManager.ReadAllLines(FilePath).Returns(CommentedFewBlocks);
            FileManager.WriteAllLines(FilePath, Arg.Do<string[]>(lines => result = lines));

            // Act
            _textConfigManager.UncommentBlock(FilePath, "//Translation Jobs hack");

            // Assert
            FileManager.Received(1).WriteAllLines(FilePath, Arg.Any<string[]>());
            Assert.IsTrue(result.SequenceEqual(UncommentedFewBlocks));
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void UncommentBlock_None_patterns_found()
        {
            // Arrange
            FileManager.ReadAllLines(FilePath).Returns(NoPatternBlock);
            
            // Act
            _textConfigManager.UncommentBlock(FilePath, "//Translation Jobs hack");

            // Assert
            FileManager.DidNotReceive().WriteAllLines(Arg.Any<string>(), Arg.Any<string[]>());
            Logger.Received(1).WriteWarning(Arg.Is($"[{FilePath}][No comment patterns were found]"));
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void UncommentBlock_Broken_comment_pattern()
        {
            // Arrange
            string[] result = null;
            FileManager.ReadAllLines(FilePath).Returns(CommentedBrokenPatternBlock);
            FileManager.WriteAllLines(FilePath, Arg.Do<string[]>(lines => result = lines));
            
            // Act
            _textConfigManager.UncommentBlock(FilePath, "//Translation Jobs hack");

            // Assert
            FileManager.Received(1).WriteAllLines(FilePath, Arg.Any<string[]>());
            Logger.Received(1).WriteWarning(Arg.Is($"[{FilePath}][Cannot not find end of the comment pattern '{"//Translation Jobs hack"}']"));
            Assert.IsTrue(result.SequenceEqual(UncommentedBrokenPatternBlock));
        }
    }
}
