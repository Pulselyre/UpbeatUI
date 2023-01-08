/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;
using System.Collections.Generic;

namespace ServiceProvidedUpbeatUISample
{
    // This is a simple list wrapper to demonstrate a scoped service shared between multiple ViewModels.
    public class SharedList
    {
        private List<string> strings = new List<string>();

        public SharedList() =>
            Strings = strings.AsReadOnly();

        public event EventHandler StringAdded;

        public IReadOnlyList<string> Strings { get; }

        public void AddString(string newString)
        {
            if (string.IsNullOrWhiteSpace(newString))
                return;
            strings.Add(newString);
            StringAdded?.Invoke(this, EventArgs.Empty);
        }
    }
}
