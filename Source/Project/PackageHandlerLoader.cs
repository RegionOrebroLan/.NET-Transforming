using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Abstractions;

namespace RegionOrebroLan.Transforming
{
	public class PackageHandlerLoader : IPackageHandlerLoader
	{
		#region Fields

		private IDictionary<string, IPackageHandler> _handlers;

		#endregion

		#region Constructors

		public PackageHandlerLoader(IFileSystem fileSystem)
		{
			this.FileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
		}

		#endregion

		#region Properties

		protected internal virtual IFileSystem FileSystem { get; }

		protected internal virtual IDictionary<string, IPackageHandler> Handlers
		{
			get
			{
				// ReSharper disable InvertIf
				if(this._handlers == null)
				{
					lock(this.HandlersLock)
					{
						if(this._handlers == null)
						{
							this._handlers = new Dictionary<string, IPackageHandler>(StringComparer.OrdinalIgnoreCase)
							{
								{ string.Empty, new DirectoryHandler(this.FileSystem) },
								{ "zip", new ZipFileHandler(this.FileSystem) }
							};
						}
					}
				}
				// ReSharper restore InvertIf

				return this._handlers;
			}
		}

		protected internal virtual object HandlersLock { get; } = new object();

		#endregion

		#region Methods

		public virtual IPackageExtractor GetExtractor(string source)
		{
			return this.GetHandler(source, "source", "package-extractor");
		}

		protected internal virtual IPackageHandler GetHandler(string path, string pathType, string handlerType)
		{
			var extension = this.FileSystem.Path.GetExtension(path).TrimStart('.');

			if(this.Handlers.TryGetValue(extension, out var packageHandler))
				return packageHandler;

			if(!this.FileSystem.File.Exists(path))
				return this.Handlers[string.Empty];

			throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Could not get {0} for {1} \"{2}\".", handlerType, pathType, path));
		}

		public virtual IPackageWriter GetWriter(string destination)
		{
			return this.GetHandler(destination, "destination", "package-writer");
		}

		#endregion
	}
}