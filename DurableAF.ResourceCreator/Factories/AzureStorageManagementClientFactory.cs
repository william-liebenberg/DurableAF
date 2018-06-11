using Microsoft.Azure.Management.Storage;
using Microsoft.Rest;

namespace DurableAF.ResourceCreator
{
	public class AzureStorageManagementClientFactory : IAzureClientFactory<IStorageManagementClient>
	{
		public IStorageManagementClient Build(ServiceClientCredentials creds, string subscriptionId)
		{
			return new StorageManagementClient(creds)
			{
				SubscriptionId = subscriptionId
			};
		}
	}
}