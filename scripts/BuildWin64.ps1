param()

[string] $Product = "TEGS"
[string] $Target = "Win64"

& "$PSScriptRoot\Build.ps1" -Product $Product -Target $Target -BuildArgs "-target:Publish -p:RuntimeIdentifier=win-x64 -p:PublishSingleFile=true -p:PublishTrimmed=true -p:IncludeAllContentForSelfExtract=true -p:SelfContained=true -p:EnableCompressionInSingleFile=true"

& "$PSScriptRoot\ZipRelease.ps1" -Product $Product -Target $Target
