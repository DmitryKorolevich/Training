using System;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Entities.Help;

namespace VitalChoice.Infrastructure.Domain.Transfer.Healthwise
{
    public class HealthwiseDatesManageModel
    {
        public int Id { get; set; }

        public DateTime StartDate { get; set; }
    }
}