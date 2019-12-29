using ViewServices.Exceptions;
using ViewServices.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace ViewServices
{
    public class ViewStack : MonoBehaviour, IViewStack
    {
        private readonly Stack<IViewController> _stack = new Stack<IViewController>();

        #region Contract
        public async Task Push(IViewController controller)
        {
            if(Contains(controller))
                throw new ViewAlreadyStackedException();

            _stack.Push(controller);

            await OnPush(controller);
            await controller.OnPushedOnViewStack(this);
        }

        public async Task Pop()
        {
            if (_stack.Count == 0)
                return;

            var controller = _stack.Pop();
            await OnPop(controller);
            await controller.OnPoppedFromViewStack(this);
        }

        public Task PopIf(IViewController controller)
        {
            if(_stack.Count > 0 && _stack.Peek() == controller)
            {
                return Pop();
            }

            return Task.CompletedTask;
        }

        public async Task<TController> PopUntilExclusive<TController>() where TController : IViewController
        {
            while(_stack.Count > 0)
            {
                if(_stack.Peek() is TController)
                    break;

                await Pop();
            }

            // We didn't find the controller in the stack...
            if(_stack.Count == 0 || !(_stack.Peek() is TController))
            {
                throw new ViewNotOnViewStackException();
            }
            else
            {
                return (TController)_stack.Peek();
            }
        }

        public async Task PopAll()
        {
            while(_stack.Count > 0)
            {
                await Pop();
            }
        }

        public bool Contains(IViewController controller)
        {
            return _stack.Contains(controller);
        }
        #endregion

        #region Protected Implementation
        protected virtual Task OnPush(IViewController controller)
        {
            // Set parent and sibling index...
            if(controller is ViewController viewController)
            {
                viewController.transform.SetParent(transform, false);
                viewController.transform.SetAsLastSibling();
            }

            return Task.CompletedTask;
        }

        protected virtual Task OnPop(IViewController controller)
        {
            // Set parent null...
            if(controller is ViewController viewController)
            {
                viewController.transform.SetParent(null);
            }

            return Task.CompletedTask;
        }
        #endregion
    }
}
