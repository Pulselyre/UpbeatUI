/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UpbeatUI.ViewModel;

namespace ManualUpbeatUISample.ViewModel;

// This extends ObservableObject from the CommunityToolkit.Mvvm NuGet package, which provides pre-written SetProperty and OnPropertyChanged methods.
internal sealed class RandomDataViewModel : ObservableObject, IDisposable
{
    private const int MaxRandomLength = 14;

    private readonly IUpbeatService _upbeatService;
    private readonly RandomNumberGenerator _random;
    private readonly SharedTimer _sharedTimer;
    private readonly ObservableCollection<KeyValuePair<string, string>> _data = new();

    public RandomDataViewModel(
        IUpbeatService upbeatService, // This will be a unique IUpbeatService created and injected by the IUpbeatStack specifically for this ViewModel.
        RandomNumberGenerator random, // This will be an injected transient instance of a Random service.
        SharedTimer sharedTimer) // This is a shared singleton service.
    {
        _upbeatService = upbeatService ?? throw new ArgumentNullException(nameof(upbeatService));
        _random = random ?? throw new ArgumentNullException(nameof(random));
        _sharedTimer = sharedTimer ?? throw new ArgumentNullException(nameof(sharedTimer));

        _sharedTimer.Ticked += SharedTimerTicked;

        // RelayCommand is an ICommand implementation from the CommunityToolkit.Mvvm NuGet package. It can be used to call methods or lambda expressions when the command is executed. It supports both async and non-async methods/lambdas.
        OpenPositionedPopupCommand = new RelayCommand<Func<Point>>(
            pointGetter => _upbeatService.OpenViewModel( // Create a Parameters object for a ViewModel and pass it to the IUpbeatStack using OpenViewModel. The IUpbeatStack will use the configured mappings to create the appropriate ViewModel from the Parameters type.
                new PopupViewModel.Parameters
                {
                    Message = "This popup appears on top of\nthe button that opened it.",
                    // The pointGetter parameter is a Func<Point> created by the View that will return the position within the window of the control that executed this command. See the bindings in View\RandomDataControl.xaml for details on how to bind a pointGetter() as a CommandParameter.
                    Position = pointGetter(),
                }));
        RefreshDataCommand = new RelayCommand(RefreshData);

        Data = new ReadOnlyObservableCollection<KeyValuePair<string, string>>(_data);

        for (var i = 0; i < 100; i++)
        {
            _data.Add(CreateRandomKeyValuePair());
        }
    }

    private void RefreshData()
    {
        for (var i = 0; i < 100; i++)
        {
            _data[i] = CreateRandomKeyValuePair();
        }
    }

    public ICommand OpenPositionedPopupCommand { get; }
    public ICommand RefreshDataCommand { get; }
    public ReadOnlyObservableCollection<KeyValuePair<string, string>> Data { get; }
    public string SecondsElapsed => $"{_sharedTimer.ElapsedSeconds} Seconds";

    public void Dispose() =>
        _sharedTimer.Ticked -= SharedTimerTicked;

    private string RandomString()
    {
        var bytes = new byte[MaxRandomLength];
        _random.GetNonZeroBytes(bytes);
        return BitConverter.ToString(bytes).Replace("-", "", StringComparison.Ordinal);
    }

    private KeyValuePair<string, string> CreateRandomKeyValuePair() =>
        new(RandomString(), RandomString());

    private void SharedTimerTicked(object sender, EventArgs e) =>
        Application.Current.Dispatcher.Invoke(() => OnPropertyChanged(nameof(SecondsElapsed))); // Ensure that the PropertyChanged event is raised on the UI thread

    // This nested Parameters class (full class name: "RandomDataViewModel.Parameters") is what other ViewModels will create instances of to tell the IUpbeatStack what type of child ViewModel to add to the stack.
    public sealed class Parameters
    { }
}
