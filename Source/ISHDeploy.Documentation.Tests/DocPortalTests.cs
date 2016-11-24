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

    [TestClass]
    public class DocPortalTests
    {
        [TestMethod]
        public void Check_all_links_of_DOC_portal()
        {
            // Arrange
            var pathToWebFolder = @"\\kiev-green-bld.global.sdl.corp\c$\inetpub\ishdeploy-doc-public";
            var baseUri = "http://kiev-green-bld.global.sdl.corp:8081/";
            var filesList = Directory.GetFiles(pathToWebFolder, "*.html", SearchOption.AllDirectories).ToList();

            // Action
            var taskList = filesList.Select(path => Task<Links>.Factory.StartNew(() => {
                var content = File.ReadAllText(path);
                var links = new List<Link>();

                string pattern = "href\\s*=\\s*(?:[\"'](?<1>[^\"']*)[\"']|(?<1>\\S+))";

                try
                {
                    Match m = Regex.Match(content, pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1));
                    while (m.Success)
                    {
                        var href = m.Groups[1].ToString();

                        if (!href.StartsWith("http") && href.EndsWith(".html"))
                        {
                            if (links.All(x => x.LinkAsItIsInFile == href))
                            {
                                var uri = new Uri(new Uri(baseUri), href);
                                var filePath = Path.Combine(pathToWebFolder,
                                    string.Join("\\",
                                        uri.LocalPath.Substring(uri.LocalPath.IndexOf('/') + 1).Replace("/", "\\")));

                                if (!File.Exists(filePath))
                                {
                                    links.Add(new Link
                                    {
                                        Index = m.Groups[1].Index,
                                        Uri = uri,
                                        LinkAsItIsInFile = href,
                                        FilePath = filePath
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

                return new Links { FilePath = path, LinksList = links };
            })).ToList();
            Task.WhenAll(taskList);

            var brokenLinks = taskList.Select(t => t.Result).Where(l => l.LinksList.Count > 0).ToList();

            var testResults = new StringBuilder();
            // Assert
            foreach (var fileWithBrokenLinks in brokenLinks)
            {
                if (testResults.Length == 0)
                {
                    testResults.AppendLine();
                }

                testResults.AppendLine($"File: {fileWithBrokenLinks.FilePath}");

                foreach (var link in fileWithBrokenLinks.LinksList)
                {
                    testResults.AppendLine(LinkInfo(link));
                }
            }

            Assert.IsFalse(testResults.Length > 0, $"Following files contains broken links: {testResults}");
        }

        [TestMethod]
        public void Check_all_src_of_DOC_portal_returns()
        {
            // Arrange
            var pathToWebFolder = @"\\kiev-green-bld.global.sdl.corp\c$\inetpub\ishdeploy-doc-public";
            var baseUri = "http://kiev-green-bld.global.sdl.corp:8081/";
            var filesList = Directory.GetFiles(pathToWebFolder, "*.html", SearchOption.AllDirectories).ToList();

            // Action
            var taskList = filesList.Select(path => Task<Links>.Factory.StartNew(() => {
                var content = File.ReadAllText(path);
                var links = new List<Link>();

                string pattern = "src\\s*=\\s*(?:[\"'](?<1>[^\"']*)[\"']|(?<1>\\S+))";

                try
                {
                    Match m = Regex.Match(content, pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1));
                    while (m.Success)
                    {
                        var src = m.Groups[1].ToString();

                        if (!src.StartsWith("http") && (src.EndsWith(".png") || src.EndsWith(".jpg")))
                        {
                            if (links.All(x => x.LinkAsItIsInFile == src))
                            {
                                var uri = new Uri(new Uri(baseUri), src);
                                var filePath = Path.Combine(pathToWebFolder,
                                    string.Join("\\",
                                        uri.LocalPath.Substring(uri.LocalPath.IndexOf('/') + 1).Replace("/", "\\")));

                                if (!File.Exists(filePath))
                                {
                                    links.Add(new Link
                                    {
                                        Index = m.Groups[1].Index,
                                        Uri = uri,
                                        LinkAsItIsInFile = src,
                                        FilePath = filePath
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

                return new Links { FilePath = path, LinksList = links };
            })).ToList();
            Task.WhenAll(taskList);

            var brokenLinks = taskList.Select(t => t.Result).Where(l => l.LinksList.Count > 0).ToList();

            var testResults = new StringBuilder();
            // Assert
            foreach (var fileWithBrokenLinks in brokenLinks)
            {
                if (testResults.Length == 0)
                {
                    testResults.AppendLine();
                }

                testResults.AppendLine($"File: {fileWithBrokenLinks.FilePath}");

                foreach (var link in fileWithBrokenLinks.LinksList)
                {
                    testResults.AppendLine(LinkInfo(link));
                }
            }

            Assert.IsFalse(testResults.Length > 0, $"Following files contains broken links: {testResults}");
        }

        private string LinkInfo(Link link)
        {
            return $"Index: {link.Index} Uri {link.Uri} File name: {link.FilePath}";
        }
    }

    public class Link
    {
        public int Index { get; set; }
        public Uri Uri { get; set; }
        public string LinkAsItIsInFile { get; set; }
        public string FilePath { get; set; }
    }

    public class Links
    {
        public string FilePath { get; set; }
        public List<Link> LinksList { get; set; }
    }
}
