/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System;
using System.Collections.Generic;
using System.Linq;

namespace UpbeatUI.ViewModel.ListSynchronize
{
    public static class ExtensionMethods
    {
        public static void Synchronize<TSyncable>(
            this IList<TSyncable> items,
            params IEnumerable<TSyncable>[] sources
        )
        {
            var list = sources.SelectMany(a => a).ToList();
            var count = items?.Count ?? throw new ArgumentNullException(nameof(items));
            for (var i = 0; i < Math.Max(list.Count, count); i++)
            {
                if (i >= count)
                {
                    items.Add(list[i]);
                }
                else if (i >= list.Count)
                {
                    items.RemoveAt(items.Count - 1);
                }
                else
                {
                    items[i] = list[i];
                }
            }
        }

        public static void Synchronize<TSource, TSyncable>(
            this IList<TSyncable> items,
            Action<TSource, TSyncable> synchronizer,
            params IEnumerable<TSource>[] sources
        ) where TSyncable : class, new() => items.Synchronize(
                () => new TSyncable(),
                synchronizer,
                sources
            );

        public static void Synchronize<TSource, TSyncable>(
            this IList<TSyncable> items,
            Func<TSyncable> blankCreator,
            Action<TSource, TSyncable> synchronizer,
            params IEnumerable<TSource>[] sources
        ) where TSyncable : class => items.Synchronize(
                s =>
                {
                    var p = blankCreator();
                    synchronizer(s, p);
                    return p;
                }, synchronizer, sources);

        public static void Synchronize<TSource, TSyncable>(
            this IList<TSyncable> items,
            Func<TSource, TSyncable> creator,
            Action<TSource, TSyncable> synchronizer,
            params IEnumerable<TSource>[] sources
        ) where TSyncable : class
        {
            _ = creator ?? throw new ArgumentNullException(nameof(creator));
            _ = synchronizer ?? throw new ArgumentNullException(nameof(synchronizer));
            var list = sources.SelectMany(a => a).ToList();
            var count = items?.Count ?? throw new ArgumentNullException(nameof(items));
            for (var i = 0; i < Math.Max(list.Count, count); i++)
            {
                if (i >= count)
                {
                    items.Add(creator(list[i]));
                }
                else if (i >= list.Count)
                {
                    items.RemoveAt(items.Count - 1);
                }
                else
                {
                    synchronizer(list[i], items[i]);
                }
            }
        }
    }
}
