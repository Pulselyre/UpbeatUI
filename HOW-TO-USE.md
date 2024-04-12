# How to use UpbeatUI

The sections below provide:

1. Guides on some of the options for initializing UpbeatUI and configuring the [`UpbeatStack`](source/UpbeatUI/ViewModel/UpbeatStack.cs).
2. Details on some of the functions that the [`IUpbeatService`](source/UpbeatUI/ViewModel/IUpbeatService.cs) provides to _ViewModels_.
3. Information on how _Views_ should be designed for best compatibility with UpbeatUI.

## Initiating with `IHostBuilder`

To create an UpbeatUI app using `IHostBuilder` (the simpler method), a [`ConfigureUpbeatHost`](source/UpbeatUI.Extensions.Hosting/ExtensionMethods.cs#L18-L43) extension method is provided in the [`UpbeatUI.Extensions.Hosting`](https://www.nuget.org/packages/UpbeatUI.Extensions.Hosting/) Nuget package. The method takes a required delegate parameter to create a _ViewModelParameters_ object for the main/bottom ViewModel. [`ConfigureUpbeatHost`](source/UpbeatUI.Extensions.Hosting/ExtensionMethods.cs#L18-L43) also takes an optional delegate parameter that provides an [`IHostedUpbeatBuilder`](source/UpbeatUI.Extensions.Hosting/IHostedUpbeatBuilder.cs) to set additional options in the application, such as specifying a custom containing window or setting specific _ViewModelParameters_-_ViewModel_-_View_ type mappings.

The below code automatically creates a default [`UpbeatMainWindow`](source/UpbeatUI/View/UpbeatMainWindow.xaml) with an  [`UpbeatStack`](source/UpbeatUI/ViewModel/UpbeatStack.cs) as its `DataContext`. A delegate to create a `BottomViewModel.Parameters` object is supplied, which UpbeatUI will use to create a `BottomViewModel` and display a `BottomControl` as the window's main/bottom layer. (If the `BottomViewModel` needs access to any services or configuration objects, they can be supplied via dependency injection similarly to _Controllers_ in ASP.NET Core.) Please see the comments in the [`App.xaml.cs`](samples/HostedUpbeatUISample/App.xaml.cs) file in the [hosted sample](samples/HostedUpbeatUISample) for guidance on additional options.

```xml
<Application
    x:Class="UpbeatUIDemo.App"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:local="clr-namespace:UpbeatUIDemo"
    Startup="HandleApplicationStartup" />
```

```C#
namespace UpbeatUIDemo;

public partial class App : Application
{
  private async void HandleApplicationStartup(object sender, StartupEventArgs e) =>
      await Host.CreateDefaultBuilder(await Host.CreateDefaultBuilder(e.Args))
          .ConfigureUpbeatHost(() => new BottomViewModel.Parameters())
          .Build()
          .RunAsync();
}
```

By default, using UpbeatUI with `IHostBuilder` enables automatic mapping between _ViewModelParameters_ and _ViewModels_. The default namespace and naming conventions are below, but the [`IHostedUpbeatBuilder`](source/UpbeatUI.Extensions.Hosting/IHostedUpbeatBuilder.cs) provides methods to set the mapping conventions manually (either from `Type`-to-`Type` or `AssemblyQualifiedName`-to-`AssemblyQualifiedName`). Specific type relationships can also be manually mapped with the [`IHostedUpbeatBuilder`](source/UpbeatUI.Extensions.Hosting/IHostedUpbeatBuilder.cs). The [`UpbeatStack`](source/UpbeatUI/ViewModel/UpbeatStack.cs) will always check for manually specified mappings first.

|                     | Naming Convention                                      | Example Class Name                                         |
|--------------------:|--------------------------------------------------------|------------------------------------------------------------|
| ViewModelParameters | `{BaseNamespace}.ViewModel.{Name}ViewModel+Parameters` | `SampleProject.ViewModel.PopupMessageViewModel+Parameters` |
|           ViewModel | `{BaseNamespace}.ViewModel.{Name}ViewModel`            | `SampleProject.ViewModel.PopupMessageViewModel`            |

>Note: In the default convention, the _ViewModelParameters_ class must be a nested class within the _ViewModel_ class (hence, the `+`).

An `IHostBuilder` UpbeatUI applciation will create _ViewModels_ and perform constructor dependency injection automatically using the `IServiceProvider` created by the `IHostBuilder`. UpbeatUI with dependency injection supports Scoped dependencies, where each _ViewModel_ is an independent scope.

## Initiating Manually

An UpbeatUI application can be started manually with automatic Dependency Injection and without.

### Without Dependency Injection

To start an UpbeatUI application without `IHostBuilder` or an `IServiceProvider`, both the containing window and the [`UpbeatStack`](source/UpbeatUI/ViewModel/UpbeatStack.cs) must be created, configured, and integrated manually. Mappings between _ViewModelParameters_ and _ViewModels_ will also need to be manually set and factory methods provided. The process is considerably more involved, so please see the comments in the [`App.xaml.cs`](samples/ManualUpbeatUISample/App.xaml.cs) file in the [manual sample](samples/ManualUpbeatUISample) for a guidance.

### With Dependency Injection

To start an UpbeatUI application without `IHostBuilder` but with an `IServiceProvider` for dependency injection, the containing window and the [`ServiceProvidedUpbeatStack`](source/UpbeatUI.Extensions.DependencyInjection/ServiceProvidedUpbeatStack.cs) must be created, configured, and integrated manually. However, mappings between _ViewModelParameters_ and _ViewModels_ do not need to be set manually. The process is also somewhat involved, so please see the comments in the [`App.xaml.cs`](samples/ServiceProvidedUpbeatUISample/App.xaml.cs) file in the [service provided sample](samples/ServiceProvidedUpbeatUISample) for a guidance. Using [`ServiceProvidedUpbeatStack`](source/UpbeatUI.Extensions.DependencyInjection/ServiceProvidedUpbeatStack.cs), mapping by convention is possible using the [`SetDefaultViewModelLocators`](source/UpbeatUI.Extensions.DependencyInjection/IServiceProvidedUpbeatStack.cs#L31-L39) method, or with more control using the other [`SetViewModelLocators`](source/UpbeatUI.Extensions.DependencyInjection/IServiceProvidedUpbeatStack.cs#L41-L62) methods. UpbeatUI will create _ViewModels_ and perform constructor dependency injection automatically using the `IServiceProvider` provided in the constructor. UpbeatUI with dependency injection supports Scoped dependencies, where each _ViewModel_ is an independent scope.

## Using [`IUpbeatService`](source/UpbeatUI/ViewModel/IUpbeatService.cs)

The [`IUpbeatService`](source/UpbeatUI/ViewModel/IUpbeatService.cs) interface provides functionality for _ViewModels_ to interact with their parent [`UpbeatStack`](source/UpbeatUI/ViewModel/UpbeatStack.cs).

### Opening New _ViewModels_

The most often used function is to open new child _ViewModels_ on the [`UpbeatStack`](source/UpbeatUI/ViewModel/UpbeatStack.cs) via the [`OpenViewModel`](source/UpbeatUI/ViewModel/IOpensViewModels.cs#L16-L29) or [`OpenViewModelAsync`](source/UpbeatUI/ViewModel/IOpensViewModels.cs#L31-L37) methods.

For example: if the currently active _ViewModel_ wanted to display a popup message to the user, then it might want to open a `PopupMessageViewModel` on the [`UpbeatStack`](source/UpbeatUI/ViewModel/UpbeatStack.cs). To do so, it would pass a `PopupMessageViewModel.Parameters` object with a `PopupMessage` string property to the `OpenViewModel` or `OpenViewModelAsync` methods:

```C#
// Note: OpenViewModelAsync returns a Task that completes when the opened ViewModel is closed
await _upbeatService.OpenViewModelAsync(
    new PopupMessageViewModel.Parameters
    {
        PopupMessage = "Message to display in popup"
    });
```

The [`UpbeatStack`](source/UpbeatUI/ViewModel/UpbeatStack.cs) will create a new `PopupMessageViewModel` (assuming the types are correctly mapped) by passing the submitted `PopupMessageViewModel.Parameters` object in the constructor for initialization (and also a new unique [`IUpbeatService`](source/UpbeatUI/ViewModel/IUpbeatService.cs)). A `PopupMessageControl` will displayed to the user with the new `PopupMessageViewModel` set as its `DataContext`, so a `TextBox` could be bound to display the `PopupMessage` property.

### Intercepting View and Window Close Events

_ViewModels_ that hold ongoing work that has not been saved or committed to a database might want confirm with the user before being closed. This is accomplished with the [`RegisterCloseCallback`](source/UpbeatUI/ViewModel/IUpbeatService.cs#L30-L40) method on the [`IUpbeatService`](source/UpbeatUI/ViewModel/IUpbeatService.cs). This method accepts an `okToClose` delegate that the [`UpbeatStack`](source/UpbeatUI/ViewModel/UpbeatStack.cs) will call before attempting to close the _ViewModel_. The delegate should return `true` if the _ViewModel_ can close, and `false` if not. Returning `false` can prevent the entire application from stopping (at least in orderly shutdown situations). The `okToClose` delegate can be synchronous or async.

## Constructing _Views_

With UpbeatUI, _ViewModel_ instances within the [`UpbeatStack`](source/UpbeatUI/ViewModel/UpbeatStack.cs) are arranged using an [`ItemsControl`](https://learn.microsoft.com/en-us/dotnet/api/system.windows.controls.itemscontrol) and [`BlurredZPanel`](source/UpbeatUI/View/BlurredZPanel.cs). Thus, _ViewModel_ instances are set as the `Content` property on [`ContentPresenter`s](https://learn.microsoft.com/en-us/dotnet/api/system.windows.controls.contentpresenter). In order for the _ViewModel_ instances to be rendered as visual controls, there must be an associated [`DataTemplate` with `DataType` property set to the _ViewModel_'s class](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/data/data-templating-overview?view=netframeworkdesktop-4.8#the-datatype-property). As an example, the following would be a `DataTemplate` defined in XAML for the above ``SampleProject.ViewModel.PopupMessageViewModel` _ViewModel_.

```xml
# xmlns:svm="clr-namespace:HostedUpbeatUISample.ViewModel"
<DataTemplate DataType="{x:Type svm:SampleProject.ViewModel.PopupMessageViewModel}">
  ...
</DataTemplate>
```

These `DataTemplate`s must be defined as `Resource`s within the `Application` instance. For simple applications, they could all be defined in the `App.xaml` file, though they can also be defined in separate `ResourceDictionary` `.xaml` files and [merged with the `Application`'s `ResourceDictionary`](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/systems/xaml-resources-merged-dictionaries?view=netdesktop-8.0#create-a-merged-dictionary). See [the sample projects](samples/HostedUpbeatUISample/App.xaml#L12-L25), as they use this technique.

### Sizing and Positioning _Views_

There are several attached properties that can be used to define where the _View's_ content is rendered within the window. These propertes must be set on the root control or panel within the `DataTemplate`. The properties are:

1. [`WidthPercent`](source/UpbeatUI/View/PercentPlace.cs#L34-L44) - Defines the amount of viewable horizontal space that the content will fill.
2. [`HeightPercent`](source/UpbeatUI/View/PercentPlace.cs#L14-L24) - Defines the amount of viewable vertical space that the content will fill.
3. [`XPositionPercent`](source/UpbeatUI/View/PercentPlace.cs#L46-L54) - Defines the percentage point horizontally within the viewable space that the content should be centered on.
4. [`YPositionPercent`](source/UpbeatUI/View/PercentPlace.cs#L55-L63) - Defines the percentage point vertically within the viewable space that the content should be centered on.
5. [`KeepInBounds`](source/UpbeatUI/View/PercentPlace.cs#L25-L33) - Defines whether the content should be kept within the viewable space or not. This is only relevent if the content is not centered.

Both [`WidthPercent`](source/UpbeatUI/View/PercentPlace.cs#L34-L44) and [`HeightPercent`](source/UpbeatUI/View/PercentPlace.cs#L14-L24) accept either one or two values. One value, such as `"50%"`, sets the content to always fill exactly half of the viewable space. Two values, such as `"10% 90%"`, sets the content to fill its desired size, but no smaller than 10% of the viewable space and no larger than 90% of the viewable space. The default behavior when [`WidthPercent`](source/UpbeatUI/View/PercentPlace.cs#L34-L44) or [`HeightPercent`](source/UpbeatUI/View/PercentPlace.cs#L14-L24) are unset is for content to fill its desired space, but no larger than viewable space.

For example, the following will take half of the available height, betwee half and 90% of available width, be positioned dynamically based on the bound [`XPositionPercent`](source/UpbeatUI/View/PercentPlace.cs#L46-L54) and [`YPositionPercent`](source/UpbeatUI/View/PercentPlace.cs#L55-L63) properites on the _ViewModel_, and will be kept within bounds (i.e., it will be rendered completely within the available space rather than potentially having a portion outside).

```xml
# xmlns:svm="clr-namespace:HostedUpbeatUISample.ViewModel"
# xmlns:uv="clr-namespace:UpbeatUI.View;assembly=UpbeatUI"
<DataTemplate DataType="{x:Type svm:SampleProject.ViewModel.PopupMessageViewModel}">
    <Grid
        uv:PercentPlace.HeightPercent="50%"
        uv:PercentPlace.WidthPercent="50% 90%"
        uv:PercentPlace.KeepInBounds="True"
        uv:PercentPlace.XPositionPercent="{Binding XPosition}"
        uv:PercentPlace.YPositionPercent="{Binding YPosition}">
        ...
    </Grid>
</DataTemplate>
```

For all _Views_ except the bottom layer, it is recommended to leave empty space around the sides that the user can touch/click to close the active _View_.

>Note: All four percent properties can accept numbers in percent format (`"50%"`) or decimal format (`"0.5"`).

See the various [`.xaml` files](samples/ManualUpbeatUISample/View/) in the sample projects for examples on how to configure `DataTemplate`s for UpbeatUI.

### Passing Position Information To _ViewModels_

In the above example, you can notice that the [`XPositionPercent`](source/UpbeatUI/View/PercentPlace.cs#L46-L54) and [`YPositionPercent`](source/UpbeatUI/View/PercentPlace.cs#L55-L63) properties are bound to `XPosition` and `YPosition` properties on the _ViewModel_ instance. These values can be arbitrary, but they could also be dynamic: for example if you would like a popup to be rendered directly on top of a button that opens it. To pass the relative location of that button back to the _ViewModel_ (so it can open a popup on that location), use the [`PercentPositionWithinUpbeatStackConverter`](source/UpbeatUI/View/Converters/PercentPositionWithinUpbeatStackConverter.cs) on the `CommandParameter`. For example, the following button is bound to the `ShowPopupCommand` command on the _ViewModel_:

```xml
<DataTemplate.Resources>
    <uvc:PercentPositionWithinUpbeatStackConverter x:Key="PositionConverter" />
</DataTemplate.Resources>
...
<Button
    Command="{Binding ShowPopupCommand}"
    CommandParameter="{Binding RelativeSource={RelativeSource Self}, Converter={StaticResource PositionConverter}}"
    Margin="5">Popup</Button>
```

[`PercentPositionWithinUpbeatStackConverter`](source/UpbeatUI/View/Converters/PercentPositionWithinUpbeatStackConverter.cs) works by converting the input value to the `Convert` method (in the above case, `{RelativeSource Self}`; i.e, the `Button` control) into a `Func<Point>` that the Command implemenation can call to get the percentage location of the control or element that invoked the command (the `Button`) within the [`UpbeatStack`'s](source/UpbeatUI/ViewModel/UpbeatStack.cs) area, like so (if using the [`CommunityToolkit.Mvvm` NuGet package](https://www.nuget.org/packages/CommunityToolkit.Mvvm)).

```c#
[RelayCommand]
private void ShowPopup(Func<Point> positionProvider)
{
    _upbeatService.OpenViewModel(
        new PopupMessageViewModel.Parameters()
        {
            Message = "Popup mesage",
            Position = positionProvider.Invoke(),
        });
}
```
