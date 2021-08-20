param()

[string] $Product = "TEGS"
[string] $Target = "Win32"

& "$PSScriptRoot\Build.ps1" -Product $Product -Target $Target -BuildArgs "-target:Publish -p:RuntimeIdentifier=win-x86 -p:PublishSingleFile=true -p:PublishTrimmed=true -p:IncludeAllContentForSelfExtract=true -p:TrimMode=link"

& "$PSScriptRoot\ZipRelease.ps1" -Product $Product -Target $Target
