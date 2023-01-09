/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using UpbeatUI.ViewModel;

namespace ServiceProvidedUpbeatUISample.ViewModel;

// This extends BaseViewModel, which provides pre-written SetProperty and RaisePropertyChanged methods.
public class SharedListDataViewModel : BaseViewModel, IDisposable
{
    private readonly IUpbeatService _upbeatService;
    private readonly SharedList _sharedList;
    // Synchronizable collection is an extension of ObservableCollection
    private readonly SynchronizableCollection<string> _strings = new();

    public SharedListDataViewModel(
    // This will be a unique IUpbeatService created and injected by the IUpbeatStack specifically for this ViewModel.
        IUpbeatService upbeatService,
    // This is a scoped service shared between this ViewModel and other ViewModel or scoped/transient service dependencies.
        SharedList sharedList)
    {
        _upbeatService = upbeatService ?? throw new ArgumentNullException(nameof(upbeatService));
        _sharedList = sharedList ?? throw new ArgumentNullException(nameof(sharedList));

        Strings = new ReadOnlyObservableCollection<string>(_strings);
        _strings.Synchronize(_sharedList.Strings);

        _sharedList.StringAdded += SharedListStringAdded;

        // DelegateCommand is a common convenience ICommand implementation to call methods or lambda expressions when the command is executed. It supports both async and non-async methods/lambdas.
        AddStringCommand = new DelegateCommand<Func<Point>>(ExecuteAddStringAsync, pg => _strings.Count < 10);
    }

    public INotifyCollectionChanged Strings { get; }
    public ICommand AddStringCommand { get; }

    public void Dispose() =>
        _sharedList.StringAdded -= SharedListStringAdded;

    private async Task ExecuteAddStringAsync(Func<Point> pointGetter)
    {
        string newString = null;
        await _upbeatService.OpenViewModelAsync(
            new TextEntryPopupViewModel.Parameters
            {
                Message = "Enter a string to add to the list:",
                ReturnCallback = s => newString = s,
                Position = pointGetter(),
            });
        if (!string.IsNullOrWhiteSpace(newString))
            _sharedList.AddString(newString);
    }

    private void SharedListStringAdded(object sender, EventArgs e) =>
        Application.Current.Dispatcher.Invoke(() => _strings.Synchronize(_sharedList.Strings)); // Ensure that the collection is changed on the UI thread

    // This nested Parameters class (full class name: "BottomViewModel.Parameters") is what other ViewModels will create instances of to tell the IUpbeatStack what type of child ViewModel to add to the stack.
    public class Parameters
    { }
}
