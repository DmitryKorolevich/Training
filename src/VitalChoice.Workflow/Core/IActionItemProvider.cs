using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Domain.Workflow;

namespace VitalChoice.Workflow.Core
{
    public interface IActionItemProvider
    {
        Task<HashSet<ActionItem>> GetDependencyItems(string treeName);
    }
}
