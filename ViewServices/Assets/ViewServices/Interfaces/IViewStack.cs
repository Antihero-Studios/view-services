using System.Threading.Tasks;

namespace ViewServices.Interfaces
{
    public interface IViewStack
    {
        Task Push(IViewController controller);
        Task Pop();
        Task PopIf(IViewController controller);
        Task<TController> PopUntilExclusive<TController>() where TController : IViewController;
        Task PopAll();

        bool Contains(IViewController controller);
    }
}
