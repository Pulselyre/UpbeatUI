/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System.ComponentModel;

namespace UpbeatUI.ViewModel
{
    public interface IUpdatableViewModel : INotifyPropertyChanged
    {
        void UpdateViewModelProperties();
    }
}
