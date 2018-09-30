@echo off
cd ..\src
nant
cd ..\test

copy ..\dist\Volte.Data.Json.dll Volte.Data.Json.dll
copy ..\dist\Volte.Data.Json.pdb Volte.Data.Json.pdb
%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\csc test.cs /w:2 /debug+ /r:Volte.Data.Json.dll,Newtonsoft.Json.dll,Volte.Utils.dll
pause