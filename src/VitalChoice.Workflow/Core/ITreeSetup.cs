using System;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Workflow.Core
{
    public interface ITreeSetup
    {
        ITreeSetup Tree<T>(string treeName, Action<IActionSetup> actions);
        ITreeSetup Action<T>(string actionName, Action<IActionSetup> actions);
        ITreeSetup ActionResolver<T>(string actionName, Action<IActionResolverSetup> actions);
        Task UpdateAsync();
    }
}