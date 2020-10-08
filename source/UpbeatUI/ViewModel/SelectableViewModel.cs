/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;
using System.Windows.Input;

namespace UpbeatUI.ViewModel
{
    public class SelectableViewModel : BaseViewModel, IUpdatableViewModel
    {
        public SelectableViewModel(PositionViewModel positionViewModel, Action select)
            :this(positionViewModel, new SizeViewModel(), select)
        { }

        public SelectableViewModel(PositionViewModel positionViewModel, SizeViewModel sizeViewModel, Action select)
        {
            PositionViewModel = positionViewModel;
            SizeViewModel = sizeViewModel;
            SelectCommand = new DelegateCommand(select);
        }

        public PositionViewModel PositionViewModel { get; }
        public ICommand SelectCommand { get; }
        public SizeViewModel SizeViewModel { get; }

        public void UpdateViewModelProperties()
        {
            PositionViewModel.UpdateViewModelProperties();
            SizeViewModel.UpdateViewModelProperties();
        }
    }
}
