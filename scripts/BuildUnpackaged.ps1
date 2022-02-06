param()

[string] $Product = "TEGS"
[string] $Target = "Unpackaged"

& "$PSScriptRoot\Build.ps1" -Product $Product -Target $Target -BuildArgs "-target:Publish"

& "$PSScriptRoot\ZipRelease.ps1" -Product $Product -Target $Target
