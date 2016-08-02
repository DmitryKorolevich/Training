using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VitalChoice.Interfaces.Services
{
    public interface IGoogleService
    {
        void Test();

        decimal GetCartAbandon(DateTime startDate, DateTime endDate);

        long GetUniqueVisits(DateTime startDate, DateTime endDate);

        decimal GetNewWebVisitsPercent(DateTime startDate, DateTime endDate);

        decimal GetTransactionsRevenueOrganics(DateTime startDate, DateTime endDate);

        decimal GetTransactionsRevenuePaid(DateTime startDate, DateTime endDate);

        decimal GetBounceRate(DateTime startDate, DateTime endDate);

        decimal GetConversionLevel(DateTime startDate, DateTime endDate);

        decimal GetAov(DateTime startDate, DateTime endDate);
    }
}