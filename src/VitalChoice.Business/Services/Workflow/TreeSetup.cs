using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Shared.Helpers;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Data.Transaction;
using VitalChoice.Domain.Entities.Workflow;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Domain.Workflow;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Services.Workflow
{
    public class TreeSetup : ITreeSetup
    {
        private readonly IEcommerceRepositoryAsync<WorkflowTreeAction> _treeActionsRepository;
        private readonly IEcommerceRepositoryAsync<WorkflowTree> _treeRepository;
        private readonly IEcommerceRepositoryAsync<WorkflowExecutor> _executorsRepository;
        private readonly IEcommerceRepositoryAsync<WorkflowResolverPath> _resolverPathsRepository;
        private readonly EcommerceContext _context;

        public TreeSetup(IEcommerceRepositoryAsync<WorkflowTreeAction> treeActionsRepository,
            IEcommerceRepositoryAsync<WorkflowTree> treeRepository,
            IEcommerceRepositoryAsync<WorkflowExecutor> executorsRepository, IEcommerceRepositoryAsync<WorkflowResolverPath> resolverPathsRepository, EcommerceContext context )
        {
            _treeActionsRepository = treeActionsRepository;
            _treeRepository = treeRepository;
            _executorsRepository = executorsRepository;
            _resolverPathsRepository = resolverPathsRepository;
            _context = context;
            Trees = new Dictionary<Type, WorkflowTreeDefinition>();
            Actions = new Dictionary<Type, WorkflowActionDefinition>();
            ActionResolvers = new Dictionary<Type, WorkflowActionResolverDefinition>();
        }

        internal Dictionary<Type, WorkflowTreeDefinition> Trees { get; }
        internal Dictionary<Type, WorkflowActionDefinition> Actions { get; }
        internal Dictionary<Type, WorkflowActionResolverDefinition> ActionResolvers { get; }

        public ITreeSetup Tree<T>(string treeName, Action<IActionSetup> actions)
        {
            if (actions == null)
                throw new ArgumentNullException(nameof(actions));

            if (!typeof (T).IsImplementGeneric(typeof (IWorkflowTree<,>)))
            {
                throw new ApiException($"Type {typeof(T)} doesn't implement IWorkflowTree<TContext, TResult>");
            }
            var tree = new WorkflowTreeDefinition(typeof (T), treeName);
            var actionSetup = new ActionSetup();
            actions(actionSetup);
            tree.Actions = actionSetup.Actions;
            Trees.Add(typeof(T), tree);
            return this;
        }

        public ITreeSetup Action<T>(string actionName, Action<IActionSetup> actions)
        {
            if (actions == null)
                throw new ArgumentNullException(nameof(actions));

            if (!typeof(T).IsImplementGeneric(typeof(IWorkflowAction<,>)))
            {
                throw new ApiException($"Type {typeof(T)} doesn't implement IWorkflowAction<TContext, TResult>");
            }
            var action = new WorkflowActionDefinition(typeof(T), actionName);
            var actionSetup = new ActionSetup();
            actions(actionSetup);
            action.Actions = actionSetup.Actions;
            Actions.Add(typeof(T), action);
            return this;
        }

        public ITreeSetup ActionResolver<T>(string actionName, Action<IActionResolverSetup> actions)
        {
            if (actions == null)
                throw new ArgumentNullException(nameof(actions));

            if (!typeof(T).IsImplementGeneric(typeof(IWorkflowActionResolver<,>)))
            {
                throw new ApiException($"Type {typeof(T)} doesn't implement IWorkflowActionResolver<TContext, TResult>");
            }
            var action = new WorkflowActionResolverDefinition(typeof(T), actionName);
            var actionSetup = new ActionResolverSetup();
            actions(actionSetup);
            action.Actions = actionSetup.Actions;
            ActionResolvers.Add(typeof(T), action);
            return this;
        }

        public async Task UpdateAsync()
        {
            ICollection<string> names = Trees.Select(t => t.Value.Name).ToList();
            using (var transaction = new TransactionAccessor(_context).BeginTransaction())
            {
                try
                {
                    var trees =
                        await
                            _treeRepository.Query(t => names.Contains(t.Name))
                                .Include(t => t.Actions)
                                .ThenInclude(a => a.Executor)
                                .ThenInclude(a => a.ResolverPaths)
                                .SelectAsync();
                    await _treeRepository.DeleteAllAsync(trees);
                    //TODO: Insert executors, insert tree actions, insert resolvers, add dependency table for action-to-action linkage, finish set up on tree construction
                    List<WorkflowTree> newTrees = new List<WorkflowTree>(Trees.Select(t => new WorkflowTree
                    {
                        Name = t.Value.Name,
                        ImplementationType = t.Value.Type.FullName,
                        Actions = t.Value.Actions.Select(type => new WorkflowTreeAction
                        {
                            Executor = new WorkflowExecutor
                            {
                                ImplementationType = type.FullName,
                                Name = GetActionName(type)
                            }
                        }).ToList()
                    }));

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        private string GetActionName(Type type)
        {
            WorkflowActionDefinition action;
            WorkflowActionResolverDefinition actionResolver;
            WorkflowTreeDefinition tree;
            if (Actions.TryGetValue(type, out action))
            {
                return action.Name;
            }
            if (ActionResolvers.TryGetValue(type, out actionResolver))
            {
                return actionResolver.Name;
            }
            if (Trees.TryGetValue(type, out tree))
            {
                return tree.Name;
            }
            throw new ApiException($"Action with type {type} not defined either as tree or action.");
        }
    }
}