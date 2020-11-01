# This file is part of the UpbeatUI project, which is released under MIT License.
# See LICENSE.md or visit:
# https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md

Param(
  [Parameter(Mandatory=$true)]
  [Security.SecureString] $apikey
)

Write-Host "`n`n*** Building projects ***`n`n"
dotnet build -c Release

Write-Host "`n`n*** Packing projects ***`n`n"
dotnet pack -c Release

$clearapikey = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto([System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($apikey))

Write-Host "`n`n*** Publishing UpbeatUI to Nuget ***`n`n"
$version = [Version] $([Xml] (Get-Content .\UpbeatUI\UpbeatUI.csproj)).Project.PropertyGroup.Version
dotnet nuget push "UpbeatUI\bin\Release\UpbeatUI.$($version.Major).$($version.Minor).$($version.Build).nupkg" --api-key $clearapikey --source "https://api.nuget.org/v3/index.json"

Write-Host "`n`n*** Publishing UpbeatUI.Extensions.Hosting to Nuget ***`n`n"
$version = [Version] $([Xml] (Get-Content .\UpbeatUI.Extensions.Hosting\UpbeatUI.Extensions.Hosting.csproj)).Project.PropertyGroup.Version
dotnet nuget push "UpbeatUI.Extensions.Hosting\bin\Release\UpbeatUI.Extensions.Hosting.$($version.Major).$($version.Minor).$($version.Build).nupkg" --api-key $clearapikey --source "https://api.nuget.org/v3/index.json"
