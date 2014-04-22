# TODO: Quieten mkdir
$null = mkdir -Force package\lib\windowsphone8
$null = mkdir -Force package\lib\windowsphone8.1
$null = mkdir -Force package\lib\portable-sl4+wp71+windows8

copy P3bble.nuspec package

copy ..\src\P3bble.Core\Bin\Release\wp8.1\P3bble.Core.dll package\lib\windowsphone8.1
copy ..\src\P3bble.Core\Bin\Release\wp8.1\P3bble.Core.XML package\lib\windowsphone8.1

copy ..\src\P3bble.Core\Bin\Release\wp8\P3bble.Core.dll package\lib\windowsphone8
copy ..\src\P3bble.Core\Bin\Release\wp8\P3bble.Core.XML package\lib\windowsphone8

copy ..\src\P3bble.PCL\Bin\Release\P3bble.PCL.dll package\lib\portable-sl4+wp71+windows8

# Update version number to that of assembly...
$ver = ls -fi .\package\lib\windowsphone8\P3bble.Core.dll | % { $_.versioninfo }
$unversioned = Get-Content package\P3bble.nuspec
$versioned = $unversioned -replace "{VERSION_MADE_BY_SCRIPT}", $ver.FileVersion
Set-Content package\P3bble.nuspec $versioned

# Finally, package up...
Start-Process -FilePath ..\..\src\.nuget\nuget.exe -ArgumentList "pack P3bble.nuspec" -Wait -WindowStyle Hidden -WorkingDirectory package

copy package\*.nupkg .
#del package\*.nupkg

# clear up working folder...
#Remove-Item -Path package -Force -Recurse