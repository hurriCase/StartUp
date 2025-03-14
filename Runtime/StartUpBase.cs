using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace StartUp.Runtime
{
    public abstract class StartUpBase : MonoBehaviour
    {
        public static bool IsInited { get; private set; }
        public static event Action OnInitializationCompleted;

        [SerializeField] private bool _autoInitialize = true;

        protected virtual IReadOnlyList<Type> StepTypes => new List<Type>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStaticMembers()
        {
            OnInitializationCompleted = null;
            IsInited = false;
        }

        protected virtual void Awake()
        {
            if (_autoInitialize)
            {
                InitializeApplication();
            }
        }

        public virtual async void InitializeApplication()
        {
            try
            {
                if (IsInited)
                    return;

                await InitializeSteps();

                IsInited = true;
                OnInitializationCompleted?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError(
                    $"[StartUpController::InitializeApplication] Initialization failed, with error: {e.Message}");
            }
        }

        protected virtual async Task InitializeSteps()
        {
            for (var i = 0; i < StepTypes.Count; i++)
            {
                var step = StepFactory.CreateStep(StepTypes[i]);
                step.OnStepCompleted += LogStepCompletion;
                await step.Execute(i);
            }
        }

        protected virtual void LogStepCompletion(int step, string stepName)
        {
            Debug.Log($"[StartUpController::LogStepCompletion] Step {step} completed: {stepName}");
        }

        protected virtual void OnDestroy()
        {
            OnInitializationCompleted = null;
        }
    }
}