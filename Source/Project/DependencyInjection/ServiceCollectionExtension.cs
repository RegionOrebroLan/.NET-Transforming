using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RegionOrebroLan.Transforming.Configuration;
using RegionOrebroLan.Transforming.IO;

namespace RegionOrebroLan.Transforming.DependencyInjection
{
	public static class ServiceCollectionExtension
	{
		#region Methods

		public static IServiceCollection AddTransforming(this IServiceCollection services)
		{
			if(services == null)
				throw new ArgumentNullException(nameof(services));

			services.Configure<TransformingOptions>(_ => { });

			services.AddTransformingInternal();

			return services;
		}

		public static IServiceCollection AddTransforming(this IServiceCollection services, Action<TransformingOptions> configureOptions)
		{
			if(services == null)
				throw new ArgumentNullException(nameof(services));

			services.Configure(configureOptions);

			services.AddTransformingInternal();

			return services;
		}

		public static IServiceCollection AddTransforming(this IServiceCollection services, IConfiguration configuration, string configurationPath = ConfigurationKeys.TransformingPath)
		{
			if(services == null)
				throw new ArgumentNullException(nameof(services));

			if(configuration == null)
				throw new ArgumentNullException(nameof(configuration));

			services.Configure<TransformingOptions>(configuration.GetSection(configurationPath));

			services.AddTransformingInternal();

			return services;
		}

		private static void AddTransformingInternal(this IServiceCollection services)
		{
			if(services == null)
				throw new ArgumentNullException(nameof(services));

			services.AddSingleton<IFileSearcher, FileSearcher>();
			services.AddSingleton<IFileSystem, FileSystem>();
			services.AddSingleton<IFileTransformerFactory, FileTransformerFactory>();
			services.AddSingleton<IPackageHandlerLoader, PackageHandlerLoader>();
			services.AddSingleton<IPackageTransformer, PackageTransformer>();
		}

		#endregion
	}
}