using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using Microsoft.Rest.Azure.Authentication;

namespace DurableAF.ResourceCreator
{
	public class ServiceCredentialsProvider : IServiceCredentialsProvider
	{
		private readonly AzureApplication m_app;

		public ServiceCredentialsProvider(IConfigProvider configProvider)
		{
			m_app = configProvider.Get<AzureApplication>();
		}

		public async Task<ServiceClientCredentials> GetCredentialsAsync(string tenantId)
		{
			//string authority = "https://login.microsoft.com";
			//AuthenticationContext authContext = new AuthenticationContext(authority, null);
			//authContext.AcquireTokenAsync()
			//var tokenProvider = new ApplicationTokenProvider()

			return await ApplicationTokenProvider.LoginSilentAsync(tenantId, m_app.ClientID, m_app.ClientSecret);
		}
	}
}