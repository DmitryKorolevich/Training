using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Entities.Healthwise;

namespace VC.Admin.Models.Healthwise
{
    public class HealthwiseCustomerListItemModel
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public ICollection<HealthwisePeriodListItemModel> Periods { get; set; }

        public HealthwiseCustomerListItemModel(ICollection<VHealthwisePeriod> periods)
        {
            Periods = new List<HealthwisePeriodListItemModel>();
            if (periods!=null && periods.Count>0)
            {
                Id = periods.First().IdCustomer;
                FirstName = periods.First().CustomerFirstName;
                LastName = periods.First().CustomerLastName;
                foreach (var period in periods)
                {
                    Periods.Add(new HealthwisePeriodListItemModel(period));
                }
                Periods = Periods.OrderByDescending(p => p.StartDate).ToList();
            }
        }
    }
}
