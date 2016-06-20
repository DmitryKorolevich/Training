using System.Collections.Generic;

namespace VitalChoice.Ecommerce.Domain.Entities.Workflow
{
    public class WorkflowTree :Entity
    {
        public WorkflowTree()
        {
            Actions = new List<WorkflowExecutor>();
        }

        public string Name { get; set; }

        public string ImplementationType { get; set; }

        public ICollection<WorkflowExecutor> Actions { get; set; }
    }
}
