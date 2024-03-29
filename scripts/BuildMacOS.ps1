[string] $Product = "TEGS"
[string] $Target = "MacOS"

& "$PSScriptRoot\Build.ps1" -Product $Product -Target $Target -Clean $False -BuildArgs "-t:Publish -p:RuntimeIdentifier=osx-x64 -p:PublishSingleFile=true -p:PublishTrimmed=true -p:IncludeAllContentForSelfExtract=true -p:SelfContained=true -p:EnableCompressionInSingleFile=true"

& "$PSScriptRoot\TarRelease.ps1" -Product $Product -Target $Target
