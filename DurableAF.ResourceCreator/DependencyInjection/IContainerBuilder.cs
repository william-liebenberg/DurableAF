using System;

namespace DurableAF.ResourceCreator
{
	public interface IContainerBuilder
	{
		IContainerBuilder RegisterModule(IComposition composition = null);

		IServiceProvider Build();
	}
}
