/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System;

namespace UpbeatUI.View.Converters
{
    internal static class Extensions
    {
        [Obsolete("'" + nameof(ParsePercent) + "' method is deprecated and will be removed in the next major release; consider using the '" + nameof(PercentPlaceContentControl) + "' class to position elements instead.")]

        public static double ParsePercent(this string percentString) =>
            percentString.EndsWith('%') ? double.Parse(percentString.TrimEnd('%')) / 100 : double.Parse(percentString);
    }
}
