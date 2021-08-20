[string] $Product = "TEGS"
[string] $Target = "MacOS"

& "$PSScriptRoot\Build.ps1" -Product $Product -Target $Target -BuildArgs "-t:BundleApp -p:RuntimeIdentifier=osx-x64 -p:PublishTrimmed=true -p:IncludeAllContentForSelfExtract=true" -ProjectPath "src\$Product.UI\$Product.UI.csproj"

# Remove everything except the app bundle
Get-Childitem "$PSScriptRoot\..\bld\$Product.$Target\" -Exclude "tegsui.app" | Remove-Item -Recurse

& "$PSScriptRoot\Build.ps1" -Product $Product -Target $Target -Clean $False -BuildArgs "-t:Publish -p:RuntimeIdentifier=osx-x64 -p:PublishSingleFile=true -p:PublishTrimmed=true -p:IncludeAllContentForSelfExtract=true -p:TrimMode=link"

# Remove unbundled tegsui
Remove-Item "$PSScriptRoot\..\bld\$Product.$Target\tegsui"

& "$PSScriptRoot\TarRelease.ps1" -Product $Product -Target $Target
