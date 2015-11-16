using System.Collections.Generic;

namespace VitalChoice.Ecommerce.Domain.Entities.Workflow
{
    public class WorkflowTree :Entity
    {
        public WorkflowTree()
        {
            Actions = new List<WorkflowTreeAction>();
        }

        public string Name { get; set; }

        public string ImplementationType { get; set; }

        public ICollection<WorkflowTreeAction> Actions { get; set; }
    }
}
