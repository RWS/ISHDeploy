using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace InfoShare.Deployment.Tests.Extensions
{
    public static class XDocumentExtension
    {
        /// <summary>
        /// Checks internal content of the XDocuments as a string, without taking into account all whitespaces
        /// </summary>
        /// <param name="doc1">Current document</param>
        /// <param name="doc2">Another document to check with</param>
        /// <returns></returns>
        public static bool IsSameAs(this XDocument doc1, XDocument doc2)
        {
            var resultStr = Regex.Replace(doc1.ToString(), @"\s", "");
            var expectedStr = Regex.Replace(doc2.ToString(), @"\s", "");

            return resultStr.Equals(expectedStr);
        }
    }
}
