# How to use UpbeatUI

The sections below provide:

1. Guides on some of the options for initializing UpbeatUI and configuring the [**UpbeatStack**](source/UpbeatUI/ViewModel/UpbeatStack.cs).
2. Details on some of the functions that the [**IUpbeatService**](source/UpbeatUI/ViewModel/IUpbeatService.cs) provides to ViewModels.
3. Information on how Views should be designed for best compatibility with UpbeatUI.

## Initiating with **IHostBuilder**

To create an UpbeatUI app using **IHostBuilder** (the simpler method), a *ConfigureUpbeatHost* extension method is provided in the UpbeatUI.Extensions.Hosting Nuget package. The method takes a required delegate parameter to create a ViewModelParameters object for the main/bottom ViewModel. *ConfigureUpbeatHost* also takes an optional delegate parameter that provides an [**IHostedUpbeatBuilder**](source/UpbeatUI.Extensions.Hosting/IHostedUpbeatBuilder.cs) to set additional options in the application, such as specifying a custom containing window or setting specific ViewModelParameters-ViewModel-View type mappings.

The below code automatically creates a default [**UpbeatMainWindow**](source/UpbeatUI/View/UpbeatMainWindow.xaml) with an  [**UpbeatStack**](source/UpbeatUI/ViewModel/UpbeatStack.cs) as its *DataContext*. A delegate to create a "BottomViewModel.Parameters" object is supplied, which UpbeatUI will use to create a "BottomViewModel" and display a "BottomControl" as the window's main/bottom layer. (If the "BottomViewModel" needs access to any services or configuration objects, they can be supplied via dependency injection similarly to Controllers in ASP.NET Core.)

```C#
private static void Main(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureUpbeatHost(() => new BottomViewModel.Parameters())
        .Build()
        .Run();
```

By default, using UpbeatUI with **IHostBuilder** enables automatic mapping between ViewModelParameters, ViewModels, and Views. The default namespace and naming conventions are below, but the [**IHostedUpbeatBuilder**](source/UpbeatUI.Extensions.Hosting/IHostedUpbeatBuilder.cs) provides methods to set the mapping conventions manually (either from Type-to-Type or AssemblyQualifiedName-to-AssemblyQualifiedName). Specific type relationships can also be manually mapped with the [**IHostedUpbeatBuilder**](source/UpbeatUI.Extensions.Hosting/IHostedUpbeatBuilder.cs). The [**UpbeatStack**](source/UpbeatUI/ViewModel/UpbeatStack.cs) will always check for manually specified mappings first.

|                     | Naming Convention                                      | Example Class Name                                       |
|--------------------:|--------------------------------------------------------|----------------------------------------------------------|
| ViewModelParameters | "{BaseNamespace}.ViewModel.{Name}ViewModel+Parameters" | SampleProject.ViewModel.PopupMessageViewModel+Parameters |
|           ViewModel | "{BaseNamespace}.ViewModel.{Name}ViewModel"            | SampleProject.ViewModel.PopupMessageViewModel            |
|                View | "{BaseNamespace}.View.{Name}Control"                   | SampleProject.View.PopupMessageControl                   |

>Note: In the default convention, the ViewModelParameters class must be a nested class within the ViewModel class (hence, the '+').

## Initiating Manually

To start an UpbeatUI application without **IHostBuilder**, both the containing window and the [**UpbeatStack**](source/UpbeatUI/ViewModel/UpbeatStack.cs) must be created, configured, and wired together manually. If the application uses an **IServiceProvider**, mapping by convention is possible using the *SetDefaultViewModelLocators* method. Otherwise, mappings between ViewModelParameters, ViewModels, and Views will also need to be manually set and factory methods provided. The process is considerably more involved, so please see the comments in the [**Program.cs**](samples/basicsample/BasicUpbeatUISample/Program.cs) file in the (basic sample)(samples/basicsample) for a guidance.

## Using [**IUpbeatService**](source/UpbeatUI/ViewModel/IUpbeatService.cs)

The [**IUpbeatService**](source/UpbeatUI/ViewModel/IUpbeatService.cs) interface provides functionality for ViewModels to interact with their parent [**UpbeatStack**](source/UpbeatUI/ViewModel/UpbeatStack.cs).

### Opening New ViewModels

The most often used function is to open new child ViewModels on the [**UpbeatStack**](source/UpbeatUI/ViewModel/UpbeatStack.cs) via the *OpenViewModel* or *OpenViewModelAsync* methods.

For example: if the currently active ViewModel wanted to display a popup message to the user, then it might want to open a "PopupMessageViewModel" on the [**UpbeatStack**](source/UpbeatUI/ViewModel/UpbeatStack.cs). To do so, it would pass a "PopupMessageViewModel.Parameters" object with a 'PopupMessage' string property to the *OpenViewModel* or *OpenViewModelAsync* methods:

```C#
// Note: OpenViewModelAsync returns a Task that completes when the opened ViewModel is closed
await _upbeatService.OpenViewModelAsync(
    new PopupMessageViewModel.Parameters
    {
        PopupMessage = "Message to display in popup"
    });
```

The [**UpbeatStack**](source/UpbeatUI/ViewModel/UpbeatStack.cs) will create a new "PopupMessageViewModel" (assuming the types are correctly mapped) by passing the submitted "PopupMessageViewModel.Parameters" object in the constructor for initialization (and also a new unique [**IUpbeatService**](source/UpbeatUI/ViewModel/IUpbeatService.cs). A "PopupMessageControl" will displayed to the user with the new "PopupMessageViewModel" set as its *DataContext*, so a TextBox could be bound to display the 'PopupMessage' property.

### Intercepting View and Window Close Events

ViewModels that hold ongoing work that has not been saved or committed to a database might want confirm with the user before being closed. This is accomplished with the *SetCloseCallback* method on the [**IUpbeatService**](source/UpbeatUI/ViewModel/IUpbeatService.cs). This method accepts an 'okToClose' delegate that the [**UpbeatStack**](source/UpbeatUI/ViewModel/UpbeatStack.cs) will call before attempting to close the ViewModel. The delegate should return True if the ViewModel can close, and False if not. Returning False can prevent the entire application from stopping (at least in orderly shutdown situations). The 'okToClose' delegate can be synchronous or async.

## Constructing Views using [**UpbeatControl**](source/UpbeatUI/View/UpbeatControl.cs)

UpbeatUI provides a base class for Views named [**UpbeatControl**](source/UpbeatUI/View/UpbeatControl.cs). Using it is similar to extending WPF's **UserControl**, except there are several additional properties that can be set to define where the View's content is rendered within the window. For all views except the bottom layer, it is recommended to leave empty space around the sides that the user can touch/click to close the active View. The properties are:

1. *WidthPercent* - Defines the amount of viewable horizontal space that the content will fill.
2. *HeightPercent* - Defines the amount of viewable vertical space that the content will fill.
3. *XPositionPercent* - Defines the percentage point horizontally within the viewable space that the content should be centered on.
4. *YPositionPercent* - Defines the percentage point vertically within the viewable space that the content should be centered on.
5. *KeepInBounds* - Defines whether the content should be kept within the viewable space or not. This is only relevent if the content is not centered.

Both *WidthPercent* and *HeightPercent* accept either one or two values. One value, such as "50%", sets the content to always fill exactly half of the viewable space. Two values, such as "10% 90%", sets the content to fill its desired size, but no smaller than 10% of the viewable space and no larger than 90% of the viewable space. The default behavior when *WidthPercent* or *HeightPercent* are unset is for content to fill its desired space, but no larger than viewable space.

>Note: All four percent properties can accept numbers in percent format ("50%") or decimal format ("0.5").

See the various [.xaml files](samples/basicsample/BasicUpbeatUISample/View/) in the sample projects for examples on how to configure an [**UpbeatControl**](source/UpbeatUI/View/UpbeatControl.cs).
