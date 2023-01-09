/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System;
using System.Windows;
using UpbeatUI.View;

namespace UpbeatUI.ViewModel
{
    public class SizeRetriever
    {
        public Size Size => Retriever?.Invoke() ?? throw new InvalidCastException($"This {nameof(SizeRetriever)} has not been bound to an {nameof(AttachedSizeAndPosition.SizeRetrieverProperty)}.");

        internal Func<Size> Retriever { get; set; }
    }
}
