using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Business.Queries.Workflow;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.Domain.Entities.Workflow;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Domain.Workflow;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Services.Workflow
{
    public class ActionItemProvider : IActionItemProvider
    {
        private readonly IReadRepositoryAsync<WorkflowTree> _treeRepository;

        public ActionItemProvider(IReadRepositoryAsync<WorkflowTree> treeRepository)
        {
            _treeRepository = treeRepository;
        }

        public async Task<HashSet<ActionItem>> GetDependencyItems(string treeName)
        {
            var result =
                await _treeRepository.Query(new WorkflowTreeQuery().WithName(treeName))
                    .Include(t => t.Actions)
                    .ThenInclude(ta => ta.Executor)
                    .SelectAsync();
            var tree = result.SingleOrDefault();
            if (tree == null)
                throw new ApiException($"Tree {treeName} not found");

            return
                new HashSet<ActionItem>(
                    tree.Actions.Select(a => new ActionItem(a.Executor.ImplementationType, a.Executor.Name)));
        }
    }
}