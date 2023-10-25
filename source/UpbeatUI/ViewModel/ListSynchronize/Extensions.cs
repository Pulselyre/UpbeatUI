/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System;
using System.Collections.Generic;
using System.Linq;

namespace UpbeatUI.ViewModel.ListSynchronize
{
    public static class Extensions
    {
        public static void Synchronize<TSyncable>(
            this IList<TSyncable> items,
            params IEnumerable<TSyncable>[] sources
        )
        {
            var list = sources.SelectMany(a => a).ToList();
            int count = items.Count;
            for (int i = 0; i < Math.Max(list.Count, count); i++)
            {
                if (i >= count)
                {
                    items.Add(list[i]);
                }
                else if (i >= list.Count)
                    items.RemoveAt(items.Count - 1);
                else
                    items[i] = list[i];
            }
        }

        public static void Synchronize<TSource, TSyncable>(
            this IList<TSyncable> items,
            Action<TSource, TSyncable> synchronizer,
            params IEnumerable<TSource>[] sources
        ) where TSyncable : class, new()
        {
            items.Synchronize(
                () => new TSyncable(),
                synchronizer,
                sources
            );
        }

        public static void Synchronize<TSource, TSyncable>(
            this IList<TSyncable> items,
            Func<TSyncable> blankCreator,
            Action<TSource, TSyncable> synchronizer,
            params IEnumerable<TSource>[] sources
        ) where TSyncable : class
        {
            items.Synchronize(
                s =>
                {
                    var p = blankCreator();
                    synchronizer(s, p);
                    return p;
                }, synchronizer, sources);
        }

        public static void Synchronize<TSource, TSyncable>(
            this IList<TSyncable> items,
            Func<TSource, TSyncable> creator,
            Action<TSource, TSyncable> synchronizer,
            params IEnumerable<TSource>[] sources
        ) where TSyncable : class
        {
            var list = sources.SelectMany(a => a).ToList();
            int count = items.Count;
            for (int i = 0; i < Math.Max(list.Count, count); i++)
            {
                if (i >= count)
                {
                    items.Add(creator(list[i]));
                }
                else if (i >= list.Count)
                    items.RemoveAt(items.Count - 1);
                else
                    synchronizer(list[i], items[i]);
            }
        }
    }

    // public class SynchronizableCollection<TSyncable, TSource> : ObservableCollection<TSyncable>
    // {
    //     private Func<TSyncable> _blankCreator;
    //     private Func<TSource, TSyncable> _initializedCreator;
    //     private Action<TSource, TSyncable> _synchronizer;

    //     public SynchronizableCollection(Action<TSource, TSyncable> synchronizer)
    //     {
    //         if (typeof(TSyncable).GetConstructor(Type.EmptyTypes) == null)
    //             throw new Exception("Type of " + typeof(TSyncable).FullName + " does not provide a default constructor.");
    //         _synchronizer = synchronizer;
    //     }

    //     public SynchronizableCollection(Func<TSyncable> blankCreator, Action<TSource, TSyncable> synchronizer)
    //     {
    //         _blankCreator = blankCreator;
    //         _synchronizer = synchronizer;
    //     }

    //     public SynchronizableCollection(Func<TSource, TSyncable> initializedCreator, Action<TSource, TSyncable> synchronizer)
    //     {
    //         _synchronizer = synchronizer;
    //         _initializedCreator = initializedCreator;
    //     }

    //     public void Synchronize(params IEnumerable<TSource>[] sources)
    //     {
    //         var list = sources.SelectMany(a => a).ToList();
    //         int count = Count;
    //         for (int i = 0; i < Math.Max(list.Count, count); i++)
    //         {
    //             if (i >= count)
    //             {
    //                 Add(
    //                     _blankCreator != null ? _blankCreator()
    //                     : _initializedCreator != null ? _initializedCreator(list[i])
    //                     : (TSyncable)typeof(TSyncable).GetConstructor(Type.EmptyTypes).Invoke(null));
    //                 if (_initializedCreator == null)
    //                     _synchronizer(list[i], this[i]);
    //             }
    //             else if (i >= list.Count)
    //                 RemoveAt(Count - 1);
    //             else
    //                 _synchronizer(list[i], this[i]);
    //         }
    //     }
    // }
}
