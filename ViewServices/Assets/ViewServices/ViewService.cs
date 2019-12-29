using ViewServices.Exceptions;
using ViewServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace ViewServices
{
    public class ViewService : MonoBehaviour, IViewService
    {
        private readonly Dictionary<Type, Func<Task<IViewController>>> _factories = new Dictionary<Type, Func<Task<IViewController>>>();
        private readonly Dictionary<Type, Func<object, Task<IViewController>>> _factoriesWithModels = new Dictionary<Type, Func<object, Task<IViewController>>>();
        private readonly Dictionary<string, IViewStack> _viewStacks = new Dictionary<string, IViewStack>();

        public void AddViewStack(string viewStackId, IViewStack viewStack)
        {
            _viewStacks[viewStackId] = viewStack;
        }

        public IViewStack FindViewStackFor(IViewController controller)
        {
            foreach(var pair in _viewStacks)
            {
                if(pair.Value.Contains(controller))
                    return pair.Value;
            }

            return null;
        }

        public IViewStack GetViewStack(string viewStackId)
        {
            if(!_viewStacks.ContainsKey(viewStackId))
                _viewStacks.Add(viewStackId, CreateViewStack(viewStackId));

            return _viewStacks[viewStackId];
        }

        public Task Pop(string viewStackId)
        {
            var viewStack = GetViewStack(viewStackId);
            return viewStack.Pop();
        }

        public Task PopAll(string viewStackId)
        {
            var viewStack = GetViewStack(viewStackId);
            return viewStack.PopAll();
        }

        public Task PopIf(string viewStackId, IViewController controller)
        {
            var viewStack = GetViewStack(viewStackId);
            return viewStack.PopIf(controller);
        }

        public Task<TController> PopUntilExclusive<TController>(string viewStackId) where TController : IViewController
        {
            var viewStack = GetViewStack(viewStackId);
            return viewStack.PopUntilExclusive<TController>();
        }

        public Task Push(string viewStackId, IViewController controller)
        {
            var viewStack = GetViewStack(viewStackId);
            return viewStack.Push(controller);
        }

        private ViewStack CreateViewStack(string viewStackId)
        {
            var instance = new GameObject($"ViewStack[{viewStackId}]", typeof(RectTransform), typeof(ViewStack));

            // Setup rect transform...
            var rectTransform = instance.GetComponent<RectTransform>();
            rectTransform.SetParent(transform, false);
            rectTransform.anchoredPosition = Vector3.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.localScale = Vector3.one;
            rectTransform.sizeDelta = Vector2.one;

            return instance.GetComponent<ViewStack>();
        }

        public async Task<TController> Create<TController>() where TController : IViewController
        {
            if (!_factories.ContainsKey(typeof(TController)))
                throw new FactoryNotRegisteredException();

            return (TController)await _factories[typeof(TController)].Invoke();
        }

        public async Task<TController> Create<TController, TModel>(TModel model) where TController : IViewController<TModel> where TModel : class
        {
            TController controller = default;

            if (_factoriesWithModels.ContainsKey(typeof(TController)))
            {
                controller = (TController)await _factoriesWithModels[typeof(TController)].Invoke(model);
            }
            else
            {
                // Fallback to default registe..
                controller = await Create<TController>();
            }

            // Set model...
            if (controller != null)
            {
                controller.Model = model;
            }

            return controller;
        }

        public void RegisterFactory<TController>(Func<Task<IViewController>> factory) where TController : IViewController
        {
            if (_factories.ContainsKey(typeof(TController)))
                throw new FactoryAlreadyRegisteredException();

            _factories[typeof(TController)] = () => factory();
        }

        public void RegisterFactory<TController, TModel>(Func<TModel, Task<IViewController>> factory) where TController : IViewController<TModel> where TModel : class
        {
            if (_factories.ContainsKey(typeof(TController)))
                throw new FactoryAlreadyRegisteredException();

            _factoriesWithModels[typeof(TController)] = (model) => factory((TModel)model);
        }
    }
}
