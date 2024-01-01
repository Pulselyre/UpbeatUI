# This file is part of the UpbeatUI project, which is released under MIT License.
# See LICENSE.md or visit:
# https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md

# This file defines tasks for the Invoke-Build tool for PowerShell.
# To install Invoke-Build, visit their GitHub page:
# https://github.com/nightroman/Invoke-Build#install-as-module

Param(
  [Parameter(Mandatory = $false, Position = 1)]
  [ValidateSet('quiet', 'minimal', 'normal', 'detailed', 'diagnostic')]
  [Alias('v')]
  [String] $verbosity = 'quiet',
  [Parameter(Mandatory = $false, Position = 2)]
  [Alias('gcs')]
  [Boolean] $generateCompatibilitySuppression = $false
)

task RestoreAll RestoreBase, RestoreDependencyInjection, RestoreHosting, `
  RestoreManualSample, RestoreServiceProvidedSample, RestoreHostedSample, `
  RestoreTests
task ra RestoreAll

task RestoreBase {
  dotnet restore `
    '.\source\UpbeatUI' `
    --verbosity $verbosity
  equals $LASTEXITCODE 0
}
task rb RestoreBase

task RestoreDependencyInjection {
  dotnet restore `
    '.\source\UpbeatUI.Extensions.DependencyInjection' `
    --verbosity $verbosity
  equals $LASTEXITCODE 0
}
task rdi RestoreDependencyInjection

task RestoreHosting {
  dotnet restore `
    '.\source\UpbeatUI.Extensions.Hosting' `
    --verbosity $verbosity
  equals $LASTEXITCODE 0
}
task rh RestoreHosting

task RestoreTests {
  dotnet restore `
    '.\source\UpbeatUI.Tests' `
    --verbosity $verbosity
  equals $LASTEXITCODE 0
}
task rt RestoreTests

task RestoreManualSample {
  dotnet restore `
    '.\samples\ManualUpbeatUISample' `
    --verbosity $verbosity
  equals $LASTEXITCODE 0
}
task rms RestoreManualSample

task RestoreServiceProvidedSample {
  dotnet restore `
    '.\samples\ServiceProvidedUpbeatUISample' `
    --no-dependencies `
    --verbosity $verbosity
  equals $LASTEXITCODE 0
}
task rsps RestoreServiceProvidedSample

task RestoreHostedSample {
  dotnet restore `
    '.\samples\HostedUpbeatUISample' `
    --no-dependencies `
    --verbosity $verbosity
  equals $LASTEXITCODE 0
}
task rhs RestoreHostedSample

task BuildAll BuildBase, BuildDependencyInjection, BuildHosting, `
  BuildManualSample, BuildServiceProvidedSample, BuildHostedSample, `
  BuildTests
task ba BuildAll

task BuildPackages BuildBase, BuildDependencyInjection, BuildHosting
task bp BuildPackages

task BuildBase {
  dotnet build `
    '.\source\UpbeatUI' `
    --verbosity $verbosity `
    -c Debug
  equals $LASTEXITCODE 0
}
task bb BuildBase

task BuildDependencyInjection {
  dotnet build `
    '.\source\UpbeatUI.Extensions.DependencyInjection' `
    --verbosity $verbosity `
    -c Debug
  equals $LASTEXITCODE 0
}
task bdi BuildDependencyInjection

task BuildHosting {
  dotnet build `
    '.\source\UpbeatUI.Extensions.Hosting' `
    --verbosity $verbosity `
    -c Debug
  equals $LASTEXITCODE 0
}
task bh BuildHosting

task BuildTests {
  dotnet build `
    '.\source\UpbeatUI.Tests' `
    --verbosity $verbosity `
    -c Debug
  equals $LASTEXITCODE 0
}
task bt BuildTests

task BuildSamples BuildManualSample, BuildServiceProvidedSample, BuildHostedSample
task bs BuildSamples

task BuildManualSample {
  dotnet build `
    '.\samples\ManualUpbeatUISample' `
    --verbosity $verbosity `
    -c Debug
  equals $LASTEXITCODE 0
}
task bms BuildManualSample

task BuildServiceProvidedSample {
  dotnet build `
    '.\samples\ServiceProvidedUpbeatUISample' `
    --verbosity $verbosity `
    -c Debug
  equals $LASTEXITCODE 0
}
task bsps BuildServiceProvidedSample

task BuildHostedSample {
  dotnet build `
    '.\samples\HostedUpbeatUISample' `
    --verbosity $verbosity `
    -c Debug
  equals $LASTEXITCODE 0
}
task bhs BuildHostedSample

task PackAll PackBase, PackDependencyInjection, PackHosting
task pa PackAll

task PackBase {
  dotnet pack `
    '.\source\UpbeatUI' `
    --verbosity $verbosity `
    --force `
    -c Release `
    /p:ContinuousIntegrationBuild=true `
    /p:GenerateCompatibilitySuppressionFile=$generateCompatibilitySuppression
  equals $LASTEXITCODE 0
}
task pb PackBase

task PackDependencyInjection {
  dotnet pack `
    '.\source\UpbeatUI.Extensions.DependencyInjection' `
    --verbosity $verbosity `
    -c Release `
    /p:ContinuousIntegrationBuild=true `
    /p:GenerateCompatibilitySuppressionFile=$generateCompatibilitySuppression
  equals $LASTEXITCODE 0
}
task pdi PackDependencyInjection

task PackHosting {
  dotnet pack `
    '.\source\UpbeatUI.Extensions.Hosting' `
    --verbosity $verbosity `
    --force `
    -c Release `
    /p:ContinuousIntegrationBuild=true `
    /p:GenerateCompatibilitySuppressionFile=$generateCompatibilitySuppression
  equals $LASTEXITCODE 0
}
task ph PackHosting

task PublishAll PublishBase, PublishDependencyInjection, PublishHosting
task puba PublishAll

task SetPublishApiKey {
  $apiKey = Read-Host "Enter NuGet API Key" -AsSecureString
  $script:clearapikey = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto([System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($apikey))
}

task PublishBase SetPublishApiKey, PackBase, {
  $version = [Version] $([Xml] (Get-Content .\source\UpbeatUI\UpbeatUI.csproj)).Project.PropertyGroup.Version
  $versionString = "$($version.Major).$($version.Minor).$($version.Build)"
  if ($Host.UI.PromptForChoice("About to publish UpbeatUI package version $versionString", 'Continue?', ('&Yes', '&No'), 1) -eq 0) {
    dotnet nuget push "source\UpbeatUI\bin\Release\UpbeatUI.$($versionString).nupkg" --api-key $clearapikey --source "https://api.nuget.org/v3/index.json"
    equals $LASTEXITCODE 0
  }
  else {
    throw "Canceled"
  }
}
task pubb PublishBase

task PublishDependencyInjection SetPublishApiKey, PackDependencyInjection, {
  $version = [Version] $([Xml] (Get-Content .\source\UpbeatUI.Extensions.DependencyInjection\UpbeatUI.Extensions.DependencyInjection.csproj)).Project.PropertyGroup.Version
  $versionString = "$($version.Major).$($version.Minor).$($version.Build)"
  if ($Host.UI.PromptForChoice("About to publish UpbeatUI.Extensions.DependencyInjection package version $versionString", 'Continue?', ('&Yes', '&No'), 1) -eq 0) {
    dotnet nuget push "source\UpbeatUI.Extensions.DependencyInjection\bin\Release\UpbeatUI.Extensions.DependencyInjection.$($versionString).nupkg" --api-key $clearapikey --source "https://api.nuget.org/v3/index.json"
    equals $LASTEXITCODE 0
  }
  else {
    throw "Canceled"
  }
}
task pubdi PublishDependencyInjection

task PublishHosting SetPublishApiKey, PackHosting, {
  $version = [Version] $([Xml] (Get-Content .\source\UpbeatUI.Extensions.Hosting\UpbeatUI.Extensions.Hosting.csproj)).Project.PropertyGroup.Version
  $versionString = "$($version.Major).$($version.Minor).$($version.Build)"
  if ($Host.UI.PromptForChoice("About to publish UpbeatUI.Extensions.Hosting package version $versionString", 'Continue?', ('&Yes', '&No'), 1) -eq 0) {
    dotnet nuget push "source\UpbeatUI.Extensions.Hosting\bin\Release\UpbeatUI.Extensions.Hosting.$($versionString).nupkg" --api-key $clearapikey --source "https://api.nuget.org/v3/index.json"
    equals $LASTEXITCODE 0
  }
  else {
    throw "Canceled"
  }
}
task pubh PublishHosting

task CleanAll CleanBase, CleanDependencyInjection, CleanHosting, `
  CleanManualSample, CleanServiceProvidedSample, CleanHostedSample, `
  CleanTests
task ca CleanAll

function Remove-Output-Dirs($baseDir) {
  foreach ($dir in @('obj', 'bin')) { Remove-Item -Path "$baseDir\$dir" -recurse -ErrorAction SilentlyContinue }
}

task CleanBase {
  Remove-Output-Dirs '.\source\UpbeatUI'
}
task cb CleanBase

task CleanDependencyInjection {
  Remove-Output-Dirs '.\source\UpbeatUI.Extensions.DependencyInjection'
}
task cdi CleanDependencyInjection

task CleanHosting {
  Remove-Output-Dirs '.\source\UpbeatUI.Extensions.Hosting'
}
task ch CleanHosting

task CleanTests {
  Remove-Output-Dirs '.\source\UpbeatUI.Tests'
}
task ct CleanTests

task CleanManualSample {
  Remove-Output-Dirs '.\samples\ManualUpbeatUISample'
}
task cms CleanManualSample

task CleanServiceProvidedSample {
  Remove-Output-Dirs '.\samples\ServiceProvidedUpbeatUISample'
}
task csps CleanServiceProvidedSample

task CleanHostedSample {
  Remove-Output-Dirs '.\samples\HostedUpbeatUISample'
}
task chs CleanHostedSample

task RunTests {
  dotnet test `
    '.\source\UpbeatUI.Tests' `
    --verbosity $verbosity
  equals $LASTEXITCODE 0
}
task runt RunTests

task RunManualSample {
  dotnet run `
    --project '.\samples\ManualUpbeatUISample' `
    --verbosity $verbosity `
    -c Debug
  equals $LASTEXITCODE 0
}
task runms RunManualSample

task RunServiceProvidedSample {
  dotnet run `
    --project '.\samples\ServiceProvidedUpbeatUISample' `
    --verbosity $verbosity `
    -c Debug
  equals $LASTEXITCODE 0
}
task runsps RunServiceProvidedSample

task RunHostedSample {
  dotnet run `
    --project '.\samples\HostedUpbeatUISample' `
    --verbosity $verbosity `
    -c Debug
  equals $LASTEXITCODE 0
}
task runhs RunHostedSample
