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
    public class RemoveTopViewModelCommand_Tests
    {
        private UpbeatStack _upbeatStack;
        private TaskCompletionSource _tcs;
        private int _closeAttemptCount;
        private int _closedCount;
        private int _emptiedCount;

        [SetUp]
        public void Setuip()
        {
            _upbeatStack = new UpbeatStack();
            _tcs = new TaskCompletionSource();
            _closeAttemptCount = 0;
            _closedCount = 0;
            _emptiedCount = 0;
            _upbeatStack.ViewModelsEmptied += (_, _) => _emptiedCount++;
            _upbeatStack.MapViewModel<TestViewModel.Parameters, TestViewModel>(
                (service, parameters) => new TestViewModel(
                    service,
                    async () =>
                    {
                        _closeAttemptCount++;
                        await _tcs.Task.ConfigureAwait(true);
                        return true;
                    }));
        }

        [TearDown]
        public void TearDown() =>
            _upbeatStack.Dispose();

        [Test]
        public void Allows_Only_One_Execution()
        {
            _upbeatStack.OpenViewModel(new TestViewModel.Parameters(), () => _closedCount++);
            Assert.AreEqual(1, _upbeatStack.Count);
            _upbeatStack.RemoveTopViewModelCommand.Execute(null);
            Assert.AreEqual(1, _upbeatStack.Count);
            Assert.AreEqual(1, _closeAttemptCount);
            Assert.AreEqual(0, _closedCount);
            Assert.AreEqual(0, _emptiedCount);
            _upbeatStack.RemoveTopViewModelCommand.Execute(null);
            Assert.AreEqual(1, _upbeatStack.Count);
            Assert.AreEqual(1, _closeAttemptCount);
            Assert.AreEqual(0, _closedCount);
            Assert.AreEqual(0, _emptiedCount);
            _tcs.SetResult();
            Assert.AreEqual(0, _upbeatStack.Count);
            Assert.AreEqual(1, _closeAttemptCount);
            Assert.AreEqual(1, _closedCount);
            Assert.AreEqual(1, _emptiedCount);
        }

        [Test]
        public void CanExecute_Returns_True_When_Not_Executiong()
        {
            _upbeatStack.OpenViewModel(new TestViewModel.Parameters(), () => _closedCount++);
            Assert.IsTrue(_upbeatStack.RemoveTopViewModelCommand.CanExecute(null));
        }

        [Test]
        public void CanExecute_Returns_False_When_No_ViewModels() =>
            Assert.IsFalse(_upbeatStack.RemoveTopViewModelCommand.CanExecute(null));

        [Test]
        public void CanExecute_Returns_False_When_Executiong()
        {
            _upbeatStack.OpenViewModel(new TestViewModel.Parameters(), () => _closedCount++);
            _upbeatStack.OpenViewModel(new TestViewModel.Parameters(), () => _closedCount++);
            _upbeatStack.RemoveTopViewModelCommand.Execute(null);
            Assert.IsFalse(_upbeatStack.RemoveTopViewModelCommand.CanExecute(null));
            _tcs.SetResult();
            Assert.IsTrue(_upbeatStack.RemoveTopViewModelCommand.CanExecute(null));
        }

        private class TestViewModel
        {
            public TestViewModel(IUpbeatService upbeatService, Func<Task<bool>> closeCallback = null)
            {
                if (closeCallback != null)
                {
                    upbeatService.RegisterCloseCallback(closeCallback);
                }
            }

            public class Parameters { }
        }
    }
}
