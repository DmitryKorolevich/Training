using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VitalChoice.Interfaces.Services
{
    public interface IGoogleService
    {
        void Test();

        Task<decimal> GetCartAbandon(DateTime startDate, DateTime endDate);

        Task<long> GetUniqueVisits(DateTime startDate, DateTime endDate);

        Task<decimal> GetNewWebVisitsPercent(DateTime startDate, DateTime endDate);

        Task<decimal> GetTransactionsRevenueOrganics(DateTime startDate, DateTime endDate);

        Task<decimal> GetTransactionsRevenuePaid(DateTime startDate, DateTime endDate);

        Task<decimal> GetBounceRate(DateTime startDate, DateTime endDate);

        Task<decimal> GetConversionLevel(DateTime startDate, DateTime endDate);

        Task<decimal> GetAov(DateTime startDate, DateTime endDate);
    }
}