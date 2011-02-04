using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using DynamicLinq.Queries;
using Microsoft.WindowsAzure;

namespace DynamicLinq.AzureTables
{
	internal class AzureTablesQueryConnection : QueryConnection
	{
		private WebResponse response;
		private Stream stream;
		private readonly IEnumerator<XElement> elements;

		internal AzureTablesQueryConnection(CloudStorageAccount storageAccount, QueryInfo queryInfo) : base(queryInfo)
		{
			HttpWebRequest request = (HttpWebRequest) WebRequest.Create(storageAccount.TableEndpoint.AbsoluteUri + "/" + queryInfo.Query);
			request.Method = "GET";
			request.KeepAlive = true;
			storageAccount.Credentials.SignRequestLite(request);

			response = request.GetResponse();
			stream = response.GetResponseStream();

			elements = XElement.Load(stream).Elements().Where(element => CheckName(element, AzureNamespaces.Atom, "entry")).GetEnumerator();
		}

		protected override bool Read(out object obj)
		{
			if (elements.MoveNext())
			{
				XElement content = elements.Current.Elements().First(element => CheckName(element, AzureNamespaces.Atom, "content"));
				XElement properties = content.Elements().First(element => CheckName(element, AzureNamespaces.DataServicesMetadata, "properties"));
				XAttribute updated = elements.Current.Attributes().First(element => CheckName(element, AzureNamespaces.DataServicesMetadata, "etag"));

				AzureTablesObject currentEntry = new AzureTablesObject(updated.Value);

				foreach (XElement property in properties.Elements())
				{
					XAttribute @null = property.Attributes().FirstOrDefault(attribute => CheckName(attribute, AzureNamespaces.DataServicesMetadata, "null"));
					XAttribute type = property.Attributes().FirstOrDefault(attribute => CheckName(attribute, AzureNamespaces.DataServicesMetadata, "type"));

					if (@null != null)
					{
						currentEntry.SetValue(property.Name.LocalName, null);
					}
					else if (type != null)
					{
						currentEntry.SetValue(property.Name.LocalName, AzureConvert.ParseValue(type.Value, property.Value));
					}
					else
					{
						currentEntry.SetValue(property.Name.LocalName, property.Value);
					}
				}

				obj = currentEntry;
				return true;
			}
			else
			{
				obj = null;
				return false;
			}
		}

		public override void Dispose()
		{
			elements.Dispose();

			if (stream != null)
			{
				stream.Dispose();
				stream = null;
			}

			if (response != null)
			{
				((IDisposable) response).Dispose();
				response = null;
			}

			IsDisposed = true;
		}

		private static bool CheckName(dynamic element, XNamespace @namespace, string localName)
		{
			return element.Name.Namespace == @namespace && element.Name.LocalName == localName;
		}

		//request.Accept = "application/atom+xml,application/xml";
		//request.Headers.Add("Accept-Charset", "UTF-8");
		//request.UserAgent = "Microsoft ADO.NET Data Services";
		//request.Headers.Add("DataServiceVersion", "1.0;NetFx");
		//request.Headers.Add("MaxDataServiceVersion", "2.0;NetFx");
		//request.Headers.Add("x-ms-version", "2009-09-19");

		//try
		//{
		//    request.GetResponse();
		//}
		//catch (WebException e)
		//{
		//    using (StreamReader asdf = new StreamReader(e.Response.GetResponseStream(), true))
		//    {
		//        StringBuilder sb = new StringBuilder();

		//        sb.AppendLine(asdf.ReadToEnd());

		//        foreach (var key in e.Response.Headers.AllKeys)
		//            sb.AppendLine(key + " " + e.Response.Headers[key]);

		//        sb.AppendLine(e.Response.ResponseUri.ToString());

		//        var s = sb.ToString();
		//    }
		//}

		//private static object ParseValue(string type, string value)
		//{
		//    switch (type)
		//    {
		//        case "Edm.String":
		//            return value;
		//        case "Edm.Byte":
		//            return byte.Parse(value);
		//        case "Edm.SByte":
		//            return sbyte.Parse(value);
		//        case "Edm.Int16":
		//            return short.Parse(value);
		//        case "Edm.Int32":
		//            return int.Parse(value);
		//        case "Edm.Int64":
		//            return long.Parse(value);
		//        case "Edm.Double":
		//            return double.Parse(value);
		//        case "Edm.Single":
		//        case "Edm.Float":
		//            return float.Parse(value);
		//        case "Edm.Boolean":
		//            return bool.Parse(value);
		//        case "Edm.Decimal":
		//            return decimal.Parse(value);
		//        case "Edm.DateTime":
		//            return XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.Local);
		//        case "Edm.Binary":
		//            return Convert.FromBase64String(value);
		//        case "Edm.Guid":
		//            return Guid.Parse(value);
		//        default:
		//            throw new NotSupportedException("Not supported type " + type);
		//    }
		//}
	}
}