# This file is part of the UpbeatUI project, which is released under MIT License.
# See LICENSE.md or visit:
# https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md

Param(
  [Parameter(Mandatory = $false, Position = 0)]
  [ValidateSet('build', 'clean', 'restore', 'run')]
  [Alias('o', 'op')]
  [String] $operation = 'build',
  [Parameter(Mandatory = $false, Position = 1)]
  [ValidateSet('all', 'base', 'di', 'hosting', 'tests', 'msample', 'spsample', 'hsample')]
  [Alias('p')] 
  [String] $project = 'all',
  [Parameter(Mandatory = $false, Position = 2)]
  [ValidateSet('quiet', 'minimal', 'normal', 'detailed', 'diagnostic')]
  [Alias('v')]
  [String] $verbosity = 'normal'
)

try {
  # temporarily change to the correct folder
  $MyInvocation.MyCommand.Path | Split-Path | Push-Location

  $operation = $operation.ToLower()
  $project = $project.ToLower()
  switch ($operation) {
    'build' {
      switch ($project) {
        'all' {
          Write-Host 'Building all UpbeatUI projects'
          (
            '.\source\UpbeatUI.Tests',
            '.\samples\ManualUpbeatUISample',
            '.\samples\ServiceProvidedUpbeatUISample',
            '.\samples\HostedUpbeatUISample'
          ) | ForEach-Object {
            dotnet build `
              $_ `
              --verbosity $verbosity `
              -c Debug `
              /p:GenerateFullPaths=true
          }
        }
        'base' {
          Write-Host 'Building UpbeatUI NuGet Package'
          dotnet build `
            '.\source\UpbeatUI' `
            --verbosity $verbosity `
            -c Debug `
            /p:GenerateFullPaths=true
        }
        'di' {
          Write-Host 'Building UpbeatUI Dependency Injection NuGet Package'
          dotnet build `
            '.\source\UpbeatUI.Extensions.DependencyInjection' `
            --verbosity $verbosity `
            -c Debug `
            /p:GenerateFullPaths=true
        }
        'hosting' {
          Write-Host 'Building UpbeatUI Hosting NuGet Package'
          dotnet build `
            '.\source\UpbeatUI.Extensions.Hosting' `
            --verbosity $verbosity `
            -c Debug `
            /p:GenerateFullPaths=true
        }
        'tests' {
          Write-Host 'Building UpbeatUI Tests'
          dotnet build `
            '.\source\UpbeatUI.Tests' `
            --verbosity $verbosity `
            -c Debug `
            /p:GenerateFullPaths=true
        }
        'msample' {
          Write-Host 'Building UpbeatUI Manual Sample'
          dotnet build `
            '.\samples\ManualUpbeatUISample' `
            --verbosity $verbosity `
            -c Debug `
            /p:GenerateFullPaths=true
        }
        'spsample' {
          Write-Host 'Building UpbeatUI Service Provided Sample'
          dotnet build `
            '.\samples\ServiceProvidedUpbeatUISample' `
            --verbosity $verbosity `
            -c Debug `
            /p:GenerateFullPaths=true
        }
        'hsample' {
          Write-Host 'Building UpbeatUI Hosted Sample'
          dotnet build `
            '.\samples\HostedUpbeatUISample' `
            --verbosity $verbosity `
            -c Debug `
            /p:GenerateFullPaths=true
        }
      }
    }
    'clean' {
      Write-Host 'Cleaning'
      Get-ChildItem -recurse -directory `
      | Where-Object { ('bin', 'obj').contains($_.name) } `
      | Remove-Item -force -recurse -verbose
      Write-Host 'Cleaned'
    }
    'restore' {
      Write-Host 'Restoring UpbeatUI Dependencies'
      switch ($project) {
        'all' {
          Write-Host 'Building all UpbeatUI projects'
          (
            '.\source\UpbeatUI.Tests',
            '.\samples\ManualUpbeatUISample',
            '.\samples\ServiceProvidedUpbeatUISample',
            '.\samples\HostedUpbeatUISample'
          ) | ForEach-Object {
            dotnet build `
              $_ `
              --verbosity $verbosity
          }
        }
        'base' {
          Write-Host 'Building UpbeatUI NuGet Package'
          dotnet build `
            '.\source\UpbeatUI' `
            --verbosity $verbosity
        }
        'di' {
          Write-Host 'Building UpbeatUI Dependency Injection NuGet Package'
          dotnet build `
            '.\source\UpbeatUI.Extensions.DependencyInjection' `
            --verbosity $verbosity
        }
        'hosting' {
          Write-Host 'Building UpbeatUI Hosting NuGet Package'
          dotnet build `
            '.\source\UpbeatUI.Extensions.Hosting' `
            --verbosity $verbosity 
        }
        'msample' {
          Write-Host 'Building UpbeatUI Manual Sample'
          dotnet build `
            '.\samples\ManualUpbeatUISample' `
            --verbosity $verbosity
        }
        'spsample' {
          Write-Host 'Building UpbeatUI Service Provided Sample'
          dotnet restore `
            '.\samples\ServiceProvidedUpbeatUISample' `
            --verbosity $verbosity
        }
        'hsample' {
          Write-Host 'Building UpbeatUI Hosted Sample'
          dotnet build `
            '.\samples\HostedUpbeatUISample' `
            --verbosity $verbosity
        }
      }
    }
    'run' {
      switch ($project) {
        'msample' {
          Write-Host 'Running UpbeatUI Manual Sample'
          dotnet run `
            --project '.\samples\ManualUpbeatUISample' `
            --verbosity $verbosity `
            -c Debug `
            /p:GenerateFullPaths=true
        }
        'spsample' {
          Write-Host 'Building UpbeatUI Service Provided Sample'
          dotnet run `
            --project '.\samples\ServiceProvidedUpbeatUISample' `
            --verbosity $verbosity `
            -c Debug `
            /p:GenerateFullPaths=true
        }
        'hsample' {
          Write-Host 'Building UpbeatUI Hosted Sample'
          dotnet run `
            --project '.\samples\HostedUpbeatUISample' `
            --verbosity $verbosity `
            -c Debug `
            /p:GenerateFullPaths=true
        }
      }
    }
    default {
      throw  '$operation is not a valid operation'
    }
  }
}
finally {
  Pop-Location
}
