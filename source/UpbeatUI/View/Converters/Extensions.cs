/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System;
using System.Globalization;

namespace UpbeatUI.View.Converters
{
    internal static class Extensions
    {
        [Obsolete("'" + nameof(ParsePercent) + "' method is deprecated and will be removed in the next major release; consider using the '" + nameof(PercentPlaceContentControl) + "' class to position elements instead.")]

        public static double ParsePercent(this string percentString) =>
            percentString is null
                ? throw new ArgumentNullException(nameof(percentString))
                : percentString.EndsWith('%')
                    ? double.Parse(percentString.TrimEnd('%'), CultureInfo.InvariantCulture) / 100
                    : double.Parse(percentString, CultureInfo.InvariantCulture);
    }
}
