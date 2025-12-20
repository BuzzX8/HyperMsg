[![Core Build](https://github.com/BuzzX8/HyperMsg/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/BuzzX8/HyperMsg/actions/workflows/build-and-test.yml)

## Packaging and Installing in a Local NuGet Feed

To package HyperMsg and install it in a local NuGet feed:

### 1. Build the NuGet Package

Run the following command in the project directory:

```sh
# For Windows PowerShell
 dotnet pack -c Release
```

This will generate a `.nupkg` file in the `bin/Release` folder.

### 2. Add the Package to a Local Feed

You can use a local folder as a NuGet feed. For example, create a folder `C:\LocalNugetFeed` and copy the `.nupkg` file there:

```sh
# For Windows PowerShell
Copy-Item .\src\HyperMsg.Core\bin\Release\*.nupkg C:\LocalNugetFeed
```

### 3. Add the Local Feed to Your Project

In your consuming project's `.csproj` or via Visual Studio NuGet settings, add the local feed:

```sh
# For Windows PowerShell
 dotnet nuget add source C:\LocalNugetFeed -n LocalFeed
```

Or, in Visual Studio: Tools > Options > NuGet Package Manager > Package Sources > Add...

### 4. Install the Package

Now you can install the package in your project:

```sh
# For Windows PowerShell
 dotnet add package HyperMsg.Core --source LocalFeed
```