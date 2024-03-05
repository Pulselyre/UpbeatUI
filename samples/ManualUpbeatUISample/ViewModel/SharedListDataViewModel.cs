/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UpbeatUI.ViewModel;
using UpbeatUI.ViewModel.ListSynchronize;

namespace ManualUpbeatUISample.ViewModel;

// This extends ObservableObject from the CommunityToolkit.Mvvm NuGet package, which provides pre-written SetProperty and OnPropertyChanged methods.
public sealed partial class SharedListDataViewModel : ObservableObject, IDisposable
{
    private readonly IUpbeatService _upbeatService;
    private readonly SharedList _sharedList;
    // Synchronizable collection is an extension of ObservableCollection
    private readonly ObservableCollection<string> _strings = new();

    public SharedListDataViewModel(
        IUpbeatService upbeatService, // This will be a unique IUpbeatService created and injected by the IUpbeatStack specifically for this ViewModel.
        SharedList sharedList) // This is a scoped service shared between this ViewModel and other ViewModel or scoped/transient service dependencies.
    {
        _upbeatService = upbeatService ?? throw new ArgumentNullException(nameof(upbeatService));
        _sharedList = sharedList ?? throw new ArgumentNullException(nameof(sharedList));

        Strings = new ReadOnlyObservableCollection<string>(_strings);
        _strings.Synchronize(_sharedList.Strings);

        _sharedList.StringAdded += SharedListStringAdded;
    }

    public INotifyCollectionChanged Strings { get; }

    // RelayCommand is an ICommand implementation from the CommunityToolkit.Mvvm NuGet package. As an attribute, it can be used to automatically wrap methods within ICommand properties. It supports both async and non-async methods/lambdas.
    [RelayCommand(CanExecute = nameof(CanAddString))]
    private async Task AddStringAsync(Func<Point> pointGetter)
    {
        ArgumentNullException.ThrowIfNull(pointGetter);
        string newString = null;
        await _upbeatService.OpenViewModelAsync(
            new TextEntryPopupViewModel.Parameters
            {
                Message = "Enter a string to add to the list:",
                ReturnCallback = s => newString = s,
                Position = pointGetter(),
            }).ConfigureAwait(true);
        if (!string.IsNullOrWhiteSpace(newString))
        {
            _sharedList.AddString(newString);
        }
    }

    private bool CanAddString(Func<Point> _) => _strings.Count < 10;

    public void Dispose() =>
        _sharedList.StringAdded -= SharedListStringAdded;

    private void SharedListStringAdded(object sender, EventArgs e) =>
        Application.Current.Dispatcher.Invoke(() => _strings.Synchronize(_sharedList.Strings)); // Ensure that the collection is changed on the UI thread

    // This nested Parameters class (full class name: "BottomViewModel.Parameters") is what other ViewModels will create instances of to tell the IUpbeatStack what type of child ViewModel to add to the stack.
    public class Parameters
    { }
}
