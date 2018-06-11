using System;
using Microsoft.Rest;

namespace DurableAF.ResourceCreator
{
	public interface IAzureClientFactory<out T> where T : IDisposable
	{
		T Build(ServiceClientCredentials creds, string subscriptionId);
	}
}