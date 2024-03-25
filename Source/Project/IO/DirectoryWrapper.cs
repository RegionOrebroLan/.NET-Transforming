namespace RegionOrebroLan.Transforming.IO
{
	public class DirectoryWrapper : IDirectory
	{
		#region Methods

		public virtual void CreateDirectory(string path)
		{
			Directory.CreateDirectory(path);
		}

		public virtual void Delete(string path)
		{
			Directory.Delete(path);
		}

		public virtual void Delete(string path, bool recursive)
		{
			Directory.Delete(path, recursive);
		}

		public virtual bool Exists(string path)
		{
			return Directory.Exists(path);
		}

		public virtual IEnumerable<string> GetDirectories(string path)
		{
			return Directory.GetDirectories(path);
		}

		public virtual IEnumerable<string> GetFiles(string path)
		{
			return Directory.GetFiles(path);
		}

		public virtual IEnumerable<string> GetFilesRecursive(string path)
		{
			return Directory.GetFiles(path, "*", SearchOption.AllDirectories);
		}

		#endregion
	}
}