using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Business.Queries.Workflow;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Ecommerce.Domain.Entities.Workflow;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Workflow.Core;
using VitalChoice.Workflow.Data;

namespace VitalChoice.Business.Services.Workflow
{
    public class ActionItemProvider : IActionItemProvider
    {
        private readonly IEcommerceRepositoryAsync<WorkflowTree> _treeRepository;
        private readonly IEcommerceRepositoryAsync<WorkflowExecutor> _executors;
        private readonly IEcommerceRepositoryAsync<WorkflowResolverPath> _resolverPaths;

        private Dictionary<ExecutorKey, HashSet<ActionItem>> _aggregations;
        private Dictionary<ExecutorKey, HashSet<ActionItem>> _depencies;
        private Dictionary<ExecutorKey, Dictionary<int, ActionItem>> _resolvers;

        public ActionItemProvider(IEcommerceRepositoryAsync<WorkflowTree> treeRepository,
            IEcommerceRepositoryAsync<WorkflowExecutor> executors,
            IEcommerceRepositoryAsync<WorkflowResolverPath> resolverPaths)
        {
            _treeRepository = treeRepository;
            _executors = executors;
            _resolverPaths = resolverPaths;
        }

        public async Task<Type> GetTreeType(string treeName)
        {
            var tree =
                await _treeRepository.Query(new WorkflowTreeQuery().WithName(treeName)).SelectFirstOrDefaultAsync(false);
            if (tree == null)
                throw new ApiException($"Tree <{treeName}> not found");
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
                throw new ApiException($"Tree <{treeName}> not found");

            return
                new HashSet<ActionItem>(
                    tree.Actions.Select(a => new ActionItem(a.Executor.ImplementationType, a.Executor.Name)
                    {
                        WorkflowActionType = a.Executor.ActionType
                    }));
        }

        public async Task<Dictionary<int, ActionItem>> GetActionResolverPaths(string actionName, Type implementation)
        {
            if (_resolvers == null)
            {
                _resolvers = new Dictionary<ExecutorKey, Dictionary<int, ActionItem>>();
                var actions =
                    await
                        _resolverPaths.Query()
                            .Include(r => r.Resolver)
                            .Include(r => r.Executor)
                            .SelectAsync(false);
                var results = actions.GroupBy(a => new ExecutorKey(a.Resolver.Name, a.Resolver.ImplementationType));
                foreach (var group in results)
                {
                    var dict = group.ToDictionary(a => a.Path, a => new ActionItem(a.Executor.ImplementationType, a.Executor.Name)
                    {
                        WorkflowActionType = a.Executor.ActionType
                    });
                    _resolvers.Add(group.Key, dict);
                }
            }
            Dictionary<int, ActionItem> result;
            if (_resolvers.TryGetValue(new ExecutorKey(actionName, implementation.FullName), out result))
            {
                return result;
            }
            throw new ApiException($"Action <{actionName}> not found");
        }

        public async Task<HashSet<ActionItem>> GetDependencies(string actionName, Type implementation)
        {
            if (_depencies == null)
            {
                _depencies = new Dictionary<ExecutorKey, HashSet<ActionItem>>();
                var results =
                    await _executors.Query()
                        .Include(t => t.Dependencies)
                        .ThenInclude(ta => ta.Dependent)
                        .SelectAsync(false);
                foreach (var action in results)
                {
                    var set = new HashSet<ActionItem>(
                        action.Dependencies.Select(a => new ActionItem(a.Dependent.ImplementationType, a.Dependent.Name)
                        {
                            WorkflowActionType = a.Dependent.ActionType
                        }));
                    _depencies.Add(new ExecutorKey(action.Name, action.ImplementationType), set);
                }
            }
            HashSet<ActionItem> result;
            if (_depencies.TryGetValue(new ExecutorKey(actionName, implementation.FullName), out result))
            {
                return result;
            }
            throw new ApiException($"Action <{actionName}> not found");
        }

        public async Task<HashSet<ActionItem>> GetAggregations(string actionName, Type implementation)
        {
            if (_aggregations == null)
            {
                _aggregations = new Dictionary<ExecutorKey, HashSet<ActionItem>>();
                var results =
                    await _executors.Query()
                        .Include(t => t.Aggreagations)
                        .ThenInclude(ta => ta.ToAggregate)
                        .SelectAsync(false);
                foreach (var action in results)
                {
                    var set = new HashSet<ActionItem>(
                        action.Aggreagations.Select(
                            a => new ActionItem(a.ToAggregate.ImplementationType, a.ToAggregate.Name)
                            {
                                WorkflowActionType = a.ToAggregate.ActionType
                            }));
                    _aggregations.Add(new ExecutorKey(action.Name, action.ImplementationType), set);
                }
            }
            HashSet<ActionItem> result;
            if (_aggregations.TryGetValue(new ExecutorKey(actionName, implementation.FullName), out result))
            {
                return result;
            }
            throw new ApiException($"Action <{actionName}> not found");
        }

        private struct ExecutorKey : IEquatable<ExecutorKey>
        {
            private readonly string _actionName;
            private readonly string _implementation;

            public ExecutorKey(string actionName, string implementation)
            {
                _actionName = actionName;
                _implementation = implementation;
            }

            public bool Equals(ExecutorKey other)
            {
                return string.Equals(_actionName, other._actionName) &&
                       string.Equals(_implementation, other._implementation);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is ExecutorKey && Equals((ExecutorKey) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (_actionName.GetHashCode()*397) ^ _implementation.GetHashCode();
                }
            }

            public static bool operator ==(ExecutorKey left, ExecutorKey right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(ExecutorKey left, ExecutorKey right)
            {
                return !left.Equals(right);
            }
        }
    }
}