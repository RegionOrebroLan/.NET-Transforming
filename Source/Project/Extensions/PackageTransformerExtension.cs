using System;
using System.Collections.Generic;

namespace RegionOrebroLan.Transforming.Extensions
{
	public static class PackageTransformerExtension
	{
		#region Methods

		public static void Transform(this IPackageTransformer packageTransformer, string destination, IEnumerable<string> fileToTransformPatterns, IEnumerable<string> pathToDeletePatterns, string source, IEnumerable<string> transformationNames)
		{
			if(packageTransformer == null)
				throw new ArgumentNullException(nameof(packageTransformer));

			packageTransformer.Transform(true, destination, fileToTransformPatterns, pathToDeletePatterns, source, transformationNames);
		}

		#endregion
	}
}