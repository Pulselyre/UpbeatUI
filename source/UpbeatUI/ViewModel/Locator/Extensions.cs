/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;

namespace UpbeatUI.ViewModel.Locator
{
    public static class LocatorExtensions
    {
        public static Locator<TSource, TProduct> SubLocate<TSource, TProduct>(this TSource source, Func<TSource, TProduct> locate)
            => new Locator<TSource, TProduct>(locate);

        public static Locator<TSource, TProduct> SubLocate<TSource, TProduct>(this TSource source, Predicate<TSource> exists, Func<TSource, TProduct> locate)
            => new Locator<TSource, TProduct>(exists, locate);
    }
}
