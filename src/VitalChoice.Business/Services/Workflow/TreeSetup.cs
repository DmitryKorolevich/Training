﻿using System;
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
        private ITransactionAccessor<EcommerceContext> _transactionAccessor;

        public TreeSetup(
            IEcommerceRepositoryAsync<WorkflowTree> treeRepository,
            IEcommerceRepositoryAsync<WorkflowExecutor> executorsRepository,
            IEcommerceRepositoryAsync<WorkflowResolverPath> resolverPathsRepository,
            IEcommerceRepositoryAsync<WorkflowActionDependency> actionDependenciesRepository,
            IEcommerceRepositoryAsync<WorkflowActionAggregation> actionAggregationsRepository, EcommerceContext context,
            ILoggerProviderExtended loggerProvider, ITransactionAccessor<EcommerceContext> transactionAccessor)
        {
            _treeRepository = treeRepository;
            _executorsRepository = executorsRepository;
            _resolverPathsRepository = resolverPathsRepository;
            _actionDependenciesRepository = actionDependenciesRepository;
            _actionAggregationsRepository = actionAggregationsRepository;
            _transactionAccessor = transactionAccessor;
            _logger = loggerProvider.CreateLoggerDefault();
            Trees = new Dictionary<Type, WorkflowTreeDefinition>();
            Actions = new Dictionary<Type, WorkflowActionDefinition>();
            ActionResolvers = new Dictionary<Type, WorkflowActionResolverDefinition>();
        }

        internal Dictionary<Type, WorkflowTreeDefinition> Trees { get; }
        internal Dictionary<Type, WorkflowActionDefinition> Actions { get; }
        internal Dictionary<Type, WorkflowActionResolverDefinition> ActionResolvers { get; }

        public ITreeSetup<TContext, TResult> Tree<T>(string treeName, Action<ITreeActionSetup<TContext, TResult>> actions)
            where T : IWorkflowTree<TContext, TResult>
        {
            if (actions == null)
                throw new ArgumentNullException(nameof(actions));

            var tree = new WorkflowTreeDefinition(typeof (T), treeName);
            var actionSetup = new TreeActionSetup<TContext, TResult>();
            actions(actionSetup);
            tree.Actions = actionSetup.Actions;
            Trees.Add(typeof (T), tree);
            return this;
        }

        public ITreeSetup<TContext, TResult> Action<T>(string actionName,
            Action<IActionSetup<TContext, TResult>> actions = null)
            where T : IWorkflowAction<TContext, TResult>
        {
            var action = new WorkflowActionDefinition(typeof (T), actionName);
            if (actions != null)
            {
                var actionSetup = new ActionSetup<TContext, TResult>();
                actions(actionSetup);
                action.Dependencies = actionSetup.Dependencies;
                action.Aggregations = actionSetup.Aggregations;
            }
            Actions.Add(typeof (T), action);
            return this;
        }

        public ITreeSetup<TContext, TResult> ActionResolver<T>(string actionName,
            Action<IActionResolverSetup<TContext, TResult>> actions)
            where T : IWorkflowActionResolver<TContext, TResult>
        {
            if (actions == null)
                throw new ArgumentNullException(nameof(actions));

            var action = new WorkflowActionResolverDefinition(typeof (T), actionName);
            var actionSetup = new ActionResolverSetup<TContext, TResult>();
            actions(actionSetup);
            action.Actions = actionSetup.Actions;
            action.Dependencies = actionSetup.Dependencies;
            ActionResolvers.Add(typeof (T), action);
            return this;
        }

        public async Task<bool> UpdateAsync()
        {
            var trees = await _treeRepository.Query().SelectAsync();
            var dependencies = await _actionDependenciesRepository.Query().SelectAsync();
            var aggregations = await _actionAggregationsRepository.Query().SelectAsync();
            var resolverPaths = await _resolverPathsRepository.Query().SelectAsync();
            var executors = await _executorsRepository.Query().SelectAsync();
            using (var transaction = _transactionAccessor.BeginTransaction())
            {
                try
                {
                    //Wipe out everything
                    await _treeRepository.DeleteAllAsync(trees);
                    await _actionAggregationsRepository.DeleteAllAsync(aggregations);
                    await _actionDependenciesRepository.DeleteAllAsync(dependencies);
                    await _resolverPathsRepository.DeleteAllAsync(resolverPaths);
                    await _executorsRepository.DeleteAllAsync(executors);
                    
                    //Insert executors
                    var dbExecutors = Actions.Select(a => new WorkflowExecutor
                    {
                        Name = a.Value.Name,
                        ImplementationType = a.Key.FullName,
                        ActionType = WorkflowActionType.Action
                    }).Union(ActionResolvers.Select(a => new WorkflowExecutor
                    {
                        Name = a.Value.Name,
                        ImplementationType = a.Key.FullName,
                        ActionType = WorkflowActionType.ActionResolver
                    })).ToList();
                    await _executorsRepository.InsertRangeAsync(dbExecutors);

                    var actions = dbExecutors.ToDictionary(e => e.ImplementationType, e => e.Id);

                    //Insert action dependencies
                    var dbActions = new List<WorkflowActionDependency>();
                    foreach (var action in Actions)
                    {
                        dbActions.AddRange(action.Value.Dependencies.Select(dependency => new WorkflowActionDependency
                        {
                            IdParent = actions[action.Key.FullName],
                            IdDependent = actions[dependency.FullName]
                        }));
                    }
                    foreach (var action in ActionResolvers)
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
                    foreach (var action in Actions)
                    {
                        dbAggregations.AddRange(
                            action.Value.Aggregations.Select(dependency => new WorkflowActionAggregation
                            {
                                IdParent = actions[action.Key.FullName],
                                IdAggregate = actions[dependency.FullName]
                            }));
                    }
                    await _actionAggregationsRepository.InsertRangeAsync(dbAggregations);

                    //Insert trees
                    var dbTrees = Trees.Select(t => new WorkflowTree
                    {
                        Name = t.Value.Name,
                        ImplementationType = t.Key.FullName,
                        Actions = t.Value.Actions.Select(type => new WorkflowTreeAction
                        {
                            IdExecutor = actions[type.FullName]
                        }).ToList()
                    });
                    await _treeRepository.InsertGraphRangeAsync(dbTrees);

                    //Insert action resolver paths
                    var dbActionResolvers = new List<WorkflowResolverPath>();
                    foreach (var resolver in ActionResolvers)
                    {
                        dbActionResolvers.AddRange(resolver.Value.Actions.Select(action => new WorkflowResolverPath
                        {
                            IdResolver = actions[resolver.Key.FullName],
                            IdExecutor = actions[action.Value.Type.FullName],
                            Path = action.Key,
                            Name = action.Value.Name
                        }));
                    }
                    await _resolverPathsRepository.InsertRangeAsync(dbActionResolvers);

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