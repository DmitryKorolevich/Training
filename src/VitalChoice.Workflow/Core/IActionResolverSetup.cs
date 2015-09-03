namespace VitalChoice.Workflow.Core
{
    public interface IActionResolverSetup
    {
        IActionResolverSetup Action<T>(int key);
    }
}