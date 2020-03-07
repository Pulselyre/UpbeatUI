/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;
using System.Threading.Tasks;

namespace UpbeatUI.Context
{
    /// <summary>
    /// Provides methods for a View Model to interact with the ContextStack that it is a part of. IContextServices are created by the ContextStack unique to each child IContext, so do not share them.
    /// </summary>
    public interface IContextService
    {
        /// <summary>
        /// Gets whether or not the View Model is the top item in the ContextStack, and thus active for the user.
        /// </summary>
        bool IsActiveContext { get; }

        /// <summary>
        /// Signals to the parent ContextStack that this IContext would like to close and be removed.
        /// </summary>
        void Close();

        /// <summary>
        /// Gets the current string contents of the clipboard. This is a convenience method for assemblies that do not want to reference System.Windows.
        /// </summary>
        /// <returns>The current contents of the clipboard.</returns>
        string GetClipboard();

        /// <summary>
        /// Adds a new IContext (View Model) to the ContextStack.
        /// </summary>
        /// <param name="creator">A delegate that accepts an IContextService as a parameter and returns a new IContext (View Model).</param>
        void OpenContext(ContextCreator contextCreator);

        /// <summary>
        /// Adds a new IContext (View Model) to the ContextStack and executes a callback after that IContext closes.
        /// </summary>
        /// <param name="creator">A delegate that accepts an IContextService as a parameter and returns a new IContext (View Model).</param>
        /// <param name="closeCallback">A delegate for the ContextStack to execute after the IContext closes.</param>
        void OpenContext(ContextCreator contextCreator, Action closedCallback);

        /// <summary>
        /// Adds a new IContext (View Model) to the ContextStack and returns a Task that ends after the IContext closes.
        /// </summary>
        /// <param name="creator">A delegate that accepts an IContextService as a parameter and returns a new IContext (View Model).</param>
        /// <returns>A Task that ends after the IContext closes.</returns>
        Task OpenContextAsync(ContextCreator contextCreator);

        /// <summary>
        /// Sets the string contents of the clipboard. This is a convenience method for assemblies that do not want to reference System.Windows.
        /// </summary>
        /// <param name="text">What to set the clipboard to.</param>
        void SetClipboard(string text);
    }
}
