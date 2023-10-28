# UpbeatUI

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/pulselyre/UpbeatUI/blob/main/LICENSE.md)
[![Nuget](https://img.shields.io/nuget/v/UpbeatUI)](https://www.nuget.org/packages/UpbeatUI/)

UpbeatUI is an open-source lightweight MVVM framework for quickly developing mobile-style touch based Windows applications using [Windows Presentation Foundation (WPF)](https://github.com/dotnet/wpf). It provides a simple API for stacking Views in the Z-Axis where only the top View is active to the user. The user can close the active view (remove it from the top of the stack) by tapping/clicking the surrounding blurred background area. UpbeatUI also includes versions of several standard MVVM objects and base classes.

UpbeatUI supports **.NET Core 3.0**, **.NET Core 3.1**, **.NET 5**, **.NET 6**, and **.NET 7**.

Please note that UpbeatUI is fairly new and may have unidentified bugs or performance inefficiencies. Please see the [Contributing](#contributing) section for information on how to help make UpbeatUI better. See the [To-Do List](#to-do-list) for planned future work.

## Installation

UpbeatUI implementations are available as NuGet packages:

* [![Nuget](https://img.shields.io/nuget/v/UpbeatUI)](https://www.nuget.org/packages/UpbeatUI/) - **UpbeatUI**: Basic implementation requiring manual setup and teardown.
* [![Nuget](https://img.shields.io/nuget/v/UpbeatUI.Extensions.DependencyInjection)](https://www.nuget.org/packages/UpbeatUI.Extensions.DependencyInjection/) - **UpbeatUI.Extensions.DependencyInjection**: An implementation integrated with [Microsoft.Extensions.DependencyInjection](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection) (**IServiceProvider**) that provides dependency injection capabilities and automatic Parameters-ViewModel-View mapping via naming convention.
* [![Nuget](https://img.shields.io/nuget/v/UpbeatUI.Extensions.Hosting)](https://www.nuget.org/packages/UpbeatUI.Extensions.Hosting/) - **UpbeatUI.Extensions.Hosting**: An implementation integrated with [Microsoft.Extensions.Hosting](https://www.nuget.org/packages/Microsoft.Extensions.Hosting) (**IHostBuilder**) for easy setup and automatic teardown.

## Examples

Three samples are included: one showing [manual setup](samples/ManualUpbeatUISample) and teardown without dependency injection, one showing manual setup and teardown with [dependency injection](samples/ServiceProvidedUpbeatUISample) using an **IServiceProvider**, and one showing [automatic setup and teardown](samples/HostedUpbeatUISample) using an **IHostBuilder**. All samples demonstrate the following capabilities:

![UpbeatUI Sample](https://github.com/Pulselyre/UpbeatUI/assets/20475952/968f2465-43cb-4486-a671-c8a0d898022e)

## How UpbeatUI Works

There are two central components of UpbeatUI: The first is the [**UpbeatStack**](source/UpbeatUI/ViewModel/UpbeatStack.cs) class. It is responsible for managing the stack of open ViewModels and also maintaining mappings between ViewModelParameters, ViewModels, and Views. The second is the [**IUpbeatService**](source/UpbeatUI/ViewModel/IUpbeatService.cs) interface which provides ViewModels with methods and properties for interacting with their parent UpbeatStack. For example, the [**IUpbeatService**](source/UpbeatUI/ViewModel/IUpbeatService.cs) has methods for opening a new ViewModel on top of the stack. The [**UpbeatStack**](source/UpbeatUI/ViewModel/UpbeatStack.cs) provides a unique [**IUpbeatService**](source/UpbeatUI/ViewModel/IUpbeatService.cs) to each ViewModel.

Opening a new ViewModel is done by passing a ViewModelParameters object to the *OpenViewModel* or *OpeanViewModelAsync* method on the [**UpbeatStack**](source/UpbeatUI/ViewModel/UpbeatStack.cs) or [**IUpbeatService**](source/UpbeatUI/ViewModel/IUpbeatService.cs). A ViewModelParameters object contains initialization data for a ViewModel. There must be a unique ViewModelParameters class for each ViewModel class, as the [**UpbeatStack**](source/UpbeatUI/ViewModel/UpbeatStack.cs) relies on mappings between Types to create ViewModels and display Views.

## How to use UpbeatUI

Please see the separate [How To Use](HOW-TO-USE.md) markdown file for a guide to the basic features in UpbeatUI.

## To-Do List

- Publish new major release with deprecated classes and methods fully removed.
- Deprecate the public [`ActionDeferrer`](source\UpbeatUI\ViewModel\ActionDeferrer.cs) (and move all functionality to [`UpbeatStack.UpbeatServiceDeferrer`](source\UpbeatUI\ViewModel\UpbeatStack.UpbeatServiceDeferrer.cs)).
- Simplify [`HostedUpbeatBuilder`](source\UpbeatUI.Extensions.Hosting\HostedUpbeatBuilder.cs), [`HostedUpbeatSerivce`](source\UpbeatUI.Extensions.Hosting\HostedUpbeatService.cs), [`UpbeatApplicationService`](source\UpbeatUI.Extensions.Hosting\UpbeatApplicationService.cs), and [`ConfigureUpbeatHost`](source/UpbeatUI.Extensions.Hosting/Extensions.cs#L22) implementations.
- Write more tests.
- Execute tests via GitHub Action.
- Create an icon/logo for NuGet packages.
- Write and [add README files](https://devblogs.microsoft.com/nuget/add-a-readme-to-your-nuget-package/) for NuGet packages.
- Add [MAUI](https://github.com/dotnet/maui) support.
- Cleanup/improve [`.gitignore` file](.gitignore) (too many unnecessary items listed).
- Cleanup/improve [`.editorconfig` file](.editorconfig) and format all source files.

## Contributing

Additions, bug fixes, and performance improvements are welcome. For larger modifications please open an issue to discuss the changes.

>Note: This project [defines tasks](UpbeatUI.build.ps1) for executing builds, tests, and other common actions using the [Invoke-Build](https://github.com/nightroman/Invoke-Build) tool for PowerShell. To install Invoke-Build, visit their GitHub page: [nightroman/Invoke-Build](https://github.com/nightroman/Invoke-Build#install-as-module).

### Step 1

Start by forking the repository then cloning that to your local machine.

```
git clone https://github.com/[you]/UpbeatUI
```

### Step 2

Make the changes, and please **test** them. For larger changes, include an example.

### Step 3

Create a pull request.

There may be some back and forth, but we appreciate you working with us to get your contributions merged.
