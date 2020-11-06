# UpbeatUI

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/michaelpduda/UpbeatUI/blob/main/LICENSE.md)
[![Nuget](https://img.shields.io/nuget/v/UpbeatUI)](https://www.nuget.org/packages/UpbeatUI/)

UpbeatUI is an open-source lightweight MVVM framework for quickly developing multi-level touch based Windows applications on top of the [WPF](https://github.com/dotnet/wpf) framework. It provides an elegant API for creating mobile-style applications on the desktop! The goal is that only screens should stack and only be visible at a time. There are other UI efficiencies, like popovers being exitable by tapping outside of them.

## Installation

UpbeatUI implementations are available as NuGet packages:

* [![Nuget](https://img.shields.io/nuget/v/UpbeatUI)](https://www.nuget.org/packages/UpbeatUI/) - UpbeatUI: Basic implementation requiring manual setup and teardown.
* [![Nuget](https://img.shields.io/nuget/v/UpbeatUI.Extensions.Hosting)](https://www.nuget.org/packages/UpbeatUI.Extensions.Hosting/) - UpbeatUI.Extensions.Hosting: An implementation integrated with [Microsoft.Extensions.Hosting](https://www.nuget.org/packages/Microsoft.Extensions.Hosting) (IHostBuilder) for easy setup and automatic teardown.

## Examples

Two samples are included: one showing [manual setup](samples/basicsample) and teardown and one showing [automatic setup](samples/hostedsample) using IHostBuilder. Both samples demonstrate the following capabilities:

![UpbeatUI Sample](https://user-images.githubusercontent.com/20475952/75388904-b5c1f900-58b3-11ea-8ef8-3cbbf347bbb3.gif)

## Contributing

Additions and bug fixes are welcome! For larger changes please open an issue to discuss the changes.

### Step 1

Start by forking the repository then cloning that to your local machine.

```
git clone https://github.com/[you]/UpbeatUI
```

### Step 2

Make the changes, and please **test** them. For larger changes, include an example.

### Step 3

Create a pull request! 

There may be some back and forth, we appreciate you working with us to get the code merged. <3
