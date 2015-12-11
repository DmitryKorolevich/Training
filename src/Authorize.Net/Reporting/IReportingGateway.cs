using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Authorize.Net.Reporting
{
    public interface IReportingGateway
    {
        Task<List<Batch>> GetBatchStatistics(string batchId);
        Task<List<Batch>> GetSettledBatchList(bool includeStats);
        Task<List<Batch>> GetSettledBatchList(DateTime from, DateTime to, bool includeStats);
        Task<List<Batch>> GetSettledBatchList(DateTime from, DateTime to);
        Task<List<Batch>> GetSettledBatchList();
        Task<Transaction> GetTransactionDetails(string transactionId);
        Task<List<Transaction>> GetTransactionList(DateTime from, DateTime to);
        Task<List<Transaction>> GetTransactionList();
        Task<List<Transaction>> GetTransactionList(string batchId);
        Task<List<Transaction>> GetUnsettledTransactionList();
    }
}