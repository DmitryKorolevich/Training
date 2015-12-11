using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Authorize.Net.AIM;
using Authorize.Net.CIM;
using Authorize.Net.Utility;

namespace Authorize.Net.Reporting
{
    /// <summary>
    ///     The gateway for requesting Reports from Authorize.Net
    /// </summary>
    public class ReportingGateway : IReportingGateway
    {
        private readonly HttpXmlUtility _gateway;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CustomerGateway" /> class.
        /// </summary>
        /// <param name="apiLogin">The API login.</param>
        /// <param name="transactionKey">The transaction key.</param>
        /// <param name="mode">Test or Live.</param>
        public ReportingGateway(string apiLogin, string transactionKey, ServiceMode mode)
        {
            if (mode == ServiceMode.Live)
            {
                _gateway = new HttpXmlUtility(ServiceMode.Live, apiLogin, transactionKey);
            }
            else
            {
                _gateway = new HttpXmlUtility(ServiceMode.Test, apiLogin, transactionKey);
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CustomerGateway" /> class.
        /// </summary>
        /// <param name="apiLogin">The API login.</param>
        /// <param name="transactionKey">The transaction key.</param>
        public ReportingGateway(string apiLogin, string transactionKey) : this(apiLogin, transactionKey, ServiceMode.Test)
        {
        }

        /// <summary>
        ///     Returns all Settled Batches for the last 30 days
        /// </summary>
        public Task<List<Batch>> GetSettledBatchList(bool includeStats)
        {
            var from = DateTime.Today.AddDays(-30);
            var to = DateTime.Today;
            return GetSettledBatchList(from, to, includeStats);
        }


        /// <summary>
        ///     returns the most recent 1000 transactions that are unsettled
        /// </summary>
        /// <returns></returns>
        public async Task<List<Transaction>> GetUnsettledTransactionList()
        {
            var response = (getUnsettledTransactionListResponse) await _gateway.Send(new getUnsettledTransactionListRequest());
            return Transaction.NewListFromResponse(response.transactions);
        }

        /// <summary>
        ///     Returns all Settled Batches for the last 30 days
        /// </summary>
        public Task<List<Batch>> GetSettledBatchList()
        {
            var from = DateTime.Today.AddDays(-30);
            var to = DateTime.Today;
            return GetSettledBatchList(from, to, false);
        }

        /// <summary>
        ///     Returns batch settlements for the specified date range
        /// </summary>
        public Task<List<Batch>> GetSettledBatchList(DateTime from, DateTime to)
        {
            return GetSettledBatchList(from, to, false);
        }

        /// <summary>
        ///     Returns charges and statistics for a given batch
        /// </summary>
        /// <param name="batchId">the batch id</param>
        /// <returns>a batch with statistics</returns>
        public async Task<List<Batch>> GetBatchStatistics(string batchId)
        {
            var req = new getBatchStatisticsRequest {batchId = batchId};
            var response = (getBatchStatisticsResponse) await _gateway.Send(req);
            return Batch.NewFromResponse(response);
        }

        /// <summary>
        ///     Returns batch settlements for the specified date range
        /// </summary>
        public async Task<List<Batch>> GetSettledBatchList(DateTime from, DateTime to, bool includeStats)
        {
            var req = new getSettledBatchListRequest
            {
                firstSettlementDate = @from.ToUniversalTime(),
                lastSettlementDate = to.ToUniversalTime(),
                firstSettlementDateSpecified = true,
                lastSettlementDateSpecified = true
            };


            if (includeStats)
            {
                req.includeStatistics = true;
                req.includeStatisticsSpecified = true;
            }
            var response = (getSettledBatchListResponse) await _gateway.Send(req);

            return Batch.NewFromResponse(response);
        }

        /// <summary>
        ///     Returns all transaction within a particular batch
        /// </summary>
        public async Task<List<Transaction>> GetTransactionList(string batchId)
        {
            var req = new getTransactionListRequest {batchId = batchId};
            var response = (getTransactionListResponse) await _gateway.Send(req);
            return Transaction.NewListFromResponse(response.transactions);
        }

        /// <summary>
        ///     Returns Transaction details for a given transaction ID (transid)
        /// </summary>
        /// <param name="transactionId"></param>
        public async Task<Transaction> GetTransactionDetails(string transactionId)
        {
            var req = new getTransactionDetailsRequest {transId = transactionId};
            var response = (getTransactionDetailsResponse) await _gateway.Send(req);
            return Transaction.NewFromResponse(response.transaction);
        }

        /// <summary>
        ///     Returns all transactions for the last 30 days
        /// </summary>
        /// <returns></returns>
        public Task<List<Transaction>> GetTransactionList()
        {
            return GetTransactionList(DateTime.Today.AddDays(-30), DateTime.Today);
        }

        /// <summary>
        ///     Returns all transactions for a given time period. This can result in a number of calls to the API
        /// </summary>
        public async Task<List<Transaction>> GetTransactionList(DateTime from, DateTime to)
        {
            var batches = await GetSettledBatchList(from, to);
            var result = new List<Transaction>();
            foreach (var batch in batches)
            {
                result.AddRange(await GetTransactionList(batch.ID));
            }
            return result;
        }
    }
}