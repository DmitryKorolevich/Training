using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Data.Transaction;
using VitalChoice.Ecommerce.Domain.Entities.Workflow;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Interfaces.Services;
using VitalChoice.Workflow.Core;
using VitalChoice.Workflow.Data;
using VitalChoice.Ecommerce.Domain.Exceptions;

namespace VitalChoice.Business.Services.Workflow
{
    public class TreeSetup<TContext, TResult> : ITreeSetup<TContext, TResult>
        where TContext : WorkflowDataContext<TResult>
    {
        private readonly IEcommerceRepositoryAsync<WorkflowTree> _treeRepository;
        private readonly IEcommerceRepositoryAsync<WorkflowExecutor> _executorsRepository;
        private readonly IEcommerceRepositoryAsync<WorkflowResolverPath> _resolverPathsRepository;
        private readonly IEcommerceRepositoryAsync<WorkflowActionDependency> _actionDependenciesRepository;
        private readonly IEcommerceRepositoryAsync<WorkflowActionAggregation> _actionAggregationsRepository;
        private readonly ILogger _logger;
        private readonly ITransactionAccessor<EcommerceContext> _transactionAccessor;

        public TreeSetup(
            IEcommerceRepositoryAsync<WorkflowTree> treeRepository,
            IEcommerceRepositoryAsync<WorkflowExecutor> executorsRepository,
            IEcommerceRepositoryAsync<WorkflowResolverPath> resolverPathsRepository,
            IEcommerceRepositoryAsync<WorkflowActionDependency> actionDependenciesRepository,
            IEcommerceRepositoryAsync<WorkflowActionAggregation> actionAggregationsRepository,
            ILoggerProviderExtended loggerProvider, ITransactionAccessor<EcommerceContext> transactionAccessor)
        {
            _treeRepository = treeRepository;
            _executorsRepository = executorsRepository;
            _resolverPathsRepository = resolverPathsRepository;
            _actionDependenciesRepository = actionDependenciesRepository;
            _actionAggregationsRepository = actionAggregationsRepository;
            _transactionAccessor = transactionAccessor;
            _logger = loggerProvider.CreateLogger<TreeSetup<TContext, TResult>>();
            Trees = new Dictionary<Type, WorkflowTreeDefinition>();
        }

        public Dictionary<Type, WorkflowTreeDefinition> Trees { get; }

        public ITreeSetup<TContext, TResult> Tree<T>(string treeName, Action<ITreeActionSetup<TContext, TResult>> actions)
            where T : IWorkflowTree<TContext, TResult>
        {
            if (actions == null)
                throw new ArgumentNullException(nameof(actions));

            var tree = new WorkflowTreeDefinition(typeof(T), treeName);
            var actionSetup = new TreeActionSetup<TContext, TResult>();
            actions(actionSetup);
            tree.Actions = actionSetup.Actions;
            tree.ActionResolvers = actionSetup.ActionResolvers;
            Trees.Add(typeof(T), tree);
            return this;
        }

        public async Task<bool> CreateTreesAsync()
        {
            using (var transaction = _transactionAccessor.BeginTransaction())
            {
                try
                {
                    //Insert trees
                    var dbTrees = Trees.Select(t => new WorkflowTree
                    {
                        Name = t.Value.Name,
                        ImplementationType = t.Key.FullName,
                        Actions = t.Value.Actions.Select(type => new WorkflowExecutor
                        {
                            ActionType = WorkflowActionType.Action,
                            Name = type.Value.Name,
                            ImplementationType = type.Key.FullName
                        }).Union(t.Value.ActionResolvers.Select(type => new WorkflowExecutor
                        {
                            ActionType = WorkflowActionType.ActionResolver,
                            Name = type.Value.Name,
                            ImplementationType = type.Key.FullName
                        })).ToList()
                    });
                    await _treeRepository.InsertGraphRangeAsync(dbTrees);

                    foreach (var tree in Trees)
                    {
                        var dbTree =
                            await
                                _treeRepository.Query(t => t.ImplementationType == tree.Key.FullName && t.Name == tree.Value.Name)
                                    .Include(t => t.Actions)
                                    .SelectFirstOrDefaultAsync(false);

                        var actions = dbTree.Actions.ToDictionary(e => e.ImplementationType, e => e.Id);

                        //Insert action dependencies
                        var dbActions = new List<WorkflowActionDependency>();
                        foreach (var action in tree.Value.Actions)
                        {
                            dbActions.AddRange(action.Value.Dependencies.Select(dependency => new WorkflowActionDependency
                            {
                                IdParent = actions[action.Key.FullName],
                                IdDependent = actions[dependency.FullName]
                            }));
                        }
                        foreach (var action in tree.Value.ActionResolvers)
                        {
                            dbActions.AddRange(action.Value.Dependencies.Select(dependency => new WorkflowActionDependency
                            {
                                IdParent = actions[action.Key.FullName],
                                IdDependent = actions[dependency.FullName]
                            }));
                        }
                        await _actionDependenciesRepository.InsertRangeAsync(dbActions);

                        //Insert action aggregations
                        var dbAggregations = new List<WorkflowActionAggregation>();
                        foreach (var action in tree.Value.Actions)
                        {
                            dbAggregations.AddRange(
                                action.Value.Aggregations.Select(dependency => new WorkflowActionAggregation
                                {
                                    IdParent = actions[action.Key.FullName],
                                    IdAggregate = actions[dependency.FullName]
                                }));
                        }
                        await _actionAggregationsRepository.InsertRangeAsync(dbAggregations);

                        //Insert action resolver paths
                        var dbActionResolvers = new List<WorkflowResolverPath>();
                        foreach (var resolver in tree.Value.ActionResolvers)
                        {
                            foreach (var action in resolver.Value.Actions)
                            {
                                if (!actions.ContainsKey(action.Value.Type.FullName))
                                {
                                    throw new ApiException(
                                        $"{action.Value.Type.FullName} is not initialized either as action or action resolver.");
                                }
                                var path = new WorkflowResolverPath
                                {
                                    IdResolver = actions[resolver.Key.FullName],
                                    IdExecutor = actions[action.Value.Type.FullName],
                                    Path = action.Key,
                                    Name = action.Value.Name
                                };
                                dbActionResolvers.Add(path);
                            }
                        }
                        await _resolverPathsRepository.InsertRangeAsync(dbActionResolvers);
                    }
                    transaction.Commit();
                    return true;
                }
                catch (Exception e)
                {
                    _logger.LogCritical(0, "Cannot update workflow manager DB", e);
                    transaction.Rollback();
                    return false;
                }
            }
        }
    }
}