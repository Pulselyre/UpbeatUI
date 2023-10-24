/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
namespace UpbeatUI.View.Converters
{
    internal static class Extensions
    {
        public static double ParsePercent(this string percentString) =>
            percentString.EndsWith('%') ? double.Parse(percentString.TrimEnd('%')) / 100 : double.Parse(percentString);
    }
}
