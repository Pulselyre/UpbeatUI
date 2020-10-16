/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;

namespace UpbeatUI.ViewModel.Locator
{
    public class Locator<TSource, TProduct>
    {
        private Predicate<TSource> _exists;
        private Func<TSource, TProduct> _locate;

        public Locator(Func<TSource, TProduct> locate)
            : this(source => true, locate)
        { }

        public Locator(Predicate<TSource> exists, Func<TSource, TProduct> locate)
        {
            _exists = exists;
            _locate = locate;
        }

        public bool Exists(TSource source)
            => _exists(source);

        public TProduct Locate(TSource source)
            => _locate(source);

        public Locator<TSource, TChild> SubLocate<TChild>(Func<TProduct, TChild> locate)
            => new Locator<TSource, TChild>(source => true, source => locate(_locate(source)));

        public Locator<TSource, TChild> SubLocate<TChild>(Predicate<TProduct> exists, Func<TProduct, TChild> locate)
            => new Locator<TSource, TChild>(source => _exists(source) ? exists(_locate(source)) : false, source => locate(_locate(source)));
    }
}
