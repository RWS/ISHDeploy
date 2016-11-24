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

    public class Links
    {
        public string FilePath { get; set; }
        public List<Link> BrokenLinksList { get; set; }
    }

    [TestClass]
    public class DocPortalTests
    {
        private string GetBrokenLinks(string linkType, string fileType)
        {
            // Arrange
            var pathToWebFolder = @"\\kiev-green-bld.global.sdl.corp\c$\inetpub\ishdeploy-doc-public";
            var baseUri = "http://kiev-green-bld.global.sdl.corp:8081/";
            var filesList = Directory.GetFiles(pathToWebFolder, "*.html", SearchOption.AllDirectories).ToList();

            // Action
            var taskList = filesList.Select(path => Task<Links>.Factory.StartNew(() => {
                var content = File.ReadAllText(path);
                var directoryRelativePath = path.Replace(pathToWebFolder, string.Empty).Replace(Path.GetFileName(path), string.Empty);
                var links = new List<Link>();

                string pattern = $"{linkType}\\s*=\\s*(?:[\"'](?<1>[^\"']*)[\"']|(?<1>\\S+))";

                try
                {
                    Match m = Regex.Match(content, pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1));
                    while (m.Success)
                    {
                        var link = m.Groups[1].ToString();

                        if (!link.StartsWith("http") && link.EndsWith(fileType))
                        {
                            if (links.All(x => x.LinkAsItIsInFile == link))
                            {
                                var uri = new Uri(new Uri($"{baseUri}{directoryRelativePath.Substring(directoryRelativePath.IndexOf('\\') + 1).Replace("\\", "/")}"), link);
                                var filePath = Path.Combine(pathToWebFolder,
                                    string.Join("\\",
                                        uri.LocalPath.Substring(uri.LocalPath.IndexOf('/') + 1).Replace("/", "\\")));

                                if (!File.Exists(filePath))
                                {
                                    links.Add(new Link
                                    {
                                        Uri = uri,
                                        LinkAsItIsInFile = link
                                    });
                                }
                            }
                        }
                        m = m.NextMatch();
                    }
                }
                catch (RegexMatchTimeoutException)
                {
                    Console.WriteLine($"The matching operation timed out for file {path}");
                }

                return new Links { FilePath = path, BrokenLinksList = links };
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
                    testResults.AppendLine(LinkInfo(link));
                }
            }

            return testResults.ToString();
        }

        private string LinkInfo(Link link)
        {
            return $"Link in file: \"{link.LinkAsItIsInFile}\"\n\nUri: \"{link.Uri}\"";
        }

        [TestMethod]
        public void Check_all_href_of_DOC_portal()
        {
            var testResults = GetBrokenLinks("href", ".html");

            Assert.IsFalse(testResults.Length > 0, $"Following files contains broken links: {testResults}");
        }

        [TestMethod]
        public void Check_all_png_src_of_DOC_portal_returns()
        {
            var testResults = GetBrokenLinks("src", ".png");

            Assert.IsFalse(testResults.Length > 0, $"Following files contains broken links: {testResults}");
        }
    }
}
