using System.Globalization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RegionOrebroLan.Transforming.Configuration;
using RegionOrebroLan.Transforming.IO;

namespace RegionOrebroLan.Transforming
{
	public abstract class BasicFileTransformer : BasicTransformer, IFileTransformer
	{
		#region Constructors

		protected BasicFileTransformer(IFileSystem fileSystem, ILoggerFactory loggerFactory, IOptionsMonitor<TransformingOptions> optionsMonitor)
		{
			this.FileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
			this.Logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger(this.GetType());
			this.OptionsMonitor = optionsMonitor ?? throw new ArgumentNullException(nameof(optionsMonitor));
		}

		#endregion

		#region Properties

		protected internal virtual IFileSystem FileSystem { get; }
		protected internal virtual ILogger Logger { get; }
		protected internal virtual IOptionsMonitor<TransformingOptions> OptionsMonitor { get; }

		#endregion

		#region Methods

		public virtual void Transform(string destination, string source, string transformation, FileTransformingOptions options = null)
		{
			if(destination == null)
				throw new ArgumentNullException(nameof(destination));

			if(string.IsNullOrWhiteSpace(destination))
				throw new ArgumentException("Destination can not be empty or whitespace.", nameof(destination));

			if(source == null)
				throw new ArgumentNullException(nameof(source));

			if(string.IsNullOrWhiteSpace(source))
				throw new ArgumentException("Source can not be empty or whitespace.", nameof(source));

			if(!this.FileSystem.File.Exists(source))
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "The source \"{0}\" does not exist.", source), nameof(source));

			if(transformation == null)
				throw new ArgumentNullException(nameof(transformation));

			if(string.IsNullOrWhiteSpace(transformation))
				throw new ArgumentException("Transformation can not be empty or whitespace.", nameof(transformation));

			if(!this.FileSystem.File.Exists(transformation))
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "The transformation \"{0}\" does not exist.", source), nameof(transformation));

			try
			{
				this.TransformInternal(destination, source, transformation, options ?? this.OptionsMonitor.CurrentValue.File);
			}
			catch(Exception exception)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Could not transform source \"{0}\" with transformation \"{1}\" to destination \"{2}\".", source, transformation, destination), exception);
			}
		}

		protected internal abstract void TransformInternal(string destination, string source, string transformation, FileTransformingOptions options);

		#endregion
	}
}