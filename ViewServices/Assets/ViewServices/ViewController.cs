using ViewServices.Interfaces;
using System.Threading.Tasks;
using UnityEngine;

namespace ViewServices
{
    public class ViewController : MonoBehaviour, IViewController
    {
        public virtual Task OnPoppedFromViewStack(IViewStack viewStack)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnPushedOnViewStack(IViewStack viewStack)
        {
            return Task.CompletedTask;
        }
    }

    public class ViewController<TModel> : ViewController, IViewController<TModel> where TModel : class
    {
        public TModel Model { get; set; }
    }
}
