﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Serialization;
using System.Text;
using System.IO;

using Apexa.CITS.WCF.Test.CITSService;

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
		public static string SerializeMessage<T>(T message)
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
            return result;
        }
    }
}