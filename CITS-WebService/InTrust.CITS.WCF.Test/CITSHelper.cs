using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Serialization;
using System.Text;
using System.IO;

using InTrust.CITS.WCF.Test.InTrustCITSService;

namespace InTrust.CITS.WCF
{
	/// <summary>
	/// Static class for various helper methods
	/// </summary>
	public static class CITSHelper
	{
		/// <summary>
		/// Serializes the TXLife message.
		/// Here only for debugging purposes.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <returns>The serialized message.</returns>
		public static string SerializeMessage(TXLife_Type message)
		{
			var serializer = new XmlSerializer(typeof(TXLife_Type), "CITS");
			var sb = new StringBuilder();
			using (var writer = new StringWriter(sb))
			{
				try
				{
					serializer.Serialize(writer, message);
				}
				catch (Exception ex)
				{
					sb.Append(ex.Message);
				}
			}
			var result = sb.ToString();
			return result;
		}
	}
}