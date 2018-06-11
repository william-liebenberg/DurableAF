using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Rest.Azure;

namespace DurableAF.ResourceCreator
{
	public static class ResourceCreationOrchestrators
	{
		[FunctionName(nameof(StartCreateResources))]
		public static async Task<HttpResponseMessage> StartCreateResources(
			[HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")]HttpRequestMessage req,
			[OrchestrationClient]DurableOrchestrationClient orchestrationClient,
			ExecutionContext executionContext, // not really needed
			ILogger log)
		{
			CreationParameters creationParams = await req.Content.ReadAsAsync<CreationParameters>();

			string instanceId = await orchestrationClient.StartNewAsync(nameof(CreateResourcesOrchestrator), creationParams);
			log.LogWarning("ORC - Started ResourceCreator orchestration with ID = '{instanceId}'.", instanceId);
			return orchestrationClient.CreateCheckStatusResponse(req, instanceId);
		}

		[FunctionName(nameof(CreateResourcesOrchestrator))]
		public static async Task CreateResourcesOrchestrator(
			[OrchestrationTrigger] DurableOrchestrationContextBase context)
		{
			CreationParameters creationParams = context.GetInput<CreationParameters>();

			// customer resources --> 
				// create resource group & register resource providers in subscription
				// create app service plan (prod & dev share the same service plan)
				// register wildcard SSL certificates
				// create storage account for http and application logs from webapps // <shortname>logs<prefix-hash>
				// create blob2sumo webjob
				// create webapp router
				// aws setup
			await context.CallSubOrchestratorAsync(nameof(CreateCustomerResources), creationParams);

			// site resources (sql server, sql db, cache, storageAccount (record attachments), ???)
			await context.CallSubOrchestratorAsync(nameof(CreateSiteResources), creationParams);

			// link resources (all the web apps)
		}

		[FunctionName(nameof(CreateCustomerResources))]
		public static async Task CreateCustomerResources(
			[OrchestrationTrigger] DurableOrchestrationContextBase context,
			ILogger log)
		{
			CreationParameters creationParams = context.GetInput<CreationParameters>();

			if (!context.IsReplaying)
			{
				log.LogWarning("ORC - Creating Customer Resources in: {resourceGroup}", creationParams.ResourceGroupName);
			}

			// Resource Group and Resource Providers
			if (!context.IsReplaying)
			{
				log.LogWarning("ORC - Waiting for Resource Group + Providers");
			}
			await context.CallSubOrchestratorAsync(nameof(CreateCustomerResourceGroup), creationParams);

			// App Serice Plan and Log Storage Account
			if (!context.IsReplaying)
			{
				log.LogWarning("ORC - Waiting for ASP + Storage Account");
			}
			await context.CallSubOrchestratorAsync(nameof(CreateCoreResources), creationParams);

			// WebApp Router + AWS Setup can be done as two tasks running in parallel
			Task[] finalTasks =
			{
				context.CallSubOrchestratorAsync(nameof(CreateWebAppRouter), creationParams),
				context.CallSubOrchestratorAsync(nameof(PerformAwsSetup), creationParams)
			};
			await Task.WhenAll(finalTasks);

			if (!context.IsReplaying)
			{
				log.LogWarning($"ORC - Created Customer Resources in Resource Group: {creationParams.ResourceGroupName}");
			}
		}

		[FunctionName(nameof(CreateSiteResources))]
		public static async Task CreateSiteResources(
			[OrchestrationTrigger] DurableOrchestrationContextBase context,
			ILogger log)
		{
			CreationParameters creationParams = context.GetInput<CreationParameters>();

			if (!context.IsReplaying)
			{
				log.LogWarning("ORC - Creating Site Resources in: {resourceGroup}", creationParams.ResourceGroupName);
			}

			// site resources (sql server, sql db, redis cache, storageAccount (record attachments), ???)
			var storageRequest = CreateStorageAccountRequest.FromCreationParams(creationParams);
			storageRequest.StorageAccountName = ResourceCreationActivities.GenerateStorageAccountName(creationParams, string.Empty);

			var t1 = context.CallSubOrchestratorAsync(nameof(CreateSiteSqlServerAndDatabase), creationParams);
			var t2 = context.CallActivityAsync<CreateStorageAccountResult>(nameof(ResourceCreationActivities.CreateStorageAccount), storageRequest);
			await Task.WhenAll(t1, t2);

			if (!context.IsReplaying)
			{
				log.LogWarning($"ORC - Created Storage Account Name: {t2.Result.StorageAccountName} - Primary Key: {t2.Result.PrimaryKey.KeyName}, {t2.Result.PrimaryKey.Value}"); 
				log.LogWarning($"ORC - Created Site Resources in Resource Group: {creationParams.ResourceGroupName}");
			}
		}
		
		[FunctionName(nameof(CreateSiteSqlServerAndDatabase))]
		public static async Task CreateSiteSqlServerAndDatabase(
			[OrchestrationTrigger] DurableOrchestrationContextBase context,
			ILogger log)
		{
			CreationParameters creationParams = context.GetInput<CreationParameters>();

			if (!context.IsReplaying)
			{
				log.LogWarning("ORC - Creating Sql Server and Database Resources in: {resourceGroupName}", creationParams.ResourceGroupName);
			}

			await context.CallActivityAsync(nameof(ResourceCreationActivities.CreateSqlServer), creationParams);
			await context.CallActivityAsync(nameof(ResourceCreationActivities.CreateSqlDatabase), creationParams);

			if (!context.IsReplaying)
			{
				log.LogWarning($"ORC - Created Sql Server and Database Resources in: {creationParams.ResourceGroupName}");
			}
		}
		
		[FunctionName(nameof(CreateCustomerResourceGroup))]
		public static async Task CreateCustomerResourceGroup(
			[OrchestrationTrigger] DurableOrchestrationContextBase context,
			ILogger log)
		{
			CreationParameters creationParams = context.GetInput<CreationParameters>();

			if (!context.IsReplaying)
			{
				log.LogWarning("ORC - Creating Customer Resource Group: {resourceGroupName}", creationParams.ResourceGroupName);
			}

			await context.CallActivityAsync(nameof(ResourceCreationActivities.CreateResourceGroup), creationParams);
			if (!context.IsReplaying)
			{
				log.LogWarning("ORC - Registering Resource Providers in: {resourceGroupName}", creationParams.ResourceGroupName);
			}

			await context.CallActivityAsync(nameof(ResourceCreationActivities.RegisterResourceProviders), creationParams);

			if (!context.IsReplaying)
			{
				log.LogWarning($"ORC - Created Resource Group & Registered Providers in: {creationParams.ResourceGroupName}");
			}
		}

		[FunctionName(nameof(CreateCoreResources))]
		public static async Task CreateCoreResources(
			[OrchestrationTrigger] DurableOrchestrationContextBase context,
			ILogger log)
		{
			CreationParameters creationParams = context.GetInput<CreationParameters>();

			if (!context.IsReplaying)
			{
				log.LogWarning("ORC - Creating Core Resources in: {resourceGroupName}", creationParams.ResourceGroupName);
			}

			//await context.CallActivityWithRetryAsync(
			//	nameof(ResourceCreationActivities.CreateAppServicePlan),
			//	new RetryOptions(TimeSpan.FromMinutes(1), maxNumberOfAttempts: 3)
			//	{
			//		BackoffCoefficient = 1.0,
			//		Handle = ex =>
			//		{
			//			if (ex is CloudException)
			//			{
			//				log.LogError(ex, "failed to create app service plan...");
			//				return true;
			//			}
			//			// don't bother retrying on any other exceptions
			//			return false;
			//		},
			//		RetryTimeout = TimeSpan.FromMinutes(1)
			//	},
			//	creationParams);
			
			var storageRequest = CreateStorageAccountRequest.FromCreationParams(creationParams);
			storageRequest.StorageAccountName = ResourceCreationActivities.GenerateStorageAccountName(creationParams, "logs");

			var t1 = context.CallSubOrchestratorAsync(nameof(CreateCustomerAppServicePlan), creationParams);
			var t2 = context.CallActivityAsync<CreateStorageAccountResult>(nameof(ResourceCreationActivities.CreateStorageAccount), storageRequest);

			await Task.WhenAll(t1, t2);

			if (!context.IsReplaying)
			{
				log.LogWarning($"ORC - Created Storage Account Name: {t2.Result.StorageAccountName} - Primary Key: {t2.Result.PrimaryKey.KeyName}, {t2.Result.PrimaryKey.Value}"); 
				log.LogWarning($"ORC - Created Core Resources in: {creationParams.ResourceGroupName}");
			}
		}

		[FunctionName(nameof(CreateCustomerAppServicePlan))]
		public static async Task CreateCustomerAppServicePlan(
			[OrchestrationTrigger] DurableOrchestrationContextBase context,
			ILogger log)
		{
			CreationParameters creationParams = context.GetInput<CreationParameters>();

			if (!context.IsReplaying)
			{
				log.LogWarning("ORC - Creating Customer App Service Plan + Wildcard SSL in Resource Group: {resourceGroupName}", creationParams.ResourceGroupName);
			}

			Task[] tasks =
			{
				context.CallActivityAsync(nameof(ResourceCreationActivities.CreateAppServicePlan), creationParams), // do with register ssl
				context.CallActivityAsync(nameof(ResourceCreationActivities.RegisterSslCertificates), creationParams), // do with create asp
			};
			await Task.WhenAll(tasks);

			if (!context.IsReplaying)
			{
				log.LogWarning("ORC - Created Customer App Service Plan + Wildcard SSL in Resource Group: {resourceGroupName}", creationParams.ResourceGroupName);
			}
		}

		[FunctionName(nameof(CreateWebAppRouter))]
		public static async Task CreateWebAppRouter(
			[OrchestrationTrigger] DurableOrchestrationContextBase context,
			ILogger log)
		{
			CreationParameters creationParams = context.GetInput<CreationParameters>();

			if (!context.IsReplaying)
			{
				log.LogWarning("ORC - Creating WebAppRouter in Resource Group: {resourceGroupName}", creationParams.ResourceGroupName);
			}

			await context.CallActivityAsync(nameof(ResourceCreationActivities.CreateWebApp), creationParams);
			await context.CallActivityAsync(nameof(ResourceCreationActivities.CreateWebAppSlot), creationParams);
			await context.CallActivityAsync(nameof(ResourceCreationActivities.UploadDeployWebAppArtifact), creationParams);
			await context.CallActivityAsync(nameof(ResourceCreationActivities.SwapWebAppSlot), creationParams);

			if (!context.IsReplaying)
			{
				log.LogWarning($"ORC - Created WebAppRouter in Resource Group: {creationParams.ResourceGroupName}");
			}
		}

		[FunctionName(nameof(PerformAwsSetup))]
		public static async Task PerformAwsSetup(
			[OrchestrationTrigger] DurableOrchestrationContextBase context,
			ILogger log)
		{
			CreationParameters creationParams = context.GetInput<CreationParameters>();

			if (!context.IsReplaying)
			{
				log.LogWarning("ORC - Setting up AWS in Resource Group: {resourceGroupName}", creationParams.ResourceGroupName);
			}

			Task[] setupAwsTasks = {
				context.CallActivityAsync(nameof(ResourceCreationActivities.SetupAwsSes), creationParams),
				context.CallActivityAsync(nameof(ResourceCreationActivities.SetupAwsS3Bucket), creationParams)
			};
			await Task.WhenAll(setupAwsTasks);

			if (!context.IsReplaying)
			{
				log.LogWarning($"ORC - Set up AWS in Resource Group: {creationParams.ResourceGroupName}");
			}
		}
	}
}
