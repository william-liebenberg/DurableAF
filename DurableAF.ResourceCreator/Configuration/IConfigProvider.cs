namespace DurableAF.ResourceCreator
{
	public interface IConfigProvider
	{
		string Get(string settingName);
		TSection Get<TSection>() where TSection : class, new();
		TScopedValue GetScoped<TScopedValue>() where TScopedValue : class, new();
	}
}