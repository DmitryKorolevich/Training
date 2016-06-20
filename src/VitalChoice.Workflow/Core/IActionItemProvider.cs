using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Workflow.Data;

namespace VitalChoice.Workflow.Core
{
    public interface IActionItemProvider
    {
        Task<TreeInfo> GetTreeInfo(string treeName);
        Task<HashSet<ActionItem>> GetTreeActions(string treeName);
        Task<Dictionary<int, ActionItem>> GetActionResolverPaths(int idTree, string actionName, Type implementation);
        Task<HashSet<ActionItem>> GetDependencies(int idTree, string actionName, Type implementation);
        Task<HashSet<ActionItem>> GetAggregations(int idTree, string actionName, Type implementation);
    }
}
