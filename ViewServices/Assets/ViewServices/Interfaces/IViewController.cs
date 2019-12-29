using System.Threading.Tasks;

namespace ViewServices.Interfaces
{
    public interface IViewController
    {
        Task OnPushedOnViewStack(IViewStack viewStack);
        Task OnPoppedFromViewStack(IViewStack viewStack);
    }

    public interface IViewController<TModel> : IViewController where TModel : class
    {
        TModel Model { get; set; }
    }
}
