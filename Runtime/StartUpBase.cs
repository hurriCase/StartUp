using System;
using System.Collections.Generic;
using UnityEngine;

namespace StartUp.Runtime
{
    public abstract class StartUpBase
    {
        public static bool IsInited { get; private set; }
        public static event Action OnInitializationCompleted;

        private static readonly List<Type> _stepTypesList = new();

        public static void RegisterSteps(params Type[] steps)
        {
            foreach (var step in steps)
            {
                if (typeof(BaseStep).IsAssignableFrom(step))
                    _stepTypesList.Add(step);
                else
                    Debug.LogError($"[StartUpBase::RegisterSteps] Type {step.Name} does not derive from BaseStep");
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStaticMembers()
        {
            OnInitializationCompleted = null;
            IsInited = false;
        }

        public static async void InitializeApplication()
        {
            try
            {
                if (IsInited)
                    return;

                for (var i = 0; i < _stepTypesList.Count; i++)
                {
                    var step = StepFactory.CreateStep(_stepTypesList[i]);
                    step.OnStepCompleted += LogStepCompletion;
                    await step.Execute(i);
                }

                IsInited = true;
                OnInitializationCompleted?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError(
                    $"[StartUpController::InitializeApplication] Initialization failed, with error: {e.Message}");
            }
        }

        private static void LogStepCompletion(int step, string stepName)
        {
            Debug.Log($"[StartUpController::LogStepCompletion] Step {step} completed: {stepName}");
        }
    }
}