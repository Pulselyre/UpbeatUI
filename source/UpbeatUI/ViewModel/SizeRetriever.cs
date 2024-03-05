/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System;
using System.Windows;
using UpbeatUI.View;

namespace UpbeatUI.ViewModel
{
    [Obsolete("'SizeRetriever' class is deprecated and will be removed in the next major release; consider using the 'PercentPositionWithinUpbeatStackConverter' converter and CommandParameter binding approach to receive to position information instead.")]
    public class SizeRetriever
    {
        public Size Size => Retriever?.Invoke() ?? throw new InvalidCastException($"This {nameof(SizeRetriever)} has not been bound to an {nameof(AttachedSizeAndPosition.SizeRetrieverProperty)}.");

        internal Func<Size> Retriever { get; set; }
    }
}
