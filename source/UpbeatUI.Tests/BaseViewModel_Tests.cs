/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;
using System.Collections.Generic;
using NUnit.Framework;
using UpbeatUI.ViewModel;

namespace UpbeatUI.Tests
{
    public class BaseViewModel_Tests
    {
        [Test]
        public void Invalid_SetProperty_Throws()
        {
            var testModel = new TestModel();
            var testViewModel = new TestViewModel(testModel);
            var notifedPropertyName = "";
            testViewModel.PropertyChanged += (o, e) => notifedPropertyName = e.PropertyName;

            try
            {
                testViewModel.InvalidModelField = 1;
                Assert.Fail();
            } catch (Exception) { }
            Assert.AreEqual(notifedPropertyName, "");
            Assert.AreEqual(testModel._backingField, 0);

            try
            {
                testViewModel.InvalidModelMethod = 1;
                Assert.Fail();
            } catch (Exception) { }
            Assert.AreEqual(notifedPropertyName, "");
        }

        [Test]
        public void Sets_BackingValue_And_Notifies()
        {
            var testModel = new TestModel();
            var testViewModel = new TestViewModel(testModel);
            var notifedPropertyName = "";
            testViewModel.PropertyChanged += (o, e) => notifedPropertyName = e.PropertyName;

            testViewModel.ViewModelField = 1;
            Assert.AreEqual(notifedPropertyName, nameof(TestViewModel.ViewModelField));
            Assert.AreEqual(testViewModel.ViewModelField, 1);
            Assert.AreEqual(testViewModel._backingField, 1);

            testViewModel.ModelField = 1;
            Assert.AreEqual(notifedPropertyName, nameof(TestViewModel.ModelField));
            Assert.AreEqual(testViewModel.ModelField, 1);
            Assert.AreEqual(testModel._backingField, 1);

            testViewModel.ModelProperty = 1;
            Assert.AreEqual(notifedPropertyName, nameof(TestViewModel.ModelProperty));
            Assert.AreEqual(testViewModel.ModelProperty, 1);
            Assert.AreEqual(testModel.BackingProperty, 1);
        }

        [Test]
        public void Notifies()
        {
            var testModel = new TestModel();
            var testViewModel = new TestViewModel(testModel);

            var notifedPropertyName = "";
            testViewModel.PropertyChanged += (o, e) => notifedPropertyName = e.PropertyName;
            testViewModel.TriggerPropertyChangedEvent("test");
            Assert.AreEqual(notifedPropertyName, "test");

            var notifiedPropertyNames = new List<string>();
            testViewModel.PropertyChanged += (o, e) => notifiedPropertyNames.Add(e.PropertyName);
            testViewModel.TriggerPropertyChangedEvents("test1", "test2");
            Assert.Contains("test1", notifiedPropertyNames);
            Assert.Contains("test2", notifiedPropertyNames);
        }

        private class TestModel
        {
            public int _backingField;

            public int BackingProperty { get; set; }

            public int TestMethod() => 0;
        }

        private class TestViewModel : BaseViewModel
        {
            private TestModel _testModel;
            public int _backingField;

            public TestViewModel(TestModel testModel) =>
                _testModel = testModel;

            public int ModelField
            {
                get => _testModel._backingField;
                set => SetProperty(ref _testModel._backingField, value);
            }
            public int ModelProperty
            {
                get => _testModel.BackingProperty;
                set => SetProperty(_testModel, t => t.BackingProperty, value);
            }
            public int ViewModelField
            {
                get => _backingField;
                set => SetProperty(ref _backingField, value);
            }
            public int InvalidModelField
            {
                get => _testModel._backingField;
                set => SetProperty(_testModel, t => t._backingField, value);
            }
            public int InvalidModelMethod
            {
                get => _testModel.TestMethod();
                set => SetProperty(_testModel, t => t.TestMethod(), value);
            }

            public void TriggerPropertyChangedEvent(string name) =>
                RaisePropertyChanged(name);

            public void TriggerPropertyChangedEvents(params string[] names) =>
                RaisePropertyChanged(names);
        }
    }
}
