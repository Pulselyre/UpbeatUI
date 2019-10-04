using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace UpbeatUI
{
    public class MainContext : ObservableObject, IDisposable, IUpdatableContext
    {
        private IDictionary<Type, Type> _contextControlMappings = new Dictionary<Type, Type>();

        public MainContext(Action contextsEmptyCallback)
        {
            ContextStack = new ContextStack(contextsEmptyCallback);
            ContextControlMappings = new ReadOnlyDictionary<Type, Type>(_contextControlMappings);
        }

        public ContextStack ContextStack { get; }
        public IReadOnlyDictionary<Type, Type> ContextControlMappings { get; }

        public void Dispose()
            => ContextStack.Dispose();

        public void SetContextControlMapping(Type contextType, Type viewType)
        {
            if (contextType == null || viewType == null)
                throw new ArgumentNullException();
            if (!typeof(IContext).IsAssignableFrom(contextType))
                throw new ArgumentException("contextType must implement the IContext interface.");
            if (!typeof(UIElement).IsAssignableFrom(viewType))
                throw new ArgumentException("viewType must extend the UIElement class.");
            _contextControlMappings[contextType] = viewType;
        }

        public void SetContextControlMappings(IDictionary<Type, Type> mappings)
        {
            foreach (var kvp in mappings)
                SetContextControlMapping(kvp.Key, kvp.Value);
        }

        public void UpdateContextProperties()
            => ContextStack.UpdateContextProperties();
    }
}
