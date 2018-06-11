using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Management.ResourceManager;
using Microsoft.Azure.Management.ResourceManager.Models;
using Microsoft.Azure.Management.Sql;
using Microsoft.Azure.Management.Sql.Models;
using Microsoft.Azure.Management.Storage;
using Microsoft.Azure.Management.Storage.Models;
using Microsoft.Azure.Management.WebSites;
using Microsoft.Azure.Management.WebSites.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SkuName = Microsoft.Azure.Management.Storage.Models.SkuName;

namespace DurableAF.ResourceCreator
{
	public static partial class ResourceCreationActivities
	{
		public static readonly IServiceProvider Container = new ContainerBuilder()
			.RegisterModule(new ResourceCreatorComposition())
			.Build();

		[FunctionName(nameof(CreateResourceGroup))]
		public static async Task CreateResourceGroup(
			[ActivityTrigger] CreationParameters creationParams,
			ILogger log)
		{
			log.LogWarning("Creating Resource Group: {resourceGroupName}", creationParams.ResourceGroupName);

			var resourceClientProvider = Container.GetService<IAzureClientProvider<IResourceManagementClient>>();
			var resourceClient = await resourceClientProvider.Get(creationParams.TenantID, creationParams.SubscriptionID);

			var rgCreateParams = new ResourceGroup()
			{
				Location = "Australia Southeast",
				Tags = GenerateTags(creationParams)
			};

			ResourceGroup newResourceGroup = await resourceClient.ResourceGroups.CreateOrUpdateAsync(creationParams.ResourceGroupName, rgCreateParams);

			log.LogWarning($"Created Resource Group: {newResourceGroup.Name} in {newResourceGroup.Location}");
		}

		[FunctionName(nameof(RegisterResourceProviders))]
		public static async Task RegisterResourceProviders(
			[ActivityTrigger] CreationParameters creationParams,
			ILogger log)
		{
			log.LogWarning("Registering Resource Providers in Resource Group: {resourceGroupName}", creationParams.ResourceGroupName);

			// simulate hard work in this activity
			await Task.Delay(5000);

			log.LogWarning($"Registered Resource Providers in Resource Group: {creationParams.ResourceGroupName}");
		}

		[FunctionName(nameof(CreateAppServicePlan))]
		public static async Task CreateAppServicePlan(
			[ActivityTrigger] CreationParameters creationParams,
			ILogger log)
		{
			log.LogWarning("Creating App Service Plan in Resource Group: {resourceGroupName}", creationParams.ResourceGroupName);

			var websiteClientProvider = Container.GetService<IAzureClientProvider<IWebSiteManagementClient>>();
			var websiteClient = await websiteClientProvider.Get(creationParams.TenantID, creationParams.SubscriptionID);

			string aspName = creationParams.CustomerShortName + "-asp";

			//"sku": {
			//	"name": "S1",
			//	"tier": "Standard",
			//	"size": "S1",
			//	"family": "S",
			//	"capacity": 1
			//}

			var appServicePlan = new AppServicePlan(maximumNumberOfWorkers: 20, location: creationParams.AzureResourceLocation)
			{
				Sku = new SkuDescription()
				{
					Name = "S1",
					Tier = Microsoft.Azure.Management.WebSites.Models.SkuName.Standard,
					Size = "S1",
					Family = "S",
					Capacity = 1
				},
				AppServicePlanName = aspName,
				PerSiteScaling = false,
				Tags = GenerateTags(creationParams)
			};

			try
			{
				var asp = await websiteClient.AppServicePlans.CreateOrUpdateAsync(creationParams.ResourceGroupName, aspName, appServicePlan);
				log.LogWarning($"Created App Service Plan {asp.AppServicePlanName} in Resource Group: {asp.ResourceGroup}");
			}
			catch (Exception e)
			{
				log.LogError(e, "Unable to create app service plan {name}", aspName);
				throw;
			}
		}

		[FunctionName(nameof(RegisterSslCertificates))]
		public static async Task RegisterSslCertificates(
			[ActivityTrigger] CreationParameters creationParams,
			ILogger log)
		{
			log.LogWarning("Registering Wildcard SSL certificates in Resource Group: {resourceGroupName}", creationParams.ResourceGroupName);

			// simulate hard work in this activity
			await Task.Delay(5000);

			log.LogWarning($"Registed Wildcard SSL certificates in Resource Group: {creationParams.ResourceGroupName}");
		}
		
		[FunctionName(nameof(SetupAwsSes))]
		public static async Task SetupAwsSes(
			[ActivityTrigger] CreationParameters creationParams,
			ILogger log)
		{
			log.LogWarning("Setting up AWS SES in Resource Group: {resourceGroupName}", creationParams.ResourceGroupName);

			// simulate hard work in this activity
			await Task.Delay(5000);

			log.LogWarning($"Set up AWS SES in Resource Group: {creationParams.ResourceGroupName}");
		}

		[FunctionName(nameof(SetupAwsS3Bucket))]
		public static async Task SetupAwsS3Bucket(
			[ActivityTrigger] CreationParameters creationParams,
			ILogger log)
		{
			log.LogWarning("Setting up AWS S3 Bucket in Resource Group: {resourceGroupName}", creationParams.ResourceGroupName);

			// simulate hard work in this activity
			await Task.Delay(5000);

			log.LogWarning($"Set up AWS S3 Bucket in Resource Group: {creationParams.ResourceGroupName}");
		}

		[FunctionName(nameof(CreateWebApp))]
		public static async Task CreateWebApp(
			[ActivityTrigger] CreationParameters creationParams,
			ILogger log)
		{
			log.LogWarning("Creating WebApp in Resource Group: {resourceGroupName}", creationParams.ResourceGroupName);

			var websiteClientProvider = Container.GetService<IAzureClientProvider<IWebSiteManagementClient>>();
			var websiteClient = await websiteClientProvider.Get(creationParams.TenantID, creationParams.SubscriptionID);

			// simulate hard work in this activity
			await Task.Delay(5000);

			log.LogWarning($"Created WebApp in Resource Group: {creationParams.ResourceGroupName}");
		}

		[FunctionName(nameof(CreateWebAppSlot))]
		public static async Task CreateWebAppSlot(
			[ActivityTrigger] CreationParameters creationParams,
			ILogger log)
		{
			log.LogWarning("Creating WebApp Slot in Resource Group: {resourceGroupName}", creationParams.ResourceGroupName);

			var websiteClientProvider = Container.GetService<IAzureClientProvider<IWebSiteManagementClient>>();
			var websiteClient = await websiteClientProvider.Get(creationParams.TenantID, creationParams.SubscriptionID);

			// simulate hard work in this activity
			await Task.Delay(5000);

			log.LogWarning($"Created WebApp Slot in Resource Group: {creationParams.ResourceGroupName}");
		}

		[FunctionName(nameof(SwapWebAppSlot))]
		public static async Task SwapWebAppSlot(
			[ActivityTrigger] CreationParameters creationParams,
			ILogger log)
		{
			log.LogWarning("Swapping WebApp Slot in Resource Group: {resourceGroupName}", creationParams.ResourceGroupName);

			var websiteClientProvider = Container.GetService<IAzureClientProvider<IWebSiteManagementClient>>();
			var websiteClient = await websiteClientProvider.Get(creationParams.TenantID, creationParams.SubscriptionID);

			// simulate hard work in this activity
			await Task.Delay(5000);

			log.LogWarning($"Swapping WebApp Slot in Resource Group: {creationParams.ResourceGroupName}");
		}


		[FunctionName(nameof(UploadDeployWebAppArtifact))]
		public static async Task UploadDeployWebAppArtifact(
			[ActivityTrigger] CreationParameters creationParams,
			ILogger log)
		{
			log.LogWarning("Deploying WebApp Artifact in Resource Group: {resourceGroupName}", creationParams.ResourceGroupName);

			// simulate hard work in this activity
			await Task.Delay(5000);

			log.LogWarning($"Deploying WebApp Artifact in Resource Group: {creationParams.ResourceGroupName}");
		}

		[FunctionName(nameof(UploadDeployWebJobArtifact))]
		public static async Task UploadDeployWebJobArtifact(
			[ActivityTrigger] CreationParameters creationParams,
			ILogger log)
		{
			log.LogWarning("Deploying WebJob Artifact in Resource Group: {resourceGroupName}", creationParams.ResourceGroupName);

			// simulate hard work in this activity
			await Task.Delay(5000);

			log.LogWarning($"Deploying WebJob Artifact in Resource Group: {creationParams.ResourceGroupName}");
		}

		[FunctionName(nameof(CreateStorageAccount))]
		public static async Task<CreateStorageAccountResult> CreateStorageAccount(
			[ActivityTrigger] CreateStorageAccountRequest storageParams,
			ILogger log)
		{
			log.LogWarning("Creating Storage Account in Resource Group: {resourceGroupName}", storageParams.ResourceGroupName);

			var storageClientProvider = Container.GetService<IAzureClientProvider<IStorageManagementClient>>();
			var storageClient = await storageClientProvider.Get(storageParams.TenantID, storageParams.SubscriptionID);

			var saParams = new StorageAccountCreateParameters
			{
				Sku = new Microsoft.Azure.Management.Storage.Models.Sku(Microsoft.Azure.Management.Storage.Models.SkuName.StandardGRS),
				Location = storageParams.AzureResourceLocation,
				Kind = Kind.Storage,
				Encryption = new Encryption("Microsoft.Storage", new EncryptionServices(blob: new EncryptionService(enabled: true))),
				Tags = GenerateTags(storageParams)
			};
			
			StorageAccount sa = await storageClient.StorageAccounts.CreateAsync(storageParams.ResourceGroupName, storageParams.StorageAccountName, saParams);

			log.LogWarning($"Created Storage Account {sa.Name} ({sa.Sku.Name}) in Resource Group: {storageParams.ResourceGroupName}");

			StorageAccountListKeysResult keys = await storageClient.StorageAccounts.ListKeysAsync(storageParams.ResourceGroupName, storageParams.StorageAccountName);
			
			return new CreateStorageAccountResult()
			{
				StorageAccountName = storageParams.StorageAccountName,
				PrimaryKey = keys.Keys.FirstOrDefault(),
				BlobUrl = sa.PrimaryEndpoints.Blob
			};
		}

		[FunctionName(nameof(CreateSqlServer))]
		public static async Task CreateSqlServer(
			[ActivityTrigger] CreationParameters creationParams,
			ILogger log)
		{
			log.LogWarning("Creating SQL Server in Resource Group: {resourceGroupName}", creationParams.ResourceGroupName);

			var sqlClientProvider = Container.GetService<IAzureClientProvider<ISqlManagementClient>>();
			var sqlClient = await sqlClientProvider.Get(creationParams.TenantID, creationParams.SubscriptionID);

			string serverName = GenerateSqlServerName(creationParams);

			var parameters = new Server()
			{
				Location = creationParams.AzureResourceLocation,
				AdministratorLogin = "sa-" + Guid.NewGuid().ToString("N").Substring(8).ToLower(),
				AdministratorLoginPassword = "@!" + Guid.NewGuid().ToString("N").Substring(12).ToLower(),
				Tags = GenerateTags(creationParams)
			};

			Server sqlServer = await sqlClient.Servers.CreateOrUpdateAsync(creationParams.ResourceGroupName, serverName, parameters);
			
			log.LogWarning($"Created SQL Server {sqlServer.FullyQualifiedDomainName} in Resource Group: {creationParams.ResourceGroupName}");
		}

		[FunctionName(nameof(CreateSqlDatabase))]
		public static async Task CreateSqlDatabase(
			[ActivityTrigger] CreationParameters creationParams,
			ILogger log)
		{
			log.LogWarning("Creating SQL Database in Resource Group: {resourceGroupName}", creationParams.ResourceGroupName);

			var sqlClientProvider = Container.GetService<IAzureClientProvider<ISqlManagementClient>>();
			var sqlClient = await sqlClientProvider.Get(creationParams.TenantID, creationParams.SubscriptionID);

			string serverName = GenerateSqlServerName(creationParams);
			string databaseName = GenerateSqlDatabaseName(creationParams);

			//"name": "Standard",
			//"tier": "Standard",
			//"capacity": 10

			var parameters = new Database()
			{
				Location = creationParams.AzureResourceLocation,
				Sku = new Microsoft.Azure.Management.Sql.Models.Sku()
				{
					Name = "Standard",
					//Size = "",
					//Family = "",
					Capacity = 10,
					Tier = "Standard"
				},
				Tags = GenerateTags(creationParams)
			};

			Database db = await sqlClient.Databases.CreateOrUpdateAsync(creationParams.ResourceGroupName, serverName, databaseName, parameters);

			log.LogWarning($"Created SQL Database {db.Name} ({db.Edition} {db.CurrentServiceObjectiveName}) in Resource Group: {creationParams.ResourceGroupName}");
		}

		public static string GenerateStorageAccountName(CreationParameters creationParams, string postfix)
		{
			string env = creationParams.Environment.Substring(0, 4);
			if (env.StartsWith("Dev"))
			{
				env = env.Substring(0, 3);
			}
			string entropy = Guid.NewGuid().ToString("N").Substring(0, 4);
			string name = creationParams.CustomerShortName + env + postfix + entropy;
			return name.Length <= 24 ? name : name.Substring(0, 24).ToLower();
		}

		public static string GenerateSqlServerName(CreationParameters creationParams)
		{
			return ("sql-" + creationParams.CustomerShortName + "-" + creationParams.Environment).ToLower();
		}

		public static string GenerateSqlDatabaseName(CreationParameters creationParams)
		{
			return ("sqldb-" + creationParams.CustomerShortName + "-" + creationParams.Environment).ToLower();
		}

		public static Dictionary<string, string> GenerateTags(CreationParameters parameters)
		{
			return new Dictionary<string, string>()
			{
				["CreationDate"] = DateTimeOffset.UtcNow.ToString(),
				["CreatedBy"] = parameters.CreatorName,
				["Customer"] = parameters.CustomerShortName
			};
		}
	}
}