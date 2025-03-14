using System;

namespace StartUp.Runtime
{
    internal static class StepFactory
    {
        internal static BaseStep CreateStep(Type stepType)
        {
            if (typeof(BaseStep).IsAssignableFrom(stepType) is false)
                throw new ArgumentException($"Type {stepType.Name} does not derive from BaseStep");

            return Activator.CreateInstance(stepType) as BaseStep;
        }
    }
}