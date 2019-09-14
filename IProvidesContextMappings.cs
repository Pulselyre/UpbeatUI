using System;
using System.Collections.Generic;

namespace UpbeatUI
{
    public interface IProvidesContextMappings
    {
        event EventHandler MainControlMappingsUpdated;

        IReadOnlyDictionary<Type, Type> MainControlMappings { get; }
    }
}
