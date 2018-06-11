using Microsoft.Azure.Management.Sql;
using Microsoft.Rest;

namespace DurableAF.ResourceCreator
{
	public class AzureSqlManagementClientFactory : IAzureClientFactory<ISqlManagementClient>
	{
		public ISqlManagementClient Build(ServiceClientCredentials creds, string subscriptionId)
		{
			return new SqlManagementClient(creds)
			{
				SubscriptionId = subscriptionId
			};
		}
	}
}