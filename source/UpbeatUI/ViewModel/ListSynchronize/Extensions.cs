/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System;
using System.Collections.Generic;

namespace UpbeatUI.ViewModel.ListSynchronize
{
    /// <summary>
    /// Extension methods to assist with synchronizing items from an <see cref="IEnumerable{T}"/> to an <see cref="IList{T}"/>.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Synchronizes the values from an <see cref="IEnumerable{T}"/> <paramref name="sources"/> to the target <see cref="IList{T}"/> <paramref name="items"/>. Useful when the target is an <see cref="System.Collections.ObjectModel.ObservableCollection{T}"/> that needs to be initially filled or its values overwritten.
        /// </summary>
        /// <typeparam name="TSyncable">The type of elelments in the target <see cref="IList{T}"/> and source <see cref="IEnumerable{T}"/></typeparam>
        /// <param name="items">The target <see cref="IList{T}"/> that elements will be synchrnonized too.</param>
        /// <param name="sources">The <see cref="IEnumerable{T}"/> that elements will be synchronized from.</param>
        /// <param name="cleaner">An optional method that will be executed on elements being removed. Useful if items need to be disposed or otherwise cleaned up.</param>
        /// <exception cref="ArgumentNullException">If any required parameters are null references.</exception>
        public static void Synchronize<TSyncable>(
            this IList<TSyncable> items,
            IEnumerable<TSyncable> sources,
            Action<TSyncable> cleaner = null
        )
        {
            _ = items ?? throw new ArgumentNullException(nameof(items));
            _ = sources ?? throw new ArgumentNullException(nameof(sources));
            var i = 0;
            foreach (var source in sources)
            {
                if (i == items.Count)
                {
                    items.Add(source);
                }
                else
                {
                    items[i] = source;
                }
                i++;
            }
            while (items.Count > i)
            {
                cleaner?.Invoke(items[items.Count - 1]);
                items.RemoveAt(items.Count - 1);
            }
        }

        /// <summary>
        /// Synchronizes the values from an <see cref="IEnumerable{T}"/> <paramref name="sources"/> to the target <see cref="IList{T}"/> <paramref name="items"/>. Useful when the target is an <see cref="System.Collections.ObjectModel.ObservableCollection{T}"/> that needs to be initially filled or its values overwritten.
        /// </summary>
        /// <typeparam name="TSource">The type of elements in the source <see cref="IEnumerable{T}"/>.</typeparam>
        /// <typeparam name="TSyncable">The type of elelments in the target <see cref="IList{T}"/>.</typeparam>
        /// <param name="items">The target <see cref="IList{T}"/> that elements will be synchrnonized too.</param>
        /// <param name="sources">The <see cref="IEnumerable{T}"/> that elements will be synchronized from.</param>
        /// <param name="synchronizer">A method that will be executed to synchronize values from a <typeparamref name="TSource"/> item to a <typeparamref name="TSyncable"/> item.</param>
        /// <param name="cleaner">An optional method that will be executed on elements being removed. Useful if items need to be disposed or otherwise cleaned up.</param>
        public static void Synchronize<TSource, TSyncable>(
            this IList<TSyncable> items,
            IEnumerable<TSource> sources,
            Action<TSource, TSyncable> synchronizer,
            Action<TSyncable> cleaner = null
        ) where TSyncable : class, new() =>
            items.Synchronize(
                sources,
                () => new TSyncable(),
                synchronizer,
                cleaner);

        /// <summary>
        /// Synchronizes the values from an <see cref="IEnumerable{T}"/> <paramref name="sources"/> to the target <see cref="IList{T}"/> <paramref name="items"/>. Useful when the target is an <see cref="System.Collections.ObjectModel.ObservableCollection{T}"/> that needs to be initially filled or its values overwritten.
        /// </summary>
        /// <typeparam name="TSource">The type of elements in the source <see cref="IEnumerable{T}"/>.</typeparam>
        /// <typeparam name="TSyncable">The type of elelments in the target <see cref="IList{T}"/>.</typeparam>
        /// <param name="items">The target <see cref="IList{T}"/> that elements will be synchrnonized too.</param>
        /// <param name="sources">The <see cref="IEnumerable{T}"/> that elements will be synchronized from.</param>
        /// <param name="blankCreator">A method that will be execute to create a new <typeparamref name="TSyncable"/>, which will then have <paramref name="synchronizer"/> executed on it with a <typeparamref name="TSource"/>.</param>
        /// <param name="synchronizer">A method that will be executed to synchronize values from a <typeparamref name="TSource"/> item to a <typeparamref name="TSyncable"/> item.</param>
        /// <param name="cleaner">An optional method that will be executed on elements being removed. Useful if items need to be disposed or otherwise cleaned up.</param>
        public static void Synchronize<TSource, TSyncable>(
            this IList<TSyncable> items,
            IEnumerable<TSource> sources,
            Func<TSyncable> blankCreator,
            Action<TSource, TSyncable> synchronizer,
            Action<TSyncable> cleaner = null
        ) where TSyncable : class =>
            items.Synchronize(
                sources,
                s =>
                {
                    var p = blankCreator();
                    synchronizer(s, p);
                    return p;
                },
                synchronizer,
                cleaner);

        /// <summary>
        /// Synchronizes the values from an <see cref="IEnumerable{T}"/> <paramref name="sources"/> to the target <see cref="IList{T}"/> <paramref name="items"/>. Useful when the target is an <see cref="System.Collections.ObjectModel.ObservableCollection{T}"/> that needs to be initially filled or its values overwritten.
        /// </summary>
        /// <typeparam name="TSource">The type of elements in the source <see cref="IEnumerable{T}"/>.</typeparam>
        /// <typeparam name="TSyncable">The type of elelments in the target <see cref="IList{T}"/>.</typeparam>
        /// <param name="items">The target <see cref="IList{T}"/> that elements will be synchrnonized too.</param>
        /// <param name="sources">The <see cref="IEnumerable{T}"/> that elements will be synchronized from.</param>
        /// <param name="creator">A method that will execute to create a new <typeparamref name="TSyncable"/> from a <typeparamref name="TSource"/>.</param>
        /// <param name="synchronizer">A method that will be executed to synchronize values from a <typeparamref name="TSource"/> item to a <typeparamref name="TSyncable"/> item.</param>
        /// <param name="cleaner">An optional method that will be executed on elements being removed. Useful if items need to be disposed or otherwise cleaned up.</param>
        /// <exception cref="ArgumentNullException">If any required parameters are null references.</exception>
        public static void Synchronize<TSource, TSyncable>(
            this IList<TSyncable> items,
            IEnumerable<TSource> sources,
            Func<TSource, TSyncable> creator,
            Action<TSource, TSyncable> synchronizer,
            Action<TSyncable> cleaner = null
        ) where TSyncable : class
        {
            _ = items ?? throw new ArgumentNullException(nameof(items));
            _ = sources ?? throw new ArgumentNullException(nameof(sources));
            _ = creator ?? throw new ArgumentNullException(nameof(creator));
            _ = synchronizer ?? throw new ArgumentNullException(nameof(synchronizer));
            var i = 0;
            foreach (var source in sources)
            {
                if (i == items.Count)
                {
                    items.Add(creator(source));
                }
                else
                {
                    synchronizer(source, items[i]);
                }
                i++;
            }
            while (items.Count > i)
            {
                cleaner?.Invoke(items[items.Count - 1]);
                items.RemoveAt(items.Count - 1);
            }
        }
    }
}
