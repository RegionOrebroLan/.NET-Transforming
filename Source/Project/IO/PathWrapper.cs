using RegionOrebroLan.Transforming.IO.Extensions;

namespace RegionOrebroLan.Transforming.IO
{
	public class PathWrapper : IPath
	{
		#region Properties

		public virtual char AltDirectorySeparatorChar => Path.AltDirectorySeparatorChar;
		public virtual char DirectorySeparatorChar => Path.DirectorySeparatorChar;

		#endregion

		#region Methods

		public virtual string Combine(params string[] paths)
		{
			return Path.Combine(paths);
		}

		public virtual string GetDirectoryName(string path)
		{
			return Path.GetDirectoryName(path);
		}

		public virtual string GetExtension(string path)
		{
			return Path.GetExtension(path);
		}

		public virtual string GetTempPath()
		{
			return Path.GetTempPath();
		}

		public virtual bool IsPathFullyQualified(string path)
		{
			return PathExtension.IsPathFullyQualified(path);
		}

		public virtual bool IsPathRooted(string path)
		{
			return Path.IsPathRooted(path);
		}

		#endregion
	}
}