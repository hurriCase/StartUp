# Startup System

## Overview
The Startup System provides a structured way to initialize your Unity application in sequential steps. Each step is executed asynchronously, allowing for complex initialization processes without blocking the main thread.

## Features
- Sequential execution of initialization steps
- Async/await pattern for non-blocking operations
- Automatic step registration and execution
- Event-based completion notifications
- Exception handling for initialization failures

## Architecture

### BaseStep
The foundation class for all initialization steps.

```csharp
public abstract class BaseStep
{
    public event Action<int, string> OnStepCompleted;

    internal virtual async Task Execute(int step)
    {
        try
        {
            await ExecuteInternal();
            OnStepCompleted?.Invoke(step, GetType().Name);
        }
        catch (Exception e)
        {
            Debug.LogError($"[{GetType().Name}::Execute] Step initialization failed: {e.Message}");
        }
    }

    protected abstract Task ExecuteInternal();
}
```

### StartUpBase
The controller class that manages step registration and execution.

```csharp
public abstract class StartUpBase
{
    public static bool IsInited { get; private set; }
    public static event Action OnInitializationCompleted;

    private static readonly List<Type> _stepTypesList = new();

    public static void RegisterSteps(params Type[] steps) { /* ... */ }
    public static async void InitializeApplication() { /* ... */ }
}
```

### StepFactory
An internal factory class that creates step instances.

```csharp
internal static class StepFactory
{
    internal static BaseStep CreateStep(Type stepType) { /* ... */ }
}
```

## How to Use

### 1. Create Your Initialization Steps

Create concrete step classes by inheriting from `BaseStep` and implementing the required method:

```csharp
using System.Threading.Tasks;
using StartUp.Runtime;

public class LoadConfigurationStep : BaseStep
{
    protected override async Task ExecuteInternal()
    {
        // Load configurations asynchronously
        await Task.Delay(100); // Simulating work
        // Your initialization logic here
    }
}

public class InitializeServicesStep : BaseStep
{
    protected override async Task ExecuteInternal()
    {
        // Initialize services asynchronously
        await Task.Delay(200); // Simulating work
        // Your initialization logic here
    }
}
```

### 2. Register Your Steps

Register your initialization steps in your game's entry point (such as a bootstrapper class):

```csharp
using StartUp.Runtime;
using UnityEngine;

public class GameBootstrapper : MonoBehaviour
{
    private void Awake()
    {
        StartUpBase.RegisterSteps(
            typeof(LoadConfigurationStep),
            typeof(InitializeServicesStep),
            typeof(SetupUIStep),
            typeof(LoadPlayerDataStep)
        );
        
        StartUpBase.OnInitializationCompleted += OnAppInitialized;
        StartUpBase.InitializeApplication();
    }
    
    private void OnAppInitialized()
    {
        Debug.Log("Application initialization completed!");
        // Start your game here
    }
}
```

### 3. Monitor Initialization Progress

You can also subscribe to individual step completions if needed:

```csharp
private void SubscribeToSteps()
{
    // This would need to be implemented differently since steps are created internally
    // One approach is to add a static event in StartUpBase for step completion
    StartUpBase.OnStepCompleted += (stepIndex, stepName) => 
    {
        Debug.Log($"Step {stepIndex}: {stepName} completed");
        // Update loading UI
    };
}
```

## Best Practices

1. **Keep Steps Focused**: Each step should have a single responsibility.
2. **Handle Exceptions**: Although the system catches exceptions, handle expected failures gracefully in your step implementation.
3. **Avoid Dependencies Between Steps**: Design steps to be as independent as possible. If dependencies exist, ensure they're registered in the correct order.
4. **Use Async Operations**: Leverage the async nature of the system for operations like loading assets, connecting to servers, etc.
5. **Clean Up**: If a step allocates resources that need to be released, implement a cleanup mechanism.

## Example Implementation

```csharp
// Example of a complete step that loads player data
public class LoadPlayerDataStep : BaseStep
{
    protected override async Task ExecuteInternal()
    {
        // Check if local cache exists
        bool hasLocalData = CheckLocalDataCache();
        
        if (hasLocalData)
        {
            // Load from local cache
            await LoadFromLocalCache();
        }
        else
        {
            // Try loading from cloud
            try
            {
                await LoadFromCloud();
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Could not load from cloud: {e.Message}");
                // Create new player data
                CreateNewPlayerData();
            }
        }
        
        // Register player data with service locator or similar
        RegisterPlayerData();
    }
    
    // Implementation details...
}
```

## Extending the System

You can extend the system by:

1. Creating a concrete implementation of `StartUpBase` for your specific game
2. Adding progress reporting mechanisms
3. Implementing step dependencies
4. Adding step priorities for more complex initialization flows

---

This system provides a robust foundation for organizing your game's initialization process in a clear, maintainable way.