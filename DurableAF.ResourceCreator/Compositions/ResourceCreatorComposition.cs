using Microsoft.Azure.Management.ResourceManager;
using Microsoft.Azure.Management.Sql;
using Microsoft.Azure.Management.Storage;
using Microsoft.Azure.Management.WebSites;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

namespace DurableAF.ResourceCreator
{
	public class ResourceCreatorComposition : Composition
	{
		public override void Load(IServiceCollection services)
		{
			services.AddSingleton<HttpClient>();

			services.AddSingleton<IConfigProvider, ConfigProvider>();
			services.AddSingleton<IServiceCredentialsProvider, ServiceCredentialsProvider>();
			
			// azure client providers
			services.AddSingleton<IAzureClientProvider<IResourceManagementClient>, AzureClientProvider<IResourceManagementClient>>();
			services.AddSingleton<IAzureClientProvider<IWebSiteManagementClient>, AzureClientProvider<IWebSiteManagementClient>>();
			services.AddSingleton<IAzureClientProvider<IStorageManagementClient>, AzureClientProvider<IStorageManagementClient>>();
			services.AddSingleton<IAzureClientProvider<ISqlManagementClient>, AzureClientProvider<ISqlManagementClient>>();

			// azure client factories
			services.AddSingleton<IAzureClientFactory<IResourceManagementClient>, AzureResourceManagementClientFactory>();
			services.AddSingleton<IAzureClientFactory<IWebSiteManagementClient>, AzureWebsiteManagementClientFactory>();
			services.AddSingleton<IAzureClientFactory<IStorageManagementClient>, AzureStorageManagementClientFactory>();
			services.AddSingleton<IAzureClientFactory<ISqlManagementClient>, AzureSqlManagementClientFactory>();
		}
	}
}