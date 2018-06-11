using System;
using System.Threading.Tasks;

namespace DurableAF.ResourceCreator
{
	public interface IAzureClientProvider<T> where T : IDisposable
	{
		Task<T> Get(string tenantId, string subscriptionId);
	}
}