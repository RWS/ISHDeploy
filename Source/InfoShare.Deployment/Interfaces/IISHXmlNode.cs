using System.Xml.Linq;

namespace InfoShare.Deployment.Interfaces
{
    /// <summary>
    /// Represents ISH xml nodes used in configuration.
    /// </summary>
    public interface IISHXmlNode
	{
		/// <summary>
		/// Gets node comemnt if exiss
		/// </summary>
		XComment GetNodeComment();

		/// <summary>
		/// Converts node to XElement
		/// </summary>
		XElement ToXElement();
    }
}
