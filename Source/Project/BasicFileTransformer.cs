using System.Globalization;
using RegionOrebroLan.Transforming.IO;
using RegionOrebroLan.Transforming.IO.Extensions;
using RegionOrebroLan.Transforming.Runtime;

namespace RegionOrebroLan.Transforming
{
	public abstract class BasicFileTransformer(IFileSystem fileSystem, IPlatform platform) : IFileTransformer
	{
		#region Properties

		protected internal virtual IFileSystem FileSystem { get; } = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
		protected internal virtual IPlatform Platform { get; } = platform ?? throw new ArgumentNullException(nameof(platform));

		#endregion

		#region Methods

		protected internal virtual bool ResolveAvoidByteOrderMark(bool? avoidByteOrderMark)
		{
			if(avoidByteOrderMark == null)
				return !this.Platform.IsWindows;

			return avoidByteOrderMark.Value;
		}

		public virtual void Transform(string destination, string source, string transformation, bool? avoidByteOrderMark = null)
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
				this.TransformInternal(destination, source, transformation, avoidByteOrderMark);
			}
			catch(Exception exception)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Could not transform source \"{0}\" with transformation \"{1}\" to destination \"{2}\".", source, transformation, destination), exception);
			}
		}

		protected internal abstract void TransformInternal(string destination, string source, string transformation, bool? avoidByteOrderMark = null);

		protected internal virtual bool UseByteOrderMark(bool? avoidByteOrderMark, string source)
		{
			using(var streamReader = new StreamReader(source, true))
			{
				return streamReader.HasByteOrderMark() && !this.ResolveAvoidByteOrderMark(avoidByteOrderMark);
			}
		}

		#endregion
	}
}