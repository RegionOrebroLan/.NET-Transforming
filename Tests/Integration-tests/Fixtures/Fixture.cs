using Microsoft.Extensions.DependencyInjection;
using RegionOrebroLan.Transforming;

namespace IntegrationTests.Fixtures
{
	/// <summary>
	/// https://xunit.net/docs/shared-context#assembly-fixture
	/// </summary>
	public class Fixture : IDisposable
	{
		#region Fields

		private FileTransformerFactory _fileTransformerFactory;
		private PackageTransformer _packageTransformer;

		#endregion

		#region Properties

		public FileTransformerFactory FileTransformerFactory => this._fileTransformerFactory ??= (FileTransformerFactory)this.ServiceProvider.GetRequiredService<IFileTransformerFactory>();
		public PackageTransformer PackageTransformer => this._packageTransformer ??= (PackageTransformer)this.ServiceProvider.GetRequiredService<IPackageTransformer>();
		public ServiceProvider ServiceProvider { get; } = Global.CreateServices().BuildServiceProvider();

		#endregion

		#region Methods

		public void Dispose()
		{
			this.ServiceProvider.Dispose();
			GC.SuppressFinalize(this);
		}

		#endregion
	}
}