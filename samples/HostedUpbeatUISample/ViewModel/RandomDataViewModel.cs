/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using UpbeatUI.ViewModel;

namespace HostedUpbeatUISample.ViewModel
{
    // This extends BaseViewModel, which provides pre-written SetProperty and RaisePropertyChanged methods.
    internal class RandomDataViewModel : BaseViewModel, IDisposable
    {
        private const int MaxRandomLength = 15;
        private static readonly string RandomFormatString = new string ('0', MaxRandomLength);

        private readonly IUpbeatService _upbeatService;
        private readonly Random _random;
        private readonly SharedTimer _sharedTimer;
        private readonly ObservableCollection<KeyValuePair<string, string>> _data = new ObservableCollection<KeyValuePair<string, string>>();

        public RandomDataViewModel(
            // This will be a unique IUpbeatService created and injected by the IUpbeatStack specifically for this ViewModel.
            IUpbeatService upbeatService,
            // This will be an injected transient instance of a Random service.
            Random random,
            // This is a shared singleton service.
            SharedTimer sharedTimer)
        {
            _upbeatService = upbeatService ?? throw new ArgumentNullException(nameof(upbeatService));
            _random = random ?? throw new ArgumentNullException(nameof(random));
            _sharedTimer = sharedTimer ?? throw new ArgumentNullException(nameof(sharedTimer));

            _sharedTimer.Ticked += SharedTimerTicked;

            // DelegateCommand is a common convenience ICommand implementation to call methods or lambda expressions when the command is executed. It supports both async and non-async methods/lambdas.
            OpenPositionedPopupCommand = new DelegateCommand<Func<Point>>(
                // Create a Parameters object for a ViewModel and pass it to the IUpbeatStack using OpenViewModel. The IUpbeatStack will use the configured mappings to create the appropriate ViewModel from the Parameters type.
                pointGetter => _upbeatService.OpenViewModel(
                    new PopupViewModel.Parameters
                    {
                        Message = "This popup appears on top of\nthe button that opened it.",
                        // The pointGetter parameter is a Func<Point> created by the View that will return the position within the window of the control that executed this command. See the bindings in View\RandomDataControl.xaml for details on how to bind a pointGetter() as a CommandParameter.
                        Position = pointGetter(),
                    }));
            RefreshDataCommand = new DelegateCommand(RefreshData);

            Data = new ReadOnlyObservableCollection<KeyValuePair<string, string>>(_data);

            for (var i = 0; i < 100; i++)
                _data.Add(CreateRandomKeyValuePair());
        }

        private void RefreshData()
        {
            for (var i = 0; i < 100; i++)
                _data[i] = CreateRandomKeyValuePair();
        }

        public ICommand OpenPositionedPopupCommand { get; }
        public ICommand RefreshDataCommand { get; }
        public ReadOnlyObservableCollection<KeyValuePair<string, string>> Data { get; }
        public string SecondsElapsed => $"{_sharedTimer.ElapsedSeconds} Seconds";

        public void Dispose() =>
            _sharedTimer.Ticked -= SharedTimerTicked;

        private KeyValuePair<string, string> CreateRandomKeyValuePair() =>
            new KeyValuePair<string, string>(
                $"{(_random.NextDouble() * Math.Pow(10, MaxRandomLength)).ToString(RandomFormatString)}.{(_random.NextDouble() * Math.Pow(10, MaxRandomLength)).ToString(RandomFormatString)}",
                $"{(_random.NextDouble() * Math.Pow(10, MaxRandomLength)).ToString(RandomFormatString)}.{(_random.NextDouble() * Math.Pow(10, MaxRandomLength)).ToString(RandomFormatString)}");

        private void SharedTimerTicked(object sender, EventArgs e) =>
            // Ensure that the PropertyChanged event is raised on the UI thread
            Application.Current.Dispatcher.Invoke(() => RaisePropertyChanged(nameof(SecondsElapsed)));

        // This nested Parameters class (full class name: "RandomDataViewModel.Parameters") is what other ViewModels will create instances of to tell the IUpbeatStack what type of child ViewModel to add to the stack.
        public class Parameters
        { }
    }
}
