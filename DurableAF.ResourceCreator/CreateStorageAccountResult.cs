using Microsoft.Azure.Management.Storage.Models;

namespace DurableAF.ResourceCreator
{
	public class CreateStorageAccountResult
	{
		public string StorageAccountName { get; set; }
		public StorageAccountKey PrimaryKey { get; set; }
		public string BlobUrl { get; set; }
	}
}