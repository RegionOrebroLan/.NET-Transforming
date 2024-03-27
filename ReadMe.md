# .NET-Transforming
Library for json-, package- and xml-transformation. The package-transformer can handle directory- and zipfile-packages.

[![NuGet](https://img.shields.io/nuget/v/RegionOrebroLan.Transforming.svg?label=NuGet)](https://www.nuget.org/packages/RegionOrebroLan.Transforming)

## 1 Transforming

### 1.1 File-transforming

#### 1.1.1 JSON-transformer

	var destination = @"C:\Data\Transforming\Destination\AppSettings.json";
	var source = @"C:\Data\Transforming\Sources\AppSettings.json";
	var transformation = @"C:\Data\Transforming\Sources\AppSettings.Release.json";

	// You can register IFileSystem, IFileTransformerFactory, ILoggerFactory and IPlatform in your preferred IoC container.
	var fileTransformerFactory = new FileTransformerFactory(new FileSystem(), new NullLoggerFactory(), new Platform());

	fileTransformerFactory.Create(source).Transform(destination, source, transformation);

#### 1.1.2 XML-transformer

	var destination = @"C:\Data\Transforming\Destination\Web.config";
	var source = @"C:\Data\Transforming\Sources\Web.config";
	var transformation = @"C:\Data\Transforming\Sources\Web.Release.config";

	// You can register IFileSystem, IFileTransformerFactory, ILoggerFactory and IPlatform in your preferred IoC container.
	var fileTransformerFactory = new FileTransformerFactory(new FileSystem(), new NullLoggerFactory(), new Platform());

	fileTransformerFactory.Create(source).Transform(destination, source, transformation);

### 1.2 Package-transforming

#### 1.2.1 Package-transformer

First you set the parameters, see the different examples below. Then:

	// You can register IFileSearcher, IFileSystem, IFileTransformerFactory, ILoggerFactory, IPackageHandlerLoader and IPlatform in your preferred IoC container.
	var fileSystem = new FileSystem();
	var loggerFactory = new NullLoggerFactory();
	var packageTransformer = new PackageTransformer(new FileMatcher(), fileSystem, new FileTransformerFactory(fileSystem, loggerFactory, new Platform()), loggerFactory, new PackageHandlerLoader(fileSystem));

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

### 1.3 Common

All the transform-methods above also have the parameter **avoidByteOrderMark null/true/false**, *bool? avoidByteOrderMark = null*. The final value for this parameter if not set (null) is:

- On Windows: false
- On other platforms (Linux/MacOS): true

This parameter controls the result of the transformation. If a source file that will be transformed has a BOM (Byte Order Mark) the destination file should also have a BOM. This is not always desired, e.g. on a Linux system. So if the source file has a BOM but the parameter "avoidByteOrderMark" is set to true, the destination file will not have a BOM.

## 2 Development

### 2.1 Signing

Drop the "StrongName.snk" file in the repository-root. The file should not be included in source control.

## 3 Notes

- [TestMethod, TestInitialize, and TestCleanup in XUnit2](https://blog.somewhatabstract.com/2016/11/21/testmethod-testinitialize-and-testcleanup-in-xunit2)
- [Shared Context between Tests](https://xunit.net/docs/shared-context)