namespace RegionOrebroLan.Transforming.IO
{
	public interface IFile
	{
		#region Methods

		void Copy(string sourcePath, string destinationPath, bool overwrite);
		void Delete(string path);
		bool Exists(string path);

		#endregion
	}
}