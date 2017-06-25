nuget install -ExcludeVersion -SolutionDirectory . -Source http://localhost/nuget/api/v2/package


mkdir %~dp0temp
nuget pack %~dp0src\JSONObject.csproj -build -Symbols -Prop Configuration=Release -OutputDirectory %~dp0temp

nuget.exe push %~dp0temp\JSONObject*.nupkg 123456 -Source http://localhost/nuget/api/v2/package
del /q %~dp0temp\*
rmdir %~dp0temp
