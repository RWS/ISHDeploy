using System.Xml.Linq;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Models.ISHXmlNodes
{
	/// <summary>
	/// Represents menu item xml node
	/// </summary>
	public class IssuerThumbprintItem : IISHXmlNode
	{
		/// <summary>
		/// Xml node name.
		/// </summary>
		protected virtual string XmlElementName => "add";

		/// <summary>
		/// Gets or sets the thumbprint.
		/// </summary>
		/// <value>
		/// The thumbprint value.
		/// </value>
		public string Thumbprint { get; set; }

		/// <summary>
		/// Gets or sets the Issuer name.
		/// </summary>
		/// <value>
		/// The Issuer name value.
		/// </value>
		public string Issuer { get; set; }

		/// <summary>
		/// Gets node comemnt if exiss
		/// </summary>
		public XComment GetNodeComment()
		{
			return null;
		}

		/// <summary>
		/// Converts object to XElement.
		/// </summary>
		/// <returns>XElement</returns>
		public virtual XElement ToXElement()
		{
			return new XElement(XmlElementName,
				new XAttribute("thumbprint", Thumbprint),
				new XAttribute("name", Issuer));
		}
	}
}
