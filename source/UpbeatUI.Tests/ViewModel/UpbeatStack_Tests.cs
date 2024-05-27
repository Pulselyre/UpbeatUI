/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System;
using System.Threading.Tasks;
using NUnit.Framework;
using UpbeatUI.ViewModel;

namespace UpbeatUI.Tests.ViewModel.UpbeatStack_Tests
{
    [TestFixture]
    public class BaseFixture
    {
        protected UpbeatStack UpbeatStack { get; set; }
        protected int CloseAttemptCount { get; set; }
        protected int ClosedCount { get; set; }
        protected int EmptiedCount { get; set; }

        [SetUp]
        public void SetUp()
        {
            UpbeatStack = new UpbeatStack();
            CloseAttemptCount = 0;
            ClosedCount = 0;
            EmptiedCount = 0;
            UpbeatStack.ViewModelsEmptied += (_, _) => EmptiedCount++;
            UpbeatStack.MapViewModel<TestViewModel.Parameters, TestViewModel>(
                (service, parameters) => new TestViewModel(service, parameters));
        }

        [TearDown]
        public void TearDown() =>
            UpbeatStack.Dispose();

        protected TestViewModel.Parameters BuildParameters() =>
            new()
            {
                CloseCallback = () =>
                    {
                        CloseAttemptCount++;
                        return Task.FromResult(true);
                    }
            };

        protected TestViewModel.Parameters BuildParameters(TaskCompletionSource tcs) =>
            new()
            {
                CloseCallback = async () =>
                    {
                        _ = tcs ?? throw new ArgumentNullException(nameof(tcs));
                        CloseAttemptCount++;
                        await tcs.Task.ConfigureAwait(true);
                        return true;
                    }
            };

        protected TestViewModel.Parameters BuildParameters(TaskCompletionSource<bool> tcs = null) =>
            new()
            {
                CloseCallback = async () =>
                    {
                        CloseAttemptCount++;
                        return await tcs.Task.ConfigureAwait(true);
                    }
            };

        protected class TestViewModel
        {
            public TestViewModel(IUpbeatService upbeatService, Parameters parameters)
            {
                if (parameters?.CloseCallback != null)
                {
                    upbeatService?.RegisterCloseCallback(parameters.CloseCallback);
                }
            }

            public class Parameters
            {

                public Func<Task<bool>> CloseCallback { get; set; }
            }
        }
    }

    public class RemoveTopViewModelCommand_Tests : BaseFixture
    {
        [Test]
        public void Allows_Only_One_Execution()
        {
            var tcs = new TaskCompletionSource();
            UpbeatStack.OpenViewModel(BuildParameters(tcs), () => ClosedCount++);
            Assert.AreEqual(1, UpbeatStack.Count);
            UpbeatStack.RemoveTopViewModelCommand.Execute(null);
            Assert.AreEqual(1, UpbeatStack.Count);
            Assert.AreEqual(1, CloseAttemptCount);
            Assert.AreEqual(0, ClosedCount);
            Assert.AreEqual(0, EmptiedCount);
            UpbeatStack.RemoveTopViewModelCommand.Execute(null);
            Assert.AreEqual(1, UpbeatStack.Count);
            Assert.AreEqual(1, CloseAttemptCount);
            Assert.AreEqual(0, ClosedCount);
            Assert.AreEqual(0, EmptiedCount);
            tcs.SetResult();
            Assert.AreEqual(0, UpbeatStack.Count);
            Assert.AreEqual(1, CloseAttemptCount);
            Assert.AreEqual(1, ClosedCount);
            Assert.AreEqual(1, EmptiedCount);
        }

        public class CanExecute_Tests : BaseFixture
        {

            [Test]
            public void Returns_True_When_Not_Executiong()
            {
                UpbeatStack.OpenViewModel(new TestViewModel.Parameters(), () => ClosedCount++);
                Assert.IsTrue(UpbeatStack.RemoveTopViewModelCommand.CanExecute(null));
            }

            [Test]
            public void Returns_False_When_No_ViewModels() =>
                Assert.IsFalse(UpbeatStack.RemoveTopViewModelCommand.CanExecute(null));

            [Test]
            public void Returns_False_When_Executiong()
            {
                var tcs = new TaskCompletionSource();
                UpbeatStack.OpenViewModel(BuildParameters(tcs), () => ClosedCount++);
                UpbeatStack.OpenViewModel(BuildParameters(tcs), () => ClosedCount++);
                UpbeatStack.RemoveTopViewModelCommand.Execute(null);
                Assert.IsFalse(UpbeatStack.RemoveTopViewModelCommand.CanExecute(null));
                tcs.SetResult();
                Assert.IsTrue(UpbeatStack.RemoveTopViewModelCommand.CanExecute(null));
            }
        }
    }

    public class TryCloseAllViewModelsAsync_Tests : BaseFixture
    {
        [Test]
        public async Task Removes_All_ViewModels_When_Allowed()
        {
            var tcs = new TaskCompletionSource();
            UpbeatStack.OpenViewModel(BuildParameters(tcs), () => ClosedCount++);
            UpbeatStack.OpenViewModel(BuildParameters(tcs), () => ClosedCount++);
            Assert.AreEqual(2, UpbeatStack.Count);
            tcs.SetResult();
            Assert.IsTrue(await UpbeatStack.TryCloseAllViewModelsAsync().ConfigureAwait(true));
            Assert.AreEqual(0, UpbeatStack.Count);
            Assert.AreEqual(2, CloseAttemptCount);
            Assert.AreEqual(2, ClosedCount);
            Assert.AreEqual(1, EmptiedCount);
        }

        [Test]
        public async Task Does_Not_Remove_All_ViewModels_When_Not_Allowed()
        {
            var tcs1 = new TaskCompletionSource<bool>();
            var tcs2 = new TaskCompletionSource<bool>();
            UpbeatStack.OpenViewModel(BuildParameters(tcs1), () => ClosedCount++);
            UpbeatStack.OpenViewModel(BuildParameters(tcs2), () => ClosedCount++);
            Assert.AreEqual(2, UpbeatStack.Count);
            tcs1.SetResult(false);
            tcs2.SetResult(true);
            Assert.IsFalse(await UpbeatStack.TryCloseAllViewModelsAsync().ConfigureAwait(true));
            Assert.AreEqual(1, UpbeatStack.Count);
            Assert.AreEqual(2, CloseAttemptCount);
            Assert.AreEqual(1, ClosedCount);
            Assert.AreEqual(0, EmptiedCount);
        }

        [Test]
        public async Task Waits_When_Already_Closing_ViewModel()
        {
            var tcs1 = new TaskCompletionSource();
            var tcs2 = new TaskCompletionSource();
            UpbeatStack.OpenViewModel(BuildParameters(), () => ClosedCount++);
            UpbeatStack.OpenViewModel(BuildParameters(tcs1), () => ClosedCount++);
            Assert.AreEqual(2, UpbeatStack.Count);
            UpbeatStack.RemoveTopViewModelCommand.Execute(null);
            UpbeatStack.OpenViewModel(BuildParameters(tcs2), () => ClosedCount++);
            Assert.AreEqual(3, UpbeatStack.Count);
            Assert.AreEqual(1, CloseAttemptCount);
            Assert.AreEqual(0, ClosedCount);
            var closeAllTask = UpbeatStack.TryCloseAllViewModelsAsync();
            Assert.AreEqual(3, UpbeatStack.Count);
            Assert.AreEqual(1, CloseAttemptCount);
            Assert.AreEqual(0, ClosedCount);
            tcs2.SetResult();
            UpbeatStack.RemoveTopViewModelCommand.Execute(null);
            Assert.AreEqual(2, UpbeatStack.Count);
            Assert.AreEqual(2, CloseAttemptCount);
            Assert.AreEqual(1, ClosedCount);
            tcs1.SetResult();
            Assert.IsTrue(await closeAllTask.ConfigureAwait(true));
            Assert.AreEqual(0, UpbeatStack.Count);
            Assert.AreEqual(3, CloseAttemptCount);
            Assert.AreEqual(3, ClosedCount);
            Assert.AreEqual(1, EmptiedCount);
        }
    }
}
