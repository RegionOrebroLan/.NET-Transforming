using System.Globalization;
using System.Xml;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RegionOrebroLan.Transforming.Configuration;
using RegionOrebroLan.Transforming.IO;

namespace RegionOrebroLan.Transforming
{
	public class FileTransformerFactory : IFileTransformerFactory
	{
		#region Constructors

		public FileTransformerFactory(IFileSystem fileSystem, ILoggerFactory loggerFactory, IOptionsMonitor<TransformingOptions> optionsMonitor)
		{
			this.FileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
			this.LoggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
			this.Logger = loggerFactory.CreateLogger(this.GetType());
			this.OptionsMonitor = optionsMonitor ?? throw new ArgumentNullException(nameof(optionsMonitor));
		}

		#endregion

		#region Properties

		protected internal virtual IFileSystem FileSystem { get; }
		protected internal virtual ILogger Logger { get; }
		protected internal virtual ILoggerFactory LoggerFactory { get; }
		protected internal virtual IOptionsMonitor<TransformingOptions> OptionsMonitor { get; }

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
				return new JsonTransformer(this.FileSystem, this.LoggerFactory, this.OptionsMonitor);

			if(this.IsXmlFile(source))
				return new XmlTransformer(this.FileSystem, this.LoggerFactory, this.OptionsMonitor);

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