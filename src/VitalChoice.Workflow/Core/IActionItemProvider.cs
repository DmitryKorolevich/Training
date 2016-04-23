using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Workflow.Data;

namespace VitalChoice.Workflow.Core
{
    public interface IActionItemProvider
    {
        Task<Type> GetTreeType(string treeName);
        Task<HashSet<ActionItem>> GetTreeActions(string treeName);
        Task<Dictionary<int, ActionItem>> GetActionResolverPaths(string actionName, Type implementation);
        Task<HashSet<ActionItem>> GetDependencies(string actionName, Type implementation);
        Task<HashSet<ActionItem>> GetAggregations(string actionName, Type implementation);
    }
}
