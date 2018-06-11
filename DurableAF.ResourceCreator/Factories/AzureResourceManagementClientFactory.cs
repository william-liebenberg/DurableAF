using Microsoft.Azure.Management.ResourceManager;
using Microsoft.Rest;

namespace DurableAF.ResourceCreator
{
	public class AzureResourceManagementClientFactory : IAzureClientFactory<IResourceManagementClient>
	{
		public IResourceManagementClient Build(ServiceClientCredentials creds, string subscriptionId)
		{
			return new ResourceManagementClient(creds)
			{
				SubscriptionId = subscriptionId
			};
		}
	}
}