using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace ISHDeploy.Documentation.Tests
{
    public class Link
    {
        public Uri Uri { get; set; }
        public string LinkAsItIsInFile { get; set; }
    }

    public class FileWithBrokenLinks
    {
        public string FilePath { get; set; }
        public List<Link> BrokenLinksList { get; set; }
    }

    [TestClass]
    public class DocPortalTests
    {
        #region Methods to get broken links

        /// <summary>
        /// Returns Uri to folder where the HTML file situated
        /// </summary>
        /// <param name="baseUri">Base Uri to Website in string format</param>
        /// <param name="pathToWebFolder">The path to the root folder of Website. Will be replaced on baseUri to define Uri to HTML file</param>
        /// <param name="pathToHtmlFile">The path to HTML file. This path is needed to build Uri to folder where this file is situated</param>
        /// <returns>Returns Uri to folder where the HTML file situated</returns>
        private Uri GetUriToFolderByPathToHtmlFile(string baseUri, string pathToWebFolder, string pathToHtmlFile)
        {
            var relativePathToFolderOfHtmlFile = pathToHtmlFile.Replace(pathToWebFolder, string.Empty).Replace(Path.GetFileName(pathToHtmlFile), string.Empty);
            return
                new Uri(
                    $"{baseUri}{relativePathToFolderOfHtmlFile.Substring(relativePathToFolderOfHtmlFile.IndexOf('\\') + 1).Replace("\\", "/")}");
        }

        /// <summary>
        /// Returns real path to element from HTML file as path to file in file system
        /// </summary>
        /// <param name="uriToElementFromHtmlFile">Real Uri to element from a Html file</param>
        /// <param name="pathToWebFolder">The path to the root folder of Website where element is situated</param>
        /// <returns>Returns real path to element from HTML file as path to file in file system</returns>
        private string GetRealPathToElementByUri(Uri uriToElementFromHtmlFile, string pathToWebFolder)
        {
            return Path.Combine(pathToWebFolder,
                                    string.Join("\\",
                                        uriToElementFromHtmlFile.LocalPath.Substring(uriToElementFromHtmlFile.LocalPath.IndexOf('/') + 1).Replace("/", "\\")));
        }

        /// <summary>
        /// Returns list of HTML files with broken links
        /// </summary>
        /// <param name="linkType">The type of link to search ("href" or "src")</param>
        /// <param name="fileType">The expansion of the element to search it in all HTML files</param>
        /// <returns>Returns list of HTML files with broken links</returns>
        private string GetBrokenLinks(string linkType, string fileType)
        {
            // Arrange
            var pathToWebFolder = @"\\kiev-green-bld.global.sdl.corp\c$\inetpub\ishdeploy-doc-public";
            var baseUri = "http://kiev-green-bld.global.sdl.corp:8081/";
            var htmlFilesPaths = Directory.GetFiles(pathToWebFolder, "*.html", SearchOption.AllDirectories).ToList();

            // Action
            var taskList = htmlFilesPaths.Select(pathToHtmlFile => Task<FileWithBrokenLinks>.Factory.StartNew(() => {
                var links = new List<Link>();

                // To build Uri to element in file we need to know Uri to folder where this HTML file situated,
                // because a link in the HTML file can be relative
                var uriToFolderWithHtmlFile = GetUriToFolderByPathToHtmlFile(baseUri, pathToWebFolder, pathToHtmlFile);

                var content = File.ReadAllText(pathToHtmlFile);

                string pattern = $"{linkType}\\s*=\\s*(?:[\"'](?<1>[^\"']*)[\"']|(?<1>\\S+))";

                try
                {
                    Match m = Regex.Match(content, pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1));
                    while (m.Success)
                    {
                        var linkToElementInHtmlFile = m.Groups[1].ToString();

                        if (!linkToElementInHtmlFile.StartsWith("http") && linkToElementInHtmlFile.EndsWith(fileType))
                        {
                            if (links.All(x => x.LinkAsItIsInFile == linkToElementInHtmlFile))
                            {
                                var uriToElementFromHtmlFile = new Uri(uriToFolderWithHtmlFile, linkToElementInHtmlFile);
                                var pathToElementAsToFile = GetRealPathToElementByUri(uriToElementFromHtmlFile, pathToWebFolder);

                                if (!File.Exists(pathToElementAsToFile))
                                {
                                    links.Add(new Link
                                    {
                                        Uri = uriToElementFromHtmlFile,
                                        LinkAsItIsInFile = linkToElementInHtmlFile
                                    });
                                }
                            }
                        }
                        m = m.NextMatch();
                    }
                }
                catch (RegexMatchTimeoutException)
                {
                    Console.WriteLine($"The matching operation timed out for file {pathToHtmlFile}");
                }

                return new FileWithBrokenLinks { FilePath = pathToHtmlFile, BrokenLinksList = links };
            })).ToList();
            Task.WhenAll(taskList);

            var brokenLinks = taskList.Select(t => t.Result).Where(l => l.BrokenLinksList.Count > 0).ToList();

            var testResults = new StringBuilder();
            foreach (var fileWithBrokenLinks in brokenLinks)
            {
                if (testResults.Length == 0)
                {
                    testResults.AppendLine();
                }

                testResults.AppendLine($"File: \"{fileWithBrokenLinks.FilePath}\"");

                foreach (var link in fileWithBrokenLinks.BrokenLinksList)
                {
                    testResults.AppendLine($"Link in file: \"{link.LinkAsItIsInFile}\"\n\nBroken Uri: \"{link.Uri}\"");
                }
            }

            return testResults.ToString();
        }

        #endregion

        [TestMethod]
        public void Check_all_HTML_links_of_DOC_portal()
        {
            var testResults = GetBrokenLinks("href", ".html");

            Assert.IsFalse(testResults.Length > 0, $"Following files contains broken links: {testResults}");
        }

        [TestMethod]
        public void Check_all_png_links_of_DOC_portal_returns()
        {
            var testResults = GetBrokenLinks("src", ".png");

            Assert.IsFalse(testResults.Length > 0, $"Following files contains broken links: {testResults}");
        }
    }
}
