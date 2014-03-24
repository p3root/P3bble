# TODO: Quieten mkdir
mkdir -Force package\lib\windowsphone8
copy P3bble.nuspec package
copy ..\src\P3bble.Core\Bin\Release\P3bble.Core.dll package\lib\windowsphone8
copy ..\src\P3bble.Core\Bin\Release\P3bble.Core.XML package\lib\windowsphone8

# Update version number to that of assembly...
#Get-Module System.Xml.XDocument.Load("");
echo "TODO: UPDATE PACKAGE VERSION!"

# Finally, package up...
Start-Process -FilePath ..\..\src\.nuget\nuget.exe -ArgumentList "pack P3bble.nuspec" -Wait -WindowStyle Hidden -WorkingDirectory package

copy package\*.nupkg .
del package\*.nupkg

# clear up working folder...
Remove-Item -Path package -Force -Recurse