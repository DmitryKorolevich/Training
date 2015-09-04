namespace VitalChoice.Workflow.Core
{
    public interface IActionSetup
    {
        IActionSetup Action<T>();
        IActionSetup ActionResolver<T>();
    }
}