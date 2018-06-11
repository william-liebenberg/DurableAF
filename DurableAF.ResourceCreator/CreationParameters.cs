namespace DurableAF.ResourceCreator
{
	public class CreationParameters
	{
		public string AzureResourceLocation { get; set; } = "Australia Southeast";
		public string CreatorName { get; set; }
		public string TenantID { get; set; }
		public string SubscriptionID { get; set; }
		public string ResourceGroupName { get; set; }
		public string CustomerShortName { get; set; }
		public string Environment { get; set; }
	}
}
