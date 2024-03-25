using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RegionOrebroLan.Transforming.Configuration;
using RegionOrebroLan.Transforming.DependencyInjection;
using Xunit;

namespace IntegrationTests.DependencyInjection
{
	public class ServiceCollectionExtensionTest
	{
		#region Methods

		[Fact]
		public async Task AddTransforming_Configuration_Test()
		{
			await Task.CompletedTask;

			using(var serviceProvider = CreateServiceProvider("appsettings-1.json"))
			{
				var options = serviceProvider.GetRequiredService<IOptions<TransformingOptions>>();
				Assert.NotNull(options);
				Assert.Equal(!RuntimeInformation.IsOSPlatform(OSPlatform.Windows), options.Value.File.AvoidByteOrderMark);
				Assert.Equal(!RuntimeInformation.IsOSPlatform(OSPlatform.Windows), options.Value.File.Replacement.Enabled);
				Assert.True(options.Value.Package.Cleanup);
			}

			using(var serviceProvider = CreateServiceProvider("appsettings-2.json"))
			{
				var options = serviceProvider.GetRequiredService<IOptions<TransformingOptions>>();
				Assert.NotNull(options);
				Assert.True(options.Value.File.AvoidByteOrderMark);
				Assert.True(options.Value.File.Replacement.Enabled);
				Assert.False(options.Value.Package.Cleanup);
			}

			using(var serviceProvider = CreateServiceProvider("appsettings-3.json", "CustomTransforming"))
			{
				var options = serviceProvider.GetRequiredService<IOptions<TransformingOptions>>();
				Assert.NotNull(options);
				Assert.True(options.Value.File.AvoidByteOrderMark);
				Assert.True(options.Value.File.Replacement.Enabled);
				Assert.False(options.Value.Package.Cleanup);
			}
		}

		private static ServiceProvider CreateServiceProvider(string fileName, string configurationPath = null)
		{
			var configuration = Global.CreateConfiguration(GetPath(fileName));

			var services = new ServiceCollection();

			services.AddSingleton(configuration);
			services.AddSingleton(Global.HostEnvironment);

			if(configurationPath == null)
				services.AddTransforming(configuration);
			else
				services.AddTransforming(configuration, configurationPath);

			return services.BuildServiceProvider();
		}

		private static string GetPath(string fileName)
		{
			return Path.Combine("DependencyInjection", "Resources", "ServiceCollectionExtensionTest", fileName);
		}

		#endregion
	}
}