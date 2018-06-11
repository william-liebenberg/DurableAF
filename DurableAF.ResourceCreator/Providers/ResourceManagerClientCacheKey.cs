using System.Collections.Generic;

namespace DurableAF.ResourceCreator
{
	public class ResourceManagerClientCacheKey
	{
		public ResourceManagerClientCacheKey(string tenantId, string subscriptionId)
		{
			TenantId = tenantId;
			SubscriptionId = subscriptionId;
		}

		public string TenantId { get; }
		public string SubscriptionId { get; }

		public override bool Equals(object obj)
		{
			return obj is ResourceManagerClientCacheKey record &&
				   TenantId == record.TenantId &&
				   SubscriptionId == record.SubscriptionId;
		}

		public override int GetHashCode()
		{
			var hashCode = -306047207;
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.TenantId);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.SubscriptionId);
			return hashCode;
		}
	}
}