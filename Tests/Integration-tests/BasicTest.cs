using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntegrationTests
{
	public abstract class BasicTest
	{
		#region Fields

		private DirectoryInfo _outputDirectory;
		private static readonly string _outputDirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Transform-output");
		private static readonly DirectoryInfo _projectDirectory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent?.Parent?.Parent;
		private DirectoryInfo _testResourceDirectory;

		#endregion

		#region Properties

		protected internal virtual DirectoryInfo OutputDirectory
		{
			get
			{
				if(this._outputDirectory == null)
					this._outputDirectory = new DirectoryInfo(this.OutputDirectoryPath);

				this._outputDirectory.Refresh();

				return this._outputDirectory;
			}
		}

		protected internal virtual string OutputDirectoryPath => _outputDirectoryPath;
		protected internal virtual DirectoryInfo ProjectDirectory => _projectDirectory;
		protected internal abstract string ProjectRelativeTestResourceDirectoryPath { get; }
		protected internal virtual DirectoryInfo TestResourceDirectory => this._testResourceDirectory ?? (this._testResourceDirectory = new DirectoryInfo(Path.Combine(this.ProjectDirectory.FullName, this.ProjectRelativeTestResourceDirectoryPath)));

		#endregion

		#region Methods

		[ClassCleanup]
		public static void Cleanup()
		{
			DeleteDirectoryIfItExists(_outputDirectoryPath);
		}

		private static void DeleteDirectoryIfItExists(string directoryPath)
		{
			if(Directory.Exists(directoryPath))
				Directory.Delete(directoryPath, true);
		}

		protected internal virtual string GetOutputPath(string fileSystemEntryName)
		{
			return Path.Combine(this.OutputDirectory.FullName, fileSystemEntryName);
		}

		protected internal virtual string GetTestResourcePath(string fileSystemEntryName)
		{
			return Path.Combine(this.TestResourceDirectory.FullName, fileSystemEntryName);
		}

		[ClassInitialize]
		public static void Initialize(TestContext testContext)
		{
			if(testContext == null)
				throw new ArgumentNullException(nameof(testContext));

			DeleteDirectoryIfItExists(_outputDirectoryPath);
		}

		[TestInitialize]
		public void InitializeTest()
		{
			DeleteDirectoryIfItExists(this.OutputDirectoryPath);
		}

		protected internal virtual void ValidateArgumentException<T>(Action action, string parameterName) where T : ArgumentException
		{
			if(action == null)
				throw new ArgumentNullException(nameof(action));

			try
			{
				action.Invoke();
			}
			catch(T argumentException)
			{
				if(string.Equals(argumentException.ParamName, parameterName, StringComparison.Ordinal))
					throw;
			}
		}

		protected internal virtual void ValidateDestinationParameterException<T>(Action action) where T : ArgumentException
		{
			this.ValidateArgumentException<T>(action, "destination");
		}

		protected internal virtual void ValidateSourceParameterException<T>(Action action) where T : ArgumentException
		{
			this.ValidateArgumentException<T>(action, "source");
		}

		protected internal virtual void ValidateTransformationParameterException<T>(Action action) where T : ArgumentException
		{
			this.ValidateArgumentException<T>(action, "transformation");
		}

		#endregion
	}
}