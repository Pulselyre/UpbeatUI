using System;

namespace UpbeatUI
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

    public static class LocatorExtensions
    {
        public static Locator<TSource, TProduct> SubLocate<TSource, TProduct>(this TSource source, Func<TSource, TProduct> locate)
            => new Locator<TSource, TProduct>(locate);

        public static Locator<TSource, TProduct> SubLocate<TSource, TProduct>(this TSource source, Predicate<TSource> exists, Func<TSource, TProduct> locate)
            => new Locator<TSource, TProduct>(exists, locate);
    }
}
