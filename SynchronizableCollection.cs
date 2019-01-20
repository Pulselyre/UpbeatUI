using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace UpbeatUI
{
    public class SynchronizableCollection<TSyncable> : ObservableCollection<TSyncable>
    {
        public void Synchronize<TSource>(Action<TSource, TSyncable> synchronizer, params IEnumerable<TSource>[] sources)
        {
            if (typeof(TSyncable).GetConstructor(Type.EmptyTypes) == null)
                throw new Exception("Type of " + typeof(TSyncable).Name + " does not provide a default constructor.");
            Synchronize(
                () =>
                {
                    var p = (TSyncable)typeof(TSyncable).GetConstructor(Type.EmptyTypes).Invoke(null);
                    return p;
                }, synchronizer, sources);
        }

        public void Synchronize<TSource>(Func<TSyncable> blankCreator, Action<TSource, TSyncable> synchronizer, params IEnumerable<TSource>[] sources)
        {
            Synchronize(
                s =>
                {
                    var p = blankCreator();
                    synchronizer(s, p);
                    return p;
                }, synchronizer, sources);
        }

        public void Synchronize<TSource>(Func<TSource, TSyncable> creator, Action<TSource, TSyncable> synchronizer, params IEnumerable<TSource>[] sources)
        {
            var list = sources.SelectMany(a => a).ToList();
            int count = Count;
            for (int i = 0; i < Math.Max(list.Count, count); i++)
            {
                if (i >= count)
                {
                    Add(creator(list[i]));
                }
                else if (i >= list.Count)
                    RemoveAt(Count - 1);
                else
                    synchronizer(list[i], this[i]);
            }
        }
    }

    public class SynchronizableCollection<TSyncable, TSource> : ObservableCollection<TSyncable>
    {
        private Func<TSyncable> _blankCreator;
        private Func<TSource, TSyncable> _initializedCreator;
        private Action<TSource, TSyncable> _synchronizer;

        public SynchronizableCollection(Action<TSource, TSyncable> synchronizer)
        {
            if (typeof(TSyncable).GetConstructor(Type.EmptyTypes) == null)
                throw new Exception("Type of " + typeof(TSyncable).Name + " does not provide a default constructor.");
            _synchronizer = synchronizer;
        }

        public SynchronizableCollection(Func<TSyncable> blankCreator, Action<TSource, TSyncable> synchronizer)
        {
            _blankCreator = blankCreator;
            _synchronizer = synchronizer;
        }

        public SynchronizableCollection(Func<TSource, TSyncable> initializedCreator, Action<TSource, TSyncable> synchronizer)
        {
             _synchronizer = synchronizer;
            _initializedCreator = initializedCreator;
         }

    public void Synchronize(params IEnumerable<TSource>[] sources)
        {
            var list = sources.SelectMany(a => a).ToList();
            int count = Count;
            for (int i = 0; i < Math.Max(list.Count, count); i++)
            {
                if (i >= count)
                {
                    Add(
                        _blankCreator != null ? _blankCreator()
                        : _initializedCreator != null ? _initializedCreator(list[i])
                        : (TSyncable)typeof(TSyncable).GetConstructor(Type.EmptyTypes).Invoke(null));
                    if (_initializedCreator == null)
                        _synchronizer(list[i], this[i]);
                }
                else if (i >= list.Count)
                    RemoveAt(Count - 1);
                else
                    _synchronizer(list[i], this[i]);
            }
        }
    }
}
