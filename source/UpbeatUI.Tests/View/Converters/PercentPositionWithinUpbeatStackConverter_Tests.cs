/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using NUnit.Framework;
using UpbeatUI.View.Converters;
using UpbeatUI.ViewModel;

namespace UpbeatUI.Tests.View.Converters.PercentPositionWithinUpbeatStackConverter_Tests
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class Convert_Tests
    {
        private Button Button { get; set; }
        private UpbeatStack UpbeatStack { get; set; }
        private Grid Grid { get; set; }
        private PercentPositionWithinUpbeatStackConverter Converter { get; set; }

        [SetUp]
        public void SetUp()
        {
            Button = new();
            UpbeatStack = new();
            Grid = new() { DataContext = UpbeatStack };
            Converter = new();
        }

        [Test]
        public void Returns_Working_Getter_When_UpbeatStack_Is_Ancestor()
        {
            _ = Grid.Children.Add(Button);
            Func<Point> pointGetter = null;
            Assert.DoesNotThrow(
                () => pointGetter = (Func<Point>)Converter.Convert(
                    Button,
                    typeof(object),
                    null,
                    CultureInfo.CurrentCulture));
            Point point = default;
            Assert.DoesNotThrow(() => point = pointGetter.Invoke());
            Assert.AreEqual(double.NaN, point.X);
            Assert.AreEqual(double.NaN, point.Y);
        }

        [Test]
        public void Returns_Working_Getter_When_UpbeatStack_Was_Ancestor()
        {
            _ = Grid.Children.Add(Button);
            Func<Point> pointGetter = null;
            Assert.DoesNotThrow(
                () => pointGetter = (Func<Point>)Converter.Convert(
                    Button,
                    typeof(object),
                    null,
                    CultureInfo.CurrentCulture));
            Grid.Children.Remove(Button);
            Point point = default;
            Assert.DoesNotThrow(() => point = pointGetter.Invoke());
            Assert.AreEqual(double.NaN, point.X);
            Assert.AreEqual(double.NaN, point.Y);
        }

        [Test]
        public void Throws_When_UpbeatStack_Is_Not_Ancestor() =>
            Assert.Throws<InvalidOperationException>(
                () => _ = (Func<Point>)Converter.Convert(
                    Button,
                    typeof(object),
                    null,
                    CultureInfo.CurrentCulture));
    }
}
