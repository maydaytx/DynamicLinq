using System.Xml.Linq;

namespace DynamicLinq.AzureTables
{
	internal static class AzureNamespaces
	{
		internal static readonly XNamespace Atom = "http://www.w3.org/2005/Atom";
		internal static readonly XNamespace DataServices = "http://schemas.microsoft.com/ado/2007/08/dataservices";
		internal static readonly XNamespace DataServicesMetadata = "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata";
	}
}