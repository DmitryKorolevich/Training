using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Domain.Entities.Workflow
{
    public class WorkflowTree :Entity
    {
        public string Name { get; set; }

        public string ImplementationType { get; set; }

        public ICollection<WorkflowTreeAction> Actions { get; set; }
    }
}
