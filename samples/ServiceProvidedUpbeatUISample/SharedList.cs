/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ServiceProvidedUpbeatUISample;

// This is a simple list wrapper to demonstrate a scoped service shared between multiple ViewModels.
public class SharedList
{
    private readonly Collection<string> _strings = new();

    public SharedList() =>
        Strings = _strings.AsReadOnly();

    public event EventHandler StringAdded;

    public IReadOnlyList<string> Strings { get; }

    public void AddString(string newString)
    {
        if (string.IsNullOrWhiteSpace(newString))
        {
            return;
        }

        _strings.Add(newString);
        StringAdded?.Invoke(this, EventArgs.Empty);
    }
}
