# ViewServices

## ViewController
Every instance of a UI Element is considered a ViewController where the prefab is the View.

```csharp
public class MyController : ViewController { .. }
```

## ViewService

### Registration
A controller is registered via a factory.

```csharp
viewService.Register<MyController>(() => {
    // Your asset loading, initial creation, pooling etc...
    var prefab = await Addressables.LoadAssetAsync<GameObject>("...");
    var instance = GameObject.Instantiate(prefab);

    var controller = instance.AddComponent<MyController>();

    return controller;
});

// for view controllers with models. Also falls back to Register<MyController> if no factory with model found...
viewService.Register<MyController, MyControllerModel>((model) => {
    // controller.Model will be set after creation, but you can use properties to determine whether to load different prefabs etc...
});
```

### Creation
You can then later call `Create` to simple create your view.

```csharp
var controller = await viewService.Create<MyController>();
```

## Stacking
Each controller can be stacked and popped onto the view stack. This can help inform layering in your application.

```csharp
var stack = viewService.GetViewStack("default"); // Named stack can be anything your application needs.
await stack.Push(controller); // Adds to stack and invokes OnPushedOnViewStack
await stack.Pop(); // Pops the top controller and invokes OnPoppedFromViewStack
await stack.PopIf(controller); // Only pops if the top is the controller
await stack.PopUntilExclusive<MyController>(); // Pops and unwinds until it reaches the specified controller type. This is exclusive, so does not pop the specified type.
await stack.PopAll(); // Pops all the controllers and invokes their OnPoppedFromViewStack sequentially.
```
