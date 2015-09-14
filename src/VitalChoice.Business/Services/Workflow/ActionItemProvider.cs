using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Templates.Helpers;
using VitalChoice.Business.Queries.Workflow;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Domain.Entities.Workflow;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Domain.Workflow;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Services.Workflow
{
    public class ActionItemProvider : IActionItemProvider
    {
        private readonly IEcommerceRepositoryAsync<WorkflowTree> _treeRepository;
        private readonly IEcommerceRepositoryAsync<WorkflowExecutor> _executors;
        private readonly IEcommerceRepositoryAsync<WorkflowResolverPath> _resolverPaths;

        public ActionItemProvider(IEcommerceRepositoryAsync<WorkflowTree> treeRepository, IEcommerceRepositoryAsync<WorkflowExecutor> executors, IEcommerceRepositoryAsync<WorkflowResolverPath> resolverPaths)
        {
            _treeRepository = treeRepository;
            _executors = executors;
            _resolverPaths = resolverPaths;
        }

        public async Task<Type> GetTreeType(string treeName)
        {
            var tree = await _treeRepository.Query(new WorkflowTreeQuery().WithName(treeName)).SelectFirstOrDefaultAsync(false);
            if (tree == null)
                throw new ApiException($"Tree {treeName} not found");
            return ReflectionHelper.ResolveType(tree.ImplementationType);
        }

        public async Task<HashSet<ActionItem>> GetTreeActions(string treeName)
        {
            var result =
                await _treeRepository.Query(new WorkflowTreeQuery().WithName(treeName))
                    .Include(t => t.Actions)
                    .ThenInclude(ta => ta.Executor)
                    .SelectAsync(false);
            var tree = result.SingleOrDefault();
            if (tree == null)
                throw new ApiException($"Tree {treeName} not found");

            return
                new HashSet<ActionItem>(
                    tree.Actions.Select(a => new ActionItem(a.Executor.ImplementationType, a.Executor.Name)
                    {
                        WorkflowActionType = a.Executor.ActionType
                    }));
        }

        public async Task<Dictionary<int, ActionItem>> GetActionResolverPaths(string actionName)
        {
            var actions =
                await
                    _resolverPaths.Query(p => p.Resolver.Name == actionName)
                        .Include(r => r.Resolver)
                        .Include(r => r.Executor)
                        .SelectAsync(false);
            if (!actions.Any())
                throw new ApiException($"Action {actionName} not found");

            return actions.ToDictionary(a => a.Path, a => new ActionItem(a.Executor.ImplementationType, a.Name)
            {
                WorkflowActionType = a.Executor.ActionType
            });
        }

        public async Task<HashSet<ActionItem>> GetDependencies(string actionName)
        {
            var result =
                await _executors.Query(new WorkflowExecutorQuery().WithName(actionName))
                    .Include(t => t.Dependencies)
                    .ThenInclude(ta => ta.Dependent)
                    .SelectAsync(false);
            var action = result.SingleOrDefault();
            if (action == null)
                throw new ApiException($"Action {actionName} not found");

            return
                new HashSet<ActionItem>(
                    action.Dependencies.Select(a => new ActionItem(a.Dependent.ImplementationType, a.Dependent.Name)
                    {
                        WorkflowActionType = a.Dependent.ActionType
                    }));
        }
    }
}