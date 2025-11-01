using System.Xml;

namespace YotanModCore.DataStore
{
	internal static class SaveUtils
	{
		/// <summary>
		/// Tries to convert data to a XmlNode[] and return a valid name for its type.
		/// This is meant to be used when writing error messages (e.g. when a type can't be parsed)
		/// </summary>
		/// <param name="data"></param>
		/// <returns>A string representing the data from "data". It gives some valid strings if it fails</returns>
		internal static string TryGetXmlNodeName(object data)
		{
			if (data is not XmlNode[] nodes)
				return "<Unknown Data>";

			if (nodes.Length == 0)
				return "<Unknown Empty>";

			return $"<{nodes[0].InnerText}>";
		}
	}
}
