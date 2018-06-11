using Microsoft.Extensions.DependencyInjection;

namespace DurableAF.ResourceCreator
{
	public class Composition : IComposition
	{
		public virtual void Load(IServiceCollection services)
		{
			return;
		}
	}
}
