/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using NUnit.Framework;
using UpbeatUI.ViewModel.ListSynchronize;

namespace UpbeatUI.Tests.ViewModel.ListSynchronize.ExtensionMethods_Tests
{
    [TestFixture]
    public class Values_Only
    {
        [Test]
        public void Synchronizes_Empty_Array_With_Values()
        {
            var observableCollection = new ObservableCollection<int>();
            var newValues = Enumerable.Range(0, 5).ToList();
            ExecuteAndTestSync(observableCollection, newValues);
        }

        [Test]
        public void Synchronizes_Full_Array_With_New_Values()
        {
            var observableCollection = new ObservableCollection<int>() { 11, 12, 13 };
            var newValues = Enumerable.Range(0, 5).ToList();
            ExecuteAndTestSync(observableCollection, newValues);
        }

        [Test]
        public void Synchronizes_Full_Array_With_No_Values()
        {
            var observableCollection = new ObservableCollection<int>() { 11, 12, 13 };
            var newValues = new List<int>();
            ExecuteAndTestSync(observableCollection, newValues);
        }

        [Test]
        public void Synchronizes_Full_Array_With_No_Values_And_Cleaner()
        {
            var observableCollection = new ObservableCollection<int>() { 11, 12, 13 };
            var newValues = new List<int>();
            var cleaned = 0;
            ExecuteAndTestSync(observableCollection, newValues, i => cleaned++);
            Assert.AreEqual(3, cleaned);
        }

        private static void ExecuteAndTestSync(
            ObservableCollection<int> observableCollection,
            IList<int> newValues,
            Action<int> cleaner = null
        )
        {
            var originalSize = observableCollection.Count;
            var changedCount = 0;
            observableCollection.CollectionChanged += (o, e) => changedCount++;
            observableCollection.Synchronize(newValues, cleaner);
            Assert.AreEqual(newValues.Count, observableCollection.Count);
            Assert.AreEqual(Math.Max(originalSize, newValues.Count), changedCount);
            for (var i = 0; i < newValues.Count; i++)
            {
                Assert.AreEqual(newValues[i], observableCollection[i]);
            }
        }
    }

    [TestFixture]
    public class With_Synchronizer
    {
        [Test]
        public void Synchronizes_Values_With_Syncrhonizer()
        {
            var observableCollection = new ObservableCollection<TestObjectWithDefaultConstructor>();
            var newValues = Enumerable.Range(0, 5).ToList();
            ExecuteAndTestSync(observableCollection, newValues);
        }

        [Test]
        public void Synchronizes_Full_Array_With_New_Values()
        {
            var observableCollection = new ObservableCollection<TestObjectWithDefaultConstructor>()
                {
                    new() { Value = "11" },
                    new() { Value = "12" },
                    new() { Value = "13" }
                };
            var newValues = Enumerable.Range(0, 5).ToList();
            ExecuteAndTestSync(observableCollection, newValues);
        }

        [Test]
        public void Synchronizes_Full_Array_With_No_Values()
        {
            var observableCollection = new ObservableCollection<TestObjectWithDefaultConstructor>()
                {
                    new() { Value = "11" },
                    new() { Value = "12" },
                    new() { Value = "13" }
                };
            var newValues = new List<int>();
            ExecuteAndTestSync(observableCollection, newValues);
        }

        [Test]
        public void Synchronizes_Full_Array_With_No_Values_And_Cleaner()
        {
            var observableCollection = new ObservableCollection<TestObjectWithDefaultConstructor>()
                {
                    new() { Value = "11" },
                    new() { Value = "12" },
                    new() { Value = "13" }
                };
            var newValues = new List<int>();
            var cleaned = 0;
            ExecuteAndTestSync(observableCollection, newValues, to => cleaned++);
            Assert.AreEqual(3, cleaned);
        }

        private static void ExecuteAndTestSync(
            ObservableCollection<TestObjectWithDefaultConstructor> observableCollection,
            IList<int> newValues,
            Action<TestObjectWithDefaultConstructor> cleaner = null
        )
        {
            var originalSize = observableCollection.Count;
            var changedCount = 0;
            observableCollection.CollectionChanged += (o, e) => changedCount++;
            observableCollection.Synchronize(
                newValues,
                (i, to) => to.Value = i.ToString(CultureInfo.InvariantCulture),
                cleaner);
            Assert.AreEqual(newValues.Count, observableCollection.Count);
            Assert.AreEqual(Math.Abs(originalSize - newValues.Count), changedCount);
            for (var i = 0; i < newValues.Count; i++)
            {
                Assert.AreEqual(newValues[i].ToString(CultureInfo.InvariantCulture), observableCollection[i].Value);
            }
        }
    }

    [TestFixture]
    public class With_Synchronizer_And_Blank_Creator
    {
        [Test]
        public void Synchronizes_Values_With_Syncrhonizer()
        {
            var observableCollection = new ObservableCollection<TestObjectWithoutDefaultConstructor>();
            var newValues = Enumerable.Range(0, 5).ToList();
            ExecuteAndTestSync(observableCollection, newValues);
        }

        [Test]
        public void Synchronizes_Full_Array_With_New_Values()
        {
            var observableCollection = new ObservableCollection<TestObjectWithoutDefaultConstructor>()
                {
                    new("11"),
                    new("12"),
                    new("13")
                };
            var newValues = Enumerable.Range(0, 5).ToList();
            ExecuteAndTestSync(observableCollection, newValues);
        }

        [Test]
        public void Synchronizes_Full_Array_With_No_Values()
        {
            var observableCollection = new ObservableCollection<TestObjectWithoutDefaultConstructor>()
                {
                    new("11"),
                    new("12"),
                    new("13")
                };
            var newValues = new List<int>();
            ExecuteAndTestSync(observableCollection, newValues);
        }

        [Test]
        public void Synchronizes_Full_Array_With_No_Values_With_Cleaner()
        {
            var observableCollection = new ObservableCollection<TestObjectWithoutDefaultConstructor>()
                {
                    new("11"),
                    new("12"),
                    new("13")
                };
            var newValues = new List<int>();
            var cleaned = 0;
            ExecuteAndTestSync(observableCollection, newValues, to => cleaned++);
            Assert.AreEqual(3, cleaned);
        }

        private static void ExecuteAndTestSync(
            ObservableCollection<TestObjectWithoutDefaultConstructor> observableCollection,
            IList<int> newValues,
            Action<TestObjectWithoutDefaultConstructor> cleaner = null
        )
        {
            var originalSize = observableCollection.Count;
            var changedCount = 0;
            observableCollection.CollectionChanged += (o, e) => changedCount++;
            observableCollection.Synchronize(
                newValues,
                () => new TestObjectWithoutDefaultConstructor(null),
                (i, to) => to.Value = i.ToString(CultureInfo.InvariantCulture),
                cleaner);
            Assert.AreEqual(newValues.Count, observableCollection.Count);
            Assert.AreEqual(Math.Abs(originalSize - newValues.Count), changedCount);
            for (var i = 0; i < newValues.Count; i++)
            {
                Assert.AreEqual(newValues[i].ToString(CultureInfo.InvariantCulture), observableCollection[i].Value);
            }
        }
    }

    public class TestObjectWithDefaultConstructor
    {
        public string Value { get; set; }
    }

    public class TestObjectWithoutDefaultConstructor
    {
        public TestObjectWithoutDefaultConstructor(string value) => Value = value;

        public string Value { get; set; }
    }
}
