using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RegionOrebroLan.Transforming;
using RegionOrebroLan.Transforming.Configuration;
using RegionOrebroLan.Transforming.IO;

namespace IntegrationTests.Fixtures
{
	/// <summary>
	/// https://xunit.net/docs/shared-context
	/// </summary>
	public class Fixture : IDisposable
	{
		#region Fields

		private FileTransformerFactory _fileTransformerFactory;
		private JsonTransformer _jsonTransformer;
		private static readonly object _outputDirectoryLock = new();
		private string _outputDirectoryPath;
		private PackageTransformer _packageTransformer;
		private XmlTransformer _xmlTransformer;

		#endregion

		#region Properties

		public FileTransformerFactory FileTransformerFactory => this._fileTransformerFactory ??= (FileTransformerFactory)this.ServiceProvider.GetRequiredService<IFileTransformerFactory>();
		public JsonTransformer JsonTransformer => this._jsonTransformer ??= new JsonTransformer(this.ServiceProvider.GetRequiredService<IFileSystem>(), this.ServiceProvider.GetRequiredService<ILoggerFactory>(), this.ServiceProvider.GetRequiredService<IOptionsMonitor<TransformingOptions>>());

		public string OutputDirectoryPath
		{
			get
			{
				if(this._outputDirectoryPath == null)
				{
					lock(_outputDirectoryLock)
					{
						if(this._outputDirectoryPath == null)
						{
							var path = Path.Combine(Global.ProjectDirectory.FullName, "Test-output");

							if(!Directory.Exists(path))
								Directory.CreateDirectory(path);

							this._outputDirectoryPath = path;
						}
					}
				}

				return this._outputDirectoryPath;
			}
		}

		public PackageTransformer PackageTransformer => this._packageTransformer ??= (PackageTransformer)this.ServiceProvider.GetRequiredService<IPackageTransformer>();
		public DirectoryInfo ResourcesDirectory { get; } = new(Path.Combine(Global.ProjectDirectory.FullName, "Resources"));
		public ServiceProvider ServiceProvider { get; } = Global.CreateServices().BuildServiceProvider();
		public XmlTransformer XmlTransformer => this._xmlTransformer ??= new XmlTransformer(this.ServiceProvider.GetRequiredService<IFileSystem>(), this.ServiceProvider.GetRequiredService<ILoggerFactory>(), this.ServiceProvider.GetRequiredService<IOptionsMonitor<TransformingOptions>>());

		#endregion

		#region Methods

		public void Dispose()
		{
			lock(_outputDirectoryLock)
			{
				if(Directory.Exists(this._outputDirectoryPath))
					Directory.Delete(this._outputDirectoryPath, true);
			}

			this.ServiceProvider.Dispose();
			GC.SuppressFinalize(this);
		}

		public string GetOutputPath(params string[] paths)
		{
			return Path.Combine(new[] { this.OutputDirectoryPath }.Concat(paths).ToArray());
		}

		public string GetResourcePath(params string[] paths)
		{
			return Path.Combine(new[] { this.ResourcesDirectory.FullName }.Concat(paths).ToArray());
		}

		#endregion
	}
}