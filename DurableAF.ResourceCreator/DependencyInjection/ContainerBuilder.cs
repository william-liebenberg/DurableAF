using System;
using Microsoft.Extensions.DependencyInjection;

namespace DurableAF.ResourceCreator
{
	public class ContainerBuilder : IContainerBuilder
	{
		private readonly IServiceCollection m_services;

		public ContainerBuilder()
		{
			this.m_services = new ServiceCollection();
		}

		public IContainerBuilder RegisterModule(IComposition composition = null)
		{
			if (composition == null)
			{
				composition = new Composition();
			}

			composition.Load(this.m_services);

			return this;
		}

		public IServiceProvider Build()
		{
			var provider = this.m_services.BuildServiceProvider();

			return provider;
		}
	}
}
