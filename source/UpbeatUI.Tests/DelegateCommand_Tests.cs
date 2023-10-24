/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System;
using System.Threading.Tasks;
using NUnit.Framework;
using UpbeatUI.ViewModel;

namespace UpbeatUI.Tests
{
    public class DelegateCommand_Tests
    {
        [Test]
        public void NoParameter_Async_CanExecute_Is_Called()
        {
            var result = false;
            var canExecute = false;
            var delegateCommand = new DelegateCommand(() => { result = true; return Task.CompletedTask; }, canExecute: () => canExecute);
            Assert.IsFalse(delegateCommand.CanExecute());
            delegateCommand.Execute();
            Assert.IsFalse(result);

            canExecute = true;
            Assert.IsTrue(delegateCommand.CanExecute());
            delegateCommand.Execute();
            Assert.IsTrue(result);
        }

        [Test]
        public void NoParameter_Async_ExceptionCallback_Is_Called()
        {
            Exception result = null;
            var delegateCommand = new DelegateCommand(ThrowExceptionAsync, exceptionCallback: e => result = e);
            delegateCommand.Execute();
            Assert.IsInstanceOf<Exception>(result);
        }

        [Test]
        public void NoParameter_Async_Execute_Is_Called()
        {
            var result = false;
            var delegateCommand = new DelegateCommand(() => { result = true; return Task.CompletedTask; });
            delegateCommand.Execute();
            Assert.IsTrue(result);
        }

        [Test]
        public void NoParameter_Async_Limits_Executions()
        {
            var tcs = new TaskCompletionSource<bool>();
            var result = false;
            var delegateCommand = new DelegateCommand(async () => { await tcs.Task; result = true; });
            delegateCommand.Execute();
            Assert.IsFalse(delegateCommand.CanExecute());
            delegateCommand.Execute();
            Assert.IsFalse(result);
            tcs.SetResult(true);
            Assert.IsTrue(result);
        }

        [Test]
        public void NoParameter_CanExecute_Is_Called()
        {
            var result = false;
            var canExecute = false;
            var delegateCommand = new DelegateCommand(() => result = true, canExecute: () => canExecute);
            Assert.IsFalse(delegateCommand.CanExecute());
            delegateCommand.Execute();
            Assert.IsFalse(result);

            canExecute = true;
            Assert.IsTrue(delegateCommand.CanExecute());
            delegateCommand.Execute();
            Assert.IsTrue(result);
        }

        [Test]
        public void NoParameter_ExceptionCallback_Is_Called()
        {
            Exception result = null;
            var delegateCommand = new DelegateCommand(ThrowException, exceptionCallback: e => result = e);
            delegateCommand.Execute();
            Assert.IsInstanceOf<Exception>(result);
        }

        [Test]
        public void NoParameter_Execute_Is_Called()
        {
            var result = false;
            var delegateCommand = new DelegateCommand(() => result = true);
            delegateCommand.Execute();
            Assert.IsTrue(result);
        }

        [Test]
        public void NoParameter_Throws_Without_ErrorCallback()
        {
            var delegateCommand = new DelegateCommand(ThrowException);
            Assert.Throws<Exception>(() => delegateCommand.Execute());
        }

        [Test]
        public void WithParameter_Async_CanExecute_Is_Called()
        {
            var result = false;
            var canExecute = false;
            var delegateCommand = new DelegateCommand<int>(i => { result = true; return Task.CompletedTask; }, canExecute: i => canExecute);
            Assert.IsFalse(delegateCommand.CanExecute(0));
            delegateCommand.Execute(0);
            Assert.IsFalse(result);

            canExecute = true;
            Assert.IsTrue(delegateCommand.CanExecute(0));
            delegateCommand.Execute(0);
            Assert.IsTrue(result);
        }

        [Test]
        public void WithParameter_Async_ExceptionCallback_Is_Called()
        {
            Exception result = null;
            var delegateCommand = new DelegateCommand<int>(ThrowExceptionAsync, exceptionCallback: e => result = e);
            delegateCommand.Execute(0);
            Assert.IsInstanceOf<Exception>(result);
        }

        [Test]
        public void WithParameter_Async_Execute_Is_Called()
        {
            var result = false;
            var delegateCommand = new DelegateCommand<int>(i => { result = true; return Task.CompletedTask; });
            delegateCommand.Execute(0);
            Assert.IsTrue(result);
        }

        [Test]
        public void WithParameter_Async_Limits_Executions()
        {
            var tcs = new TaskCompletionSource<bool>();
            var result = false;
            var delegateCommand = new DelegateCommand<int>(async i => { await tcs.Task; result = true; });
            delegateCommand.Execute(0);
            Assert.IsFalse(delegateCommand.CanExecute(0));
            delegateCommand.Execute(0);
            Assert.IsFalse(result);
            tcs.SetResult(true);
            Assert.IsTrue(result);
        }

        [Test]
        public void WithParameter_CanExecute_Is_Called()
        {
            var result = false;
            var canExecute = false;
            var delegateCommand = new DelegateCommand<int>(i => result = true, canExecute: i => canExecute);
            Assert.IsFalse(delegateCommand.CanExecute(0));
            delegateCommand.Execute(0);
            Assert.IsFalse(result);

            canExecute = true;
            Assert.IsTrue(delegateCommand.CanExecute(0));
            delegateCommand.Execute(0);
            Assert.IsTrue(result);
        }

        [Test]
        public void WithParameter_ExceptionCallback_Is_Called()
        {
            Exception result = null;
            var delegateCommand = new DelegateCommand<int>(ThrowException, exceptionCallback: e => result = e);
            delegateCommand.Execute(0);
            Assert.IsInstanceOf<Exception>(result);
        }

        [Test]
        public void WithParameter_Execute_Is_Called()
        {
            var result = false;
            var delegateCommand = new DelegateCommand<int>(i => result = true);
            delegateCommand.Execute(0);
            Assert.IsTrue(result);
        }

        [Test]
        public void WithParameter_Throws_Without_ErrorCallback()
        {
            var delegateCommand = new DelegateCommand<int>(ThrowException);
            Assert.Throws<Exception>(() => delegateCommand.Execute(0));
        }

        private void ThrowException() =>
            throw new Exception();

        private void ThrowException(int i) =>
            throw new Exception();

        private Task ThrowExceptionAsync() =>
            throw new Exception();

        private Task ThrowExceptionAsync(int i) =>
            throw new Exception();
    }
}
