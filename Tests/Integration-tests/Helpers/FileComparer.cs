using System.Globalization;

namespace IntegrationTests.Helpers
{
	public class FileComparer : IEqualityComparer<string>
	{
		#region Constructors

		public FileComparer(string actualDirectoryPath, string expectedDirectoryPath)
		{
			if(string.IsNullOrWhiteSpace(actualDirectoryPath))
				throw new ArgumentException("The actual-directory-path can not be null or whitespace.", nameof(actualDirectoryPath));

			if(!Directory.Exists(actualDirectoryPath))
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "The actual-directory \"{0}\" does not exist.", actualDirectoryPath), nameof(actualDirectoryPath));

			if(string.IsNullOrWhiteSpace(expectedDirectoryPath))
				throw new ArgumentException("The expected-directory-path can not be null or whitespace.", nameof(expectedDirectoryPath));

			if(!Directory.Exists(expectedDirectoryPath))
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "The expected-directory \"{0}\" does not exist.", expectedDirectoryPath), nameof(expectedDirectoryPath));

			this.ActualDirectoryPath = actualDirectoryPath;
			this.ExpectedDirectoryPath = expectedDirectoryPath;
		}

		#endregion

		#region Properties

		protected internal virtual string ActualDirectoryPath { get; }
		protected internal virtual string ExpectedDirectoryPath { get; }

		#endregion

		#region Methods

		public virtual bool Equals(string x, string y)
		{
			var firstRelativePath = this.GetRelativePath(x);
			var secondRelativePath = this.GetRelativePath(y);

			if(!string.Equals(firstRelativePath, secondRelativePath, StringComparison.OrdinalIgnoreCase))
				return false;

			// ReSharper disable AssignNullToNotNullAttribute

			var firstAttributes = File.GetAttributes(x);
			var secondAttributes = File.GetAttributes(y);

			if(firstAttributes != secondAttributes)
				return false;

			// ReSharper disable InvertIf
			if((firstAttributes & FileAttributes.Directory) != FileAttributes.Directory && (secondAttributes & FileAttributes.Directory) != FileAttributes.Directory)
			{
				if(!string.Equals(File.ReadAllText(x), File.ReadAllText(y), StringComparison.Ordinal))
					return false;
			}
			// ReSharper restore InvertIf

			// ReSharper restore AssignNullToNotNullAttribute

			return true;
		}

		public virtual int GetHashCode(string obj)
		{
			if(obj == null)
				throw new ArgumentNullException(nameof(obj));

			return obj.ToUpperInvariant().GetHashCode();
		}

		protected internal virtual string GetRelativePath(string path)
		{
			return path?.ToUpperInvariant().Replace(this.ActualDirectoryPath.ToUpperInvariant(), string.Empty).Replace(this.ExpectedDirectoryPath.ToUpperInvariant(), string.Empty);
		}

		#endregion
	}
}