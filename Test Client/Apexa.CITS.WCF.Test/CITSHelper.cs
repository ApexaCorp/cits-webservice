using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Apexa.CITS.WCF
{
    /// <summary>
    /// Static class for various helper methods
    /// </summary>
    public static class CITSHelper
    {
        /// <summary>
		/// Serializes generic message.
		/// Here only for debugging purposes.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <returns>The serialized message.</returns>
		public static void SerializeMessage<T>(T message)
        {
            var serializer = new XmlSerializer(typeof(T), "GenericType");
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
            Console.WriteLine(result);
            Console.WriteLine();
        }
    }
}