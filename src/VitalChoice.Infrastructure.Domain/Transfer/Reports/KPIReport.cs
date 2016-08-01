using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VitalChoice.Infrastructure.Domain.Transfer.Reports
{
    public class KPIReport
    {
        public KPIReport()
        {
            TodayYearSales = new KPIReportSalesItem();
            MonthAgoYearSales = new KPIReportSalesItem();
            YearAgoYearSales = new KPIReportSalesItem();
            TodayMonthMarketing = new KPIReportMarketingItem();
            MonthAgoMonthMarketing = new KPIReportMarketingItem();
            YearAgoMonthMarketing = new KPIReportMarketingItem();
            MonthRates = new KPIReportRatesItem();
            TwoMonthRates = new KPIReportRatesItem();
            YearRates = new KPIReportRatesItem();
        }

        public DateTime Date { get; set; }

        public KPIReportSalesItem TodayYearSales { get; set; }

        public KPIReportSalesItem MonthAgoYearSales { get; set; }

        public KPIReportSalesItem YearAgoYearSales { get; set; }

        public KPIReportMarketingItem TodayMonthMarketing { get; set; }

        public KPIReportMarketingItem MonthAgoMonthMarketing { get; set; }

        public KPIReportMarketingItem YearAgoMonthMarketing { get; set; }

        public KPIReportRatesItem MonthRates { get; set; }

        public KPIReportRatesItem TwoMonthRates { get; set; }

        public KPIReportRatesItem YearRates { get; set; }
    }
}