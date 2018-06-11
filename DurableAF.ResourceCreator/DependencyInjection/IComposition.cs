using Microsoft.Extensions.DependencyInjection;

namespace DurableAF.ResourceCreator
{
	public interface IComposition
	{
		void Load(IServiceCollection services);
	}
}
