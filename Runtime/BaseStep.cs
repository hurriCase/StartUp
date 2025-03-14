using System;
using System.Threading.Tasks;
using UnityEngine;

namespace StartUp.Runtime
{
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
}