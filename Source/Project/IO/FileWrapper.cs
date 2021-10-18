using System.IO;

namespace RegionOrebroLan.Transforming.IO
{
	public class FileWrapper : IFile
	{
		#region Methods

		public virtual void Copy(string sourcePath, string destinationPath, bool overwrite)
		{
			File.Copy(sourcePath, destinationPath, overwrite);
		}

		public virtual void Delete(string path)
		{
			File.Delete(path);
		}

		public virtual bool Exists(string path)
		{
			return File.Exists(path);
		}

		#endregion
	}
}