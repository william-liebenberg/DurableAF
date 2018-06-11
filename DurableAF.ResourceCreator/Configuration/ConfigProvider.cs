using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace DurableAF.ResourceCreator
{
	public class ConfigProvider : IConfigProvider
	{
		private readonly IConfiguration m_configuration;

		public ConfigProvider()
		{
			string configPath = Directory.GetCurrentDirectory();
			m_configuration = new ConfigurationBuilder()
				.SetBasePath(configPath)
				.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
				.AddEnvironmentVariables()
				.Build();
		}

		public string Get(string settingName)
		{
			return m_configuration[settingName];
		}

		public TSection Get<TSection>() where TSection : class, new()
		{
			IConfigurationSection section = m_configuration.GetSection(typeof(TSection).Name);
			PropertyInfo[] setProperties = typeof(TSection).GetProperties();
			if (setProperties.Length == 0)
			{
				return default(TSection);
			}

			TSection instance = new TSection();

			foreach (PropertyInfo prop in setProperties)
			{
				string val = section[prop.Name];
				object conv = Convert.ChangeType(val, prop.PropertyType);
				prop.SetValue(instance, conv);
			}

			return instance;
		}

		public TScopedValue GetScoped<TScopedValue>() where TScopedValue : class, new()
		{
			string typename = typeof(TScopedValue).Name;
			PropertyInfo[] setProperties = typeof(TScopedValue).GetProperties();
			if (setProperties.Length == 0)
			{
				return default(TScopedValue);
			}

			TScopedValue instance = new TScopedValue();

			foreach (PropertyInfo prop in setProperties)
			{
				string val = m_configuration[typename + ":" + prop.Name];
				object conv = Convert.ChangeType(val, prop.PropertyType);
				prop.SetValue(instance, conv);
			}

			return instance;
		}
	}
}