# StartUp Framework

A flexible and extensible framework for organizing application initialization steps in Unity projects.

## Quick Start

1. Create a new script that inherits from `StartUpController`:

```csharp
using System;
using System.Collections.Generic;
using StartUp.Runtime;

public class GameStartUpController : StartUpController
{
    protected override IReadOnlyList<Type> StepTypes => new List<Type>
    {
        typeof(DIStep),
        typeof(AdsStep),
        typeof(YourCustomStep)
    };
}
```

2. Create your custom initialization steps by extending `BaseStep`:

```csharp
using System.Threading.Tasks;
using StartUp.Runtime;
using UnityEngine;

public class YourCustomStep : BaseStep
{
    protected override Task ExecuteInternal()
    {
        Debug.Log("Initializing custom systems...");
        
        // Your initialization code here
        
        return Task.CompletedTask;
    }
}
```

3. Add your `GameStartUpController` to a GameObject in your initial scene.

## Features

- **Sequential Initialization**: Execute initialization steps in a defined order
- **Async Support**: Use async/await for operations that need to wait for completion
- **Event System**: Get notified when initialization completes
- **Extensible**: Create custom steps for your specific needs
- **Code Stripping Prevention**: Built-in protection against IL2CPP stripping

## Creating Custom Steps

1. Create a class that inherits from `BaseStep`
2. Override the `ExecuteInternal` method to implement your initialization logic
3. Add your step to the `StepTypes` list in your custom `StartUpController`

```csharp
public class NetworkInitStep : BaseStep
{
    protected override async Task ExecuteInternal()
    {
        // Initialize network
        var result = await NetworkManager.Initialize();
        
        if (!result.Success)
            throw new Exception($"Network initialization failed: {result.ErrorMessage}");
    }
}
```

### Manual Initialization

If you need to control when initialization starts:

1. Set `_autoInitialize` to false in the inspector
2. Call `InitializeApplication()` when you're ready to start

```csharp
[SerializeField] private GameStartUpController _startUpController;

private void StartGame()
{
    _startUpController.InitializeApplication();
}
```

### Handling Initialization Completion

You can subscribe to the `OnInitializationCompleted` event to know when all steps have been executed:

```csharp
private void Awake()
{
    StartUpController.OnInitializationCompleted += OnStartUpCompleted;
}

private void OnStartUpCompleted()
{
    Debug.Log("All initialization steps have completed!");
    LoadMainMenu();
}

private void OnDestroy()
{
    StartUpController.OnInitializationCompleted -= OnStartUpCompleted;
}
```

## Code Stripping Prevention

This package includes a `link.xml` file that prevents code stripping in IL2CPP builds by preserving:

- All types in the StartUp assembly
- The BaseStep class
- All classes that derive from BaseStep

This ensures that reflection-based step creation works correctly in all build configurations.

## Best Practices

1. **Keep Steps Focused**: Each step should handle a specific initialization concern
2. **Consider Dependencies**: Organize steps to respect initialization order dependencies
3. **Handle Errors**: Each step should handle its errors and provide meaningful messages
4. **Use Async When Needed**: For operations that may take time (network, asset loading)
5. **Log Progress**: Use the OnStepCompleted event to track initialization progress