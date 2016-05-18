using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Data.Transaction;
using VitalChoice.Ecommerce.Domain.Entities.Workflow;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Interfaces.Services;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Services.Workflow
{
    public class TreeSetupCleaner : ITreeSetupCleaner
    {
        private readonly IEcommerceRepositoryAsync<WorkflowTree> _treeRepository;
        private readonly IEcommerceRepositoryAsync<WorkflowExecutor> _executorsRepository;
        private readonly IEcommerceRepositoryAsync<WorkflowResolverPath> _resolverPathsRepository;
        private readonly IEcommerceRepositoryAsync<WorkflowActionDependency> _actionDependenciesRepository;
        private readonly IEcommerceRepositoryAsync<WorkflowActionAggregation> _actionAggregationsRepository;
        private readonly ILogger _logger;
        private readonly ITransactionAccessor<EcommerceContext> _transactionAccessor;

        public TreeSetupCleaner(IEcommerceRepositoryAsync<WorkflowTree> treeRepository,
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
            _logger = loggerProvider.CreateLogger<TreeSetupCleaner>();
        }

        public async Task<bool> CleanAllTrees()
        {
            using (var transaction = _transactionAccessor.BeginTransaction())
            {
                try
                {
                    //Wipe out everything
                    var trees = await _treeRepository.Query().SelectAsync();
                    var dependencies = await _actionDependenciesRepository.Query().SelectAsync();
                    var aggregations = await _actionAggregationsRepository.Query().SelectAsync();
                    var resolverPaths = await _resolverPathsRepository.Query().SelectAsync();
                    var executors = await _executorsRepository.Query().SelectAsync();
                    await _treeRepository.DeleteAllAsync(trees);
                    await _actionAggregationsRepository.DeleteAllAsync(aggregations);
                    await _actionDependenciesRepository.DeleteAllAsync(dependencies);
                    await _resolverPathsRepository.DeleteAllAsync(resolverPaths);
                    await _executorsRepository.DeleteAllAsync(executors);
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