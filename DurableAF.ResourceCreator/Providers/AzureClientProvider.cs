using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace DurableAF.ResourceCreator
{
	public class AzureClientProvider<T> : IAzureClientProvider<T>, IDisposable where T : IDisposable
	{
		private readonly IServiceCredentialsProvider m_credsProvider;
		private readonly IAzureClientFactory<T> m_factory;
		private readonly MemoryCache m_cache = new MemoryCache(new MemoryCacheOptions(){});
		//private T m_client;

		public AzureClientProvider(IServiceCredentialsProvider credsProvider, IAzureClientFactory<T> factory)
		{
			m_credsProvider = credsProvider;
			m_factory = factory;
		}

		public async Task<T> Get(string tenantId, string subscriptionId)
		{
			//if (m_client == null)
			//{
			//	m_client = m_factory.Build(await m_credsProvider.GetCredentialsAsync(tenantId), subscriptionId);
			//}

			//return m_client;

			// TODO: Somehow take care of expired credentials -- when creds expire, then create a whole new client
			ResourceManagerClientCacheKey key = new ResourceManagerClientCacheKey(tenantId, subscriptionId);

			if (!m_cache.TryGetValue(key, out T client))
			{
				client = m_factory.Build(await m_credsProvider.GetCredentialsAsync(tenantId), subscriptionId);
				m_cache.Set(key, client);
			}

			return client;
		}

		public void Dispose()
		{
			m_cache?.Dispose();
			//m_client?.Dispose();
		}
	}
}