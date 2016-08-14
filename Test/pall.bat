@echo off
copy ..\dist\Volte.Data.JsonObject.dll Volte.Data.JsonObject.dll
copy ..\dist\Volte.Data.JsonObject.pdb Volte.Data.JsonObject.pdb
%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\csc test.cs /w:2 /debug+ /r:Volte.Data.JsonObject.dll
