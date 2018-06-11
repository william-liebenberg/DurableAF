namespace DurableAF.ResourceCreator
{
	public class CreateStorageAccountRequest : CreationParameters
	{
		public string StorageAccountName { get; set; }

		public static CreateStorageAccountRequest FromCreationParams(CreationParameters creationParams)
		{
			return new CreateStorageAccountRequest()
			{
				ResourceGroupName = creationParams.ResourceGroupName,
				AzureResourceLocation = creationParams.AzureResourceLocation,
				CustomerShortName = creationParams.CustomerShortName,
				CreatorName = creationParams.CreatorName,
				Environment = creationParams.Environment,
				SubscriptionID = creationParams.SubscriptionID,
				TenantID = creationParams.TenantID
			};
		}
	}
}