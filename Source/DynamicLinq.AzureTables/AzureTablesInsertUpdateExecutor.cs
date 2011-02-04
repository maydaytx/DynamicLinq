using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using Microsoft.WindowsAzure;

namespace DynamicLinq.AzureTables
{
	internal static class AzureTablesInsertUpdateExecutor
	{
		internal static void Insert(CloudStorageAccount storageAccount, string tableName, AzureTablesObject[] rows)
		{
			if (rows.Length == 1)
			{
				HttpWebRequest request = (HttpWebRequest) WebRequest.Create(storageAccount.TableEndpoint.AbsoluteUri + "/" + tableName);

				ExcludeExpect100ContinueHeader(request.RequestUri);

				request.Method = "POST";
				request.KeepAlive = true;
				request.AllowWriteStreamBuffering = true;
				request.ContentType = "application/atom+xml";
				storageAccount.Credentials.SignRequestLite(request);

				XDocument document = GetDocument(rows[0], false, null);

				document.Save(request.GetRequestStream());

				((IDisposable) request.GetResponse()).Dispose();
			}
			else if (rows.Length > 1)
			{
				Func<AzureTablesObject, string> getUrl = row => storageAccount.TableEndpoint.AbsoluteUri + "/" + tableName;

				ExecuteBatch(rows, storageAccount, "POST", getUrl, true);
			}
		}

		internal static void Update(CloudStorageAccount storageAccount, string tableName, AzureTablesObject[] rows)
		{
			if (rows.Length == 1)
			{
				string id = string.Format("{0}/{1}(PartitionKey='{2}',RowKey='{3}')", storageAccount.TableEndpoint.AbsoluteUri, tableName, rows[0].PartitionKey, rows[0].RowKey);

				HttpWebRequest request = (HttpWebRequest) WebRequest.Create(id);

				ExcludeExpect100ContinueHeader(request.RequestUri);

				request.Method = "MERGE";
				request.KeepAlive = true;
				request.AllowWriteStreamBuffering = true;
				request.ContentType = "application/atom+xml";
				request.Headers.Add("If-Match", rows[0].ETag ?? "*");
				storageAccount.Credentials.SignRequestLite(request);

				XDocument document = GetDocument(rows[0], true, id);

				document.Save(request.GetRequestStream());
				
				((IDisposable) request.GetResponse()).Dispose();
			}
			else if (rows.Length > 1)
			{
				Func<AzureTablesObject, string> getUrl = row => string.Format("{0}/{1}(PartitionKey='{2}',RowKey='{3}')", storageAccount.TableEndpoint.AbsoluteUri, tableName, row.PartitionKey, row.RowKey);
				Func<AzureTablesObject, string> getIfMatchHeader = row => "If-Match: " + (row.ETag ?? "*");

				ExecuteBatch(rows, storageAccount, "MERGE", getUrl, true, getIfMatchHeader);
			}
		}

		private static void ExecuteBatch(IList<AzureTablesObject> rows, CloudStorageAccount storageAccount, string method, Func<AzureTablesObject, string> getUrl, bool includeId, params Func<AzureTablesObject, string>[] additionalHeaders)
		{
			string batchId = "batch_" + Guid.NewGuid().ToString("D").ToLower();
			string changesetId = "changeset_" + Guid.NewGuid().ToString("D").ToLower();

			HttpWebRequest request = (HttpWebRequest) WebRequest.Create(storageAccount.TableEndpoint.AbsoluteUri + "/$batch");

			ExcludeExpect100ContinueHeader(request.RequestUri);

			request.Method = "POST";
			request.KeepAlive = true;
			request.AllowWriteStreamBuffering = true;
			request.ContentType = "multipart/mixed; boundary=" + batchId;
			request.Headers.Add("x-ms-version", "2009-04-14");
			storageAccount.Credentials.SignRequestLite(request);

			StreamWriter writer = new StreamWriter(request.GetRequestStream());

			writer.WriteLine("--" + batchId);
			writer.WriteLine("Content-Type: multipart/mixed; boundary=" + changesetId);
			writer.WriteLine();

			for (int i = 0; i < rows.Count; ++i)
			{
				string url = getUrl(rows[i]);

				string xml;

				using (MemoryStream stream = new MemoryStream())
				{
					XDocument document = GetDocument(rows[i], includeId, url);

					document.Save(stream);

					stream.Position = 0;

					using (StreamReader reader = new StreamReader(stream, true))
						xml = reader.ReadToEnd();
				}

				writer.WriteLine("--" + changesetId);
				writer.WriteLine("Content-Type: application/http");
				writer.WriteLine("Content-Transfer-Encoding: binary");
				writer.WriteLine();
				writer.WriteLine(method + " " + url + " HTTP/1.1");
				writer.WriteLine("Content-ID: " + (i + 1));
				writer.WriteLine("Content-Type: application/atom+xml;type=entry");
				foreach (Func<AzureTablesObject, string> getHeader in additionalHeaders)
					writer.WriteLine(getHeader(rows[i]));
				writer.WriteLine("Content-Length: " + xml.Length);
				writer.WriteLine();
				writer.WriteLine(xml);
			}

			writer.WriteLine("--" + changesetId + "--");
			writer.WriteLine("--" + batchId + "--");

			writer.Flush();
				
			((IDisposable) request.GetResponse()).Dispose();
		}

		private static void ExcludeExpect100ContinueHeader(Uri uri)
		{
			ServicePoint servicePoint = ServicePointManager.FindServicePoint(uri);

			if (servicePoint.Expect100Continue)
				servicePoint.Expect100Continue = false;
		}

		private static XDocument GetDocument(AzureTablesObject row, bool includeId, string id)
		{
			return new XDocument
			(
				new XDeclaration("1.0", "utf-8", "yes"),
				new XElement(AzureNamespaces.Atom + "entry",
					new XAttribute(XNamespace.Xmlns + "d", AzureNamespaces.DataServices),
					new XAttribute(XNamespace.Xmlns + "m", AzureNamespaces.DataServicesMetadata),
					new XAttribute("xmlns", AzureNamespaces.Atom),
					new XElement(AzureNamespaces.Atom + "title"),
					new XElement(AzureNamespaces.Atom + "author",
						new XElement(AzureNamespaces.Atom + "name")),
					new XElement(AzureNamespaces.Atom + "updated", AzureConvert.ToString(DateTime.UtcNow)),
					GetIdElement(includeId, id),
					new XElement(AzureNamespaces.Atom + "content",
						new XAttribute("type", "application/xml"),
						new XElement(AzureNamespaces.DataServicesMetadata + "properties",
							from property in row.Values
							select GetPropertyElement(property.Item1, property.Item2))))
			);
		}

		private static XElement GetIdElement(bool includeId, string id)
		{
			XElement idElement = new XElement(AzureNamespaces.Atom + "id");

			if (includeId)
				idElement.Add(id);

			return idElement;
		}

		private static XElement GetPropertyElement(string name, object value)
		{
			XElement propertyElement = new XElement(AzureNamespaces.DataServices + name);

			XAttribute typeAttribute = GetTypeAttribute(value);

			if (typeAttribute != null)
				propertyElement.Add(typeAttribute);

			propertyElement.Add(AzureConvert.ToString(value));

			return propertyElement;
		}

		private static XAttribute GetTypeAttribute(object value)
		{
			if (value == null)
			{
				return new XAttribute(AzureNamespaces.DataServicesMetadata + "null", "true");
			}
			else
			{
				string edmType = AzureConvert.GetEdmType(value.GetType());

				if (edmType != null)
				{
					return new XAttribute(AzureNamespaces.DataServicesMetadata + "type", edmType);
				}
				else
				{
					string stringValue = AzureConvert.ToString(value);

					if (stringValue.Length == 0)
					{
						return new XAttribute(AzureNamespaces.DataServicesMetadata + "null", "false");
					}
					else
					{
						if (BeginsOrEndsWithWhiteSpace(stringValue))
						{
							return new XAttribute(AzureNamespaces.Atom + "space", "preserve");
						}
						else
						{
							return null;
						}
					}
				}
			}
		}

		private static bool BeginsOrEndsWithWhiteSpace(string stringValue)
		{
			return char.IsWhiteSpace(stringValue[0]) || char.IsWhiteSpace(stringValue[stringValue.Length - 1]);
		}
	}
}
