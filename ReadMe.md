# .NET-Transforming

Library for json-, package- and xml-transformation. The package-transformer can handle directory- and zipfile-packages.

[![NuGet](https://img.shields.io/nuget/v/RegionOrebroLan.Transforming.svg?label=NuGet)](https://www.nuget.org/packages/RegionOrebroLan.Transforming)

## 1 Transforming

### 1.1 Configure services

#### 1.1.1 [Default](/Source/Project/DependencyInjection/ServiceCollectionExtension.cs#L12)

	services.AddTransforming();

#### 1.1.2 [Options](/Source/Project/DependencyInjection/ServiceCollectionExtension.cs#L24)

	services.AddTransforming(options =>
	{
		// options.File.AvoidByteOrderMark = true;
	});

#### 1.1.3 [Configuration](/Source/Project/DependencyInjection/ServiceCollectionExtension.cs#L36)

	services.AddTransforming(configuration);

### 1.2 File-transforming

#### 1.2.1 JSON-transformer

	var destination = @"C:\Data\Transforming\Destination\AppSettings.json";
	var source = @"C:\Data\Transforming\Sources\AppSettings.json";
	var transformation = @"C:\Data\Transforming\Sources\AppSettings.Release.json";

	// You get the "fileTransformerFactory" from the service-provider.

	fileTransformerFactory.Create(source).Transform(destination, source, transformation);

#### 1.2.2 XML-transformer

	var destination = @"C:\Data\Transforming\Destination\Web.config";
	var source = @"C:\Data\Transforming\Sources\Web.config";
	var transformation = @"C:\Data\Transforming\Sources\Web.Release.config";

	// You get the "fileTransformerFactory" from the service-provider.

	fileTransformerFactory.Create(source).Transform(destination, source, transformation);

#### 1.2.3 [Additional options](/Source/Project/Configuration/FileTransformingOptions.cs)

	fileTransformerFactory.Create(source).Transform(destination, source, transformation, new FileTransformingOptions
	{
		AvoidByteOrderMark = true,
		Replacement =
		{
			Enabled = true,
			Replace = value =>
			{
				// You can do replacements here.
				return value;
			}
		}
	});

The "AvoidByteOrderMark" property controls the result of the transformation. If a source file that will be transformed has a BOM (Byte Order Mark) the destination file should also have a BOM. This is not always desired, e.g. on a Linux system. So if the source file has a BOM but the property "AvoidByteOrderMark" is set to true, the destination file will not have a BOM.

The default value for the "AvoidByteOrderMark" property:

- On Windows: false
- On other platforms (Linux/MacOS): true

The default values can be configured during service configuration, e.g. in appsettings.json.

### 1.3 Package-transforming

#### 1.3.1 Package-transformer

First you set the parameters, see the different examples below. Then:

	// You get the "packageTransformer" from the service-provider.

	packageTransformer.Transform(destination, fileToTransformPatterns, pathToDeletePatterns, source, transformationNames);

##### 1.3.1.1 Patterns

For handling patterns the Microsoft.Extensions.FileSystemGlobbing.Matcher class is used under the hood.

- fileToTransformPatterns
- pathToDeletePatterns

Examples: https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.filesystemglobbing.matcher?view=dotnet-plat-ext-5.0#remarks

NuGet: https://www.nuget.org/packages/Microsoft.Extensions.FileSystemGlobbing

Patterns with absolute paths does not result in any matches.

##### 1.3.1.2 Directory to directory

	var destination = @"C:\Data\Transforming\Destination\Package";
	var fileToTransformPatterns = new[] {"**/*.config*", "**/*.json", "**/*.xml"};
	var pathToDeletePatterns = new[] {"**/Directory-To-Delete/**", @"**/File-To-Delete.*"};
	var source = @"C:\Data\Transforming\Sources\Package";
	var transformationNames = new[] {"Release", "Test"};

##### 1.3.1.3 Directory to zip-file

	var destination = @"C:\Data\Transforming\Destination\Package.zip";
	var fileToTransformPatterns = new[] {"**/*.config*", "**/*.json", "**/*.xml"};
	var pathToDeletePatterns = new[] {"**/Directory-To-Delete/**", @"**/File-To-Delete.*"};
	var source = @"C:\Data\Transforming\Sources\Package";
	var transformationNames = new[] {"Release", "Test"};

##### 1.3.1.4 Zip-file to zip-file

	var destination = @"C:\Data\Transforming\Destination\Package.zip";
	var fileToTransformPatterns = new[] {"**/*.config*", "**/*.json", "**/*.xml"};
	var pathToDeletePatterns = new[] {"**/Directory-To-Delete/**", @"**/File-To-Delete.*"};
	var source = @"C:\Data\Transforming\Sources\Package.zip";
	var transformationNames = new[] {"Release", "Test"};

##### 1.3.1.5 Zipfile to directory

	var destination = @"C:\Data\Transforming\Destination\Package";
	var fileToTransformPatterns = new[] {"**/*.config*", "**/*.json", "**/*.xml"};
	var pathToDeletePatterns = new[] {"**/Directory-To-Delete/**", @"**/File-To-Delete.*"};
	var source = @"C:\Data\Transforming\Sources\Package.zip";
	var transformationNames = new[] {"Release", "Test"};

#### 1.3.2 [Additional options](/Source/Project/Configuration/TransformingOptions.cs)

	packageTransformer.Transform(destination, fileToTransformPatterns, pathToDeletePatterns, source, transformationNames, new TransformingOptions
	{
		File =
		{
			AvoidByteOrderMark = true,
			Replacement =
			{
				Enabled = true,
				Replace = value =>
				{
					// You can do replacements here.
					return value;
				}
			}

		},
		Package =
		{
			Cleanup = true
		}
	});

The actual transforming is done under the %temp%-directory. The "Cleanup" property (defaults to true) is for removing the temporary transform-directories or not.

## 2 Development

### 2.1 Signing

Drop the "StrongName.snk" file in the repository-root. The file should not be included in source control.

## 3 Tests

### 3.1 Links

- [TestMethod, TestInitialize, and TestCleanup in XUnit2](https://blog.somewhatabstract.com/2016/11/21/testmethod-testinitialize-and-testcleanup-in-xunit2)
- [Shared Context between Tests](https://xunit.net/docs/shared-context)