# .NET-Transforming
Library for json-, package- and xml-transformation. The package-transformer can handle directory- and zipfile-packages.

[![NuGet](https://img.shields.io/nuget/v/RegionOrebroLan.Transforming.svg?label=NuGet)](https://www.nuget.org/packages/RegionOrebroLan.Transforming)

## 1 Transforming

### 1.1 File-transforming

#### 1.1.1 JSON-transformer

	var destination = @"C:\Data\Transforming\Destination\AppSettings.json";
	var source = @"C:\Data\Transforming\Sources\AppSettings.json";
	var transformation = @"C:\Data\Transforming\Sources\AppSettings.Release.json";

	// You can register IFileSystem and IFileTransformerFactory in your preferred IoC container.
	var fileTransformerFactory = new FileTransformerFactory(new FileSystem());

	fileTransformerFactory.Create(source).Transform(destination, source, transformation);

#### 1.1.2 XML-transformer

	var destination = @"C:\Data\Transforming\Destination\Web.config";
	var source = @"C:\Data\Transforming\Sources\Web.config";
	var transformation = @"C:\Data\Transforming\Sources\Web.Release.config";

	// You can register IFileSystem and IFileTransformerFactory in your preferred IoC container.
	var fileTransformerFactory = new FileTransformerFactory(new FileSystem());

	fileTransformerFactory.Create(source).Transform(destination, source, transformation);

### 1.2 Package-transforming

#### 1.2.1 Package-transformer

First you set the parameters, see the different examples below. Then:

	// You can register IFileSearcher, IFileSystem, IFileTransformerFactory and IPackageHandlerLoader in your preferred IoC container.
	var fileSystem = new FileSystem();
	var packageTransformer = new PackageTransformer(new FileMatcher(), fileSystem, new FileTransformerFactory(fileSystem), new PackageHandlerLoader(fileSystem));

	// The actual transforming is done under the %temp%-directory. The cleanup-parameter (the first boolean parameter) is for removing the temporary transform-directories or not.
	packageTransformer.Transform(true, destination, fileToTransformPatterns, pathToDeletePatterns, source, transformationNames);

##### 1.2.1.1 Patterns

For handling patterns the Microsoft.Extensions.FileSystemGlobbing.Matcher class is used under the hood.

- fileToTransformPatterns
- pathToDeletePatterns

Examples: https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.filesystemglobbing.matcher?view=dotnet-plat-ext-5.0#remarks

NuGet: https://www.nuget.org/packages/Microsoft.Extensions.FileSystemGlobbing

Patterns with absolute paths does not result in any matches.

##### 1.2.1.2 Directory to directory

	var destination = @"C:\Data\Transforming\Destination\Package";
	var fileToTransformPatterns = new[] {"**/*.config*", "**/*.json", "**/*.xml"};
	var pathToDeletePatterns = new[] {"**/Directory-To-Delete/**", @"**/File-To-Delete.*"};
	var source = @"C:\Data\Transforming\Sources\Package";
	var transformationNames = new[] {"Release", "Test"};

##### 1.2.1.3 Directory to zip-file

	var destination = @"C:\Data\Transforming\Destination\Package.zip";
	var fileToTransformPatterns = new[] {"**/*.config*", "**/*.json", "**/*.xml"};
	var pathToDeletePatterns = new[] {"**/Directory-To-Delete/**", @"**/File-To-Delete.*"};
	var source = @"C:\Data\Transforming\Sources\Package";
	var transformationNames = new[] {"Release", "Test"};

##### 1.2.1.4 Zip-file to zip-file

	var destination = @"C:\Data\Transforming\Destination\Package.zip";
	var fileToTransformPatterns = new[] {"**/*.config*", "**/*.json", "**/*.xml"};
	var pathToDeletePatterns = new[] {"**/Directory-To-Delete/**", @"**/File-To-Delete.*"};
	var source = @"C:\Data\Transforming\Sources\Package.zip";
	var transformationNames = new[] {"Release", "Test"};

##### 1.2.1.5 Zipfile to directory

	var destination = @"C:\Data\Transforming\Destination\Package";
	var fileToTransformPatterns = new[] {"**/*.config*", "**/*.json", "**/*.xml"};
	var pathToDeletePatterns = new[] {"**/Directory-To-Delete/**", @"**/File-To-Delete.*"};
	var source = @"C:\Data\Transforming\Sources\Package.zip";
	var transformationNames = new[] {"Release", "Test"};

## 2 Development

### 2.1 Signing

Drop the "StrongName.snk" file in the repository-root. The file should not be included in source control.