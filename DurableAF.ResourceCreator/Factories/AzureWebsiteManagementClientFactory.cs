using Microsoft.Azure.Management.WebSites;
using Microsoft.Rest;

namespace DurableAF.ResourceCreator
{
	public class AzureWebsiteManagementClientFactory : IAzureClientFactory<IWebSiteManagementClient>
	{
		public IWebSiteManagementClient Build(ServiceClientCredentials creds, string subscriptionId)
		{
			return new WebSiteManagementClient(creds)
			{
				SubscriptionId = subscriptionId
			};
		}
	}
}