using System;
using System.Threading.Tasks;

namespace ViewServices.Interfaces
{
    public interface IViewService
    {
        void AddViewStack(string viewStackId, IViewStack viewStack);
        IViewStack GetViewStack(string viewStackId);
        IViewStack FindViewStackFor(IViewController controller);

        Task Push(string viewStackId, IViewController controller);
        Task Pop(string viewStackId);
        Task PopIf(string viewStackId, IViewController controller);
        Task<TController> PopUntilExclusive<TController>(string viewStackId) where TController : IViewController;
        Task PopAll(string viewStackId);

        Task<TController> Create<TController>() where TController : IViewController;
        Task<TController> Create<TController, TModel>(TModel model) where TController : IViewController<TModel> where TModel : class;

        void RegisterFactory<TController>(Func<Task<IViewController>> factory) where TController : IViewController;
        void RegisterFactory<TController, TModel>(Func<TModel, Task<IViewController>> factory) where TController : IViewController<TModel> where TModel : class;
    }
}
