# Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass
delete packages\*.nupkg
# dotnet nuget source remove --name HomeFeed

dotnet pack -c Debug -o ./packages -p:PackageVersion=1.0.0.0-beta

// create a directory if required
# if (not exist "%USERPROFILE%\NugetFeed") {
#     mkdir "%USERPROFILE%\NugetFeed"
# }

# dotnet nuget add source "%USERPROFILE%\NugetFeed" --name HomeFeed