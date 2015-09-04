using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Domain.Workflow;

namespace VitalChoice.Workflow.Core
{
    public interface IActionItemProvider
    {
        Task<Type> GetTreeType(string treeName);
        Task<HashSet<ActionItem>> GetDependencyItems(string treeName);
        Task<Dictionary<int, ActionItem>> GetActionResolverDependencyItems(string actionName);
        Task<HashSet<ActionItem>> GetActionDependencyItems(string actionName);
    }
}
