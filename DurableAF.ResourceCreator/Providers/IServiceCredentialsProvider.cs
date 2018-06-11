using System.Threading.Tasks;
using Microsoft.Rest;

namespace DurableAF.ResourceCreator
{
	public interface IServiceCredentialsProvider
	{
		Task<ServiceClientCredentials> GetCredentialsAsync(string tenantId);
	}
}