# UpbeatUI

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/michaelpduda/UpbeatUI/blob/main/LICENSE.md)
[![Nuget](https://img.shields.io/nuget/v/UpbeatUI)](https://www.nuget.org/packages/UpbeatUI/)

UpbeatUI is an open-source lightweight MVVM framework for quickly developing multi-level touch based Windows applications on top of the [WPF](https://github.com/dotnet/wpf) framework.

## Samples

Two samples are included: one showing [manual setup](samples/basicsample) and teardown and one showing [automatic setup](samples/hostedsample) using IHostBuilder. Both samples demonstrate the following capabilities:

![UpbeatUI Sample](https://user-images.githubusercontent.com/20475952/75388904-b5c1f900-58b3-11ea-8ef8-3cbbf347bbb3.gif)

## Download

UpbeatUI implementations are available as NuGet packages:

* [![Nuget](https://img.shields.io/nuget/v/UpbeatUI)](https://www.nuget.org/packages/UpbeatUI/) - UpbeatUI: Basic implementation requiring manual setup and teardown.
* [![Nuget](https://img.shields.io/nuget/v/UpbeatUI.Extensions.Hosting)](https://www.nuget.org/packages/UpbeatUI.Extensions.Hosting/) - UpbeatUI.Extensions.Hosting: An implementation integrated with [Microsoft.Extensions.Hosting](https://www.nuget.org/packages/Microsoft.Extensions.Hosting) (IHostBuilder) for easy setup and automatic teardown.
