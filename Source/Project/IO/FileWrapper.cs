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

		public virtual string ReadAllText(string path)
		{
			return File.ReadAllText(path);
		}

		public virtual void WriteAllText(string path, string content)
		{
			File.WriteAllText(path, content);
		}

		#endregion
	}
}