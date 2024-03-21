using System.Globalization;
using System.Xml;
using RegionOrebroLan.Transforming.IO;
using RegionOrebroLan.Transforming.Runtime;

namespace RegionOrebroLan.Transforming
{
	public class FileTransformerFactory(IFileSystem fileSystem, IPlatform platform) : IFileTransformerFactory
	{
		#region Properties

		protected internal virtual IFileSystem FileSystem { get; } = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
		protected internal virtual IPlatform Platform { get; } = platform ?? throw new ArgumentNullException(nameof(platform));

		#endregion

		#region Methods

		public virtual IFileTransformer Create(string source)
		{
			if(source == null)
				throw new ArgumentNullException(nameof(source));

			if(string.IsNullOrWhiteSpace(source))
				throw new ArgumentException("Source can not be empty or whitespace.", nameof(source));

			if(!this.FileSystem.File.Exists(source))
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "The source \"{0}\" does not exist.", source), nameof(source));

			if(this.IsJsonFile(source))
				return new JsonTransformer(this.FileSystem, this.Platform);

			if(this.IsXmlFile(source))
				return new XmlTransformer(this.FileSystem, this.Platform);

			throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "A transformer for file \"{0}\" could not be created.", source));
		}

		protected internal virtual bool IsJsonFile(string filePath)
		{
			// ReSharper disable PossibleNullReferenceException
			return this.IsValidFilePath(filePath) && this.FileSystem.Path.GetExtension(filePath).Equals(".json", StringComparison.OrdinalIgnoreCase);
			// ReSharper restore PossibleNullReferenceException
		}

		protected internal virtual bool IsValidFilePath(string filepath)
		{
			return filepath != null && this.FileSystem.File.Exists(filepath);
		}

		protected internal virtual bool IsXmlFile(string filePath)
		{
			if(!this.IsValidFilePath(filePath))
				return false;

			try
			{
				using(var xmlTextReader = new XmlTextReader(filePath))
				{
					xmlTextReader.DtdProcessing = DtdProcessing.Ignore;
					xmlTextReader.Read();
				}

				return true;
			}
			catch(XmlException)
			{
				return false;
			}
		}

		#endregion
	}
}