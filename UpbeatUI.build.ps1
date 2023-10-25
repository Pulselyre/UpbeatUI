# This file is part of the UpbeatUI project, which is released under MIT License.
# See LICENSE.md or visit:
# https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md

# This file defines tasks for the Invoke-Build tool for PowerShell.
# To install Invoke-Build, visit their GitHub page:
# https://github.com/nightroman/Invoke-Build#install-as-module

Param(

  [Parameter(Mandatory = $false, Position = 2)]
  [ValidateSet('quiet', 'minimal', 'normal', 'detailed', 'diagnostic')]
  [Alias('v')]
  [String] $verbosity = 'quiet'
)

task RestoreAll RestoreBase, RestoreDependencyInjection, RestoreHosting,
  RestoreManualSample, RestoreServiceProvidedSample, RestoreHostedSample,
  RestoreTests
task ra RestoreAll

task RestoreBase {
  dotnet restore `
    '.\source\UpbeatUI' `
    --verbosity $verbosity
}
task rb RestoreBase

task RestoreDependencyInjection {
  dotnet restore `
    '.\source\UpbeatUI.Extensions.DependencyInjection' `
    --verbosity $verbosity
}
task rdi RestoreDependencyInjection

task RestoreHosting {
  dotnet restore `
    '.\source\UpbeatUI.Extensions.Hosting' `
    --verbosity $verbosity
}
task rh RestoreHosting

task RestoreTests {
  dotnet restore `
    '.\source\UpbeatUI.Tests' `
    --verbosity $verbosity
}
task rt RestoreTests

task RestoreManualSample  {
  dotnet restore `
    '.\samples\ManualUpbeatUISample' `
    --verbosity $verbosity
}
task rms RestoreManualSample

task RestoreServiceProvidedSample {
  dotnet restore `
    '.\samples\ServiceProvidedUpbeatUISample' `
    --no-dependencies `
    --verbosity $verbosity
}
task rsps RestoreServiceProvidedSample

task RestoreHostedSample {
  dotnet restore `
    '.\samples\HostedUpbeatUISample' `
    --no-dependencies `
    --verbosity $verbosity
}
task rhs RestoreHostedSample

task BuildAll BuildBase, BuildDependencyInjection, BuildHosting,
  BuildManualSample, BuildServiceProvidedSample, BuildHostedSample,
  BuildTests
task ba BuildAll

task BuildPackages BuildBase, BuildDependencyInjection, BuildHosting
task bp BuildPackages

task BuildBase {
  dotnet build `
    '.\source\UpbeatUI' `
    --verbosity $verbosity `
    --no-incremental `
    -c Debug `
    /p:GenerateFullPaths=true
}
task bb BuildBase

task BuildDependencyInjection {
  dotnet build `
    '.\source\UpbeatUI.Extensions.DependencyInjection' `
    --verbosity $verbosity `
    --no-incremental `
    -c Debug `
    /p:GenerateFullPaths=true
}
task bdi BuildDependencyInjection

task BuildHosting {
  dotnet build `
    '.\source\UpbeatUI.Extensions.Hosting' `
    --verbosity $verbosity `
    --no-incremental `
    -c Debug `
    /p:GenerateFullPaths=true
}
task bh BuildHosting

task BuildTests {
  dotnet build `
    '.\source\UpbeatUI.Tests' `
    --verbosity $verbosity `
    --no-incremental `
    -c Debug `
    /p:GenerateFullPaths=true
}
task bt BuildTests

task BuildSamples BuildManualSample, BuildServiceProvidedSample, BuildHostedSample
task bs BuildSamples

task BuildManualSample {
  dotnet build `
    '.\samples\ManualUpbeatUISample' `
    --verbosity $verbosity `
    --no-incremental `
    -c Debug `
    /p:GenerateFullPaths=true
}
task bms BuildManualSample

task BuildServiceProvidedSample {
  dotnet build `
    '.\samples\ServiceProvidedUpbeatUISample' `
    --verbosity $verbosity `
    --no-incremental `
    -c Debug `
    /p:GenerateFullPaths=true
}
task bsps BuildServiceProvidedSample

task BuildHostedSample {
  dotnet build `
    '.\samples\HostedUpbeatUISample' `
    --verbosity $verbosity `
    --no-incremental `
    -c Debug `
    /p:GenerateFullPaths=true
}
task bhs BuildHostedSample

task CleanAll CleanBase, CleanDependencyInjection, CleanHosting,
  CleanManualSample, CleanServiceProvidedSample, CleanHostedSample,
  CleanTests
task ca CleanAll

task CleanBase {
  dotnet clean `
    '.\source\UpbeatUI' `
    --verbosity $verbosity
}
task cb CleanBase

task CleanDependencyInjection {
  dotnet clean `
    '.\source\UpbeatUI.Extensions.DependencyInjection' `
    --verbosity $verbosity
}
task cdi CleanDependencyInjection

task CleanHosting {
  dotnet clean `
    '.\source\UpbeatUI.Extensions.Hosting' `
    --verbosity $verbosity
}
task ch CleanHosting

task CleanTests {
  dotnet clean `
    '.\source\UpbeatUI.Tests' `
    --verbosity $verbosity
}
task ct CleanTests

task CleanManualSample {
  dotnet clean `
    '.\samples\ManualUpbeatSample' `
    --verbosity $verbosity
}
task cms CleanManualSample

task CleanServiceProvidedSample {
  dotnet clean `
    '.\samples\ServiceProvidedUpbeatUISample' `
    --verbosity $verbosity
}
task csp CleanServiceProvidedSample

task CleanHostedSample {
  dotnet clean `
    '.\samples\HostedUpbeatUISample' `
    --verbosity $verbosity
}
task ch CleanHostedSample

task RunTests {
  dotnet test `
    '.\source\UpbeatUI.Tests' `
    --verbosity $verbosity
}
task runt RunTests

task RunManualSample {
  dotnet run `
    --project '.\samples\ManualUpbeatUISample' `
    --verbosity $verbosity `
    -c Debug `
    /p:GenerateFullPaths=true
}
task runms RunManualSample

task RunServiceProvidedSample {
  dotnet run `
    --project '.\samples\ServiceProvidedUpbeatUISample' `
    --verbosity $verbosity `
    -c Debug `
    /p:GenerateFullPaths=true
}
task runsps RunServiceProvidedSample

task RunHostedSample {
  dotnet run `
    --project '.\samples\HostedUpbeatUISample' `
    --verbosity $verbosity `
    -c Debug `
    /p:GenerateFullPaths=true
}
task runhs RunHostedSample
