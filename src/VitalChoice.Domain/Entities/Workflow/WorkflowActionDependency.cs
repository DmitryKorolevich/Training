using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Domain.Entities.Workflow
{
    public class WorkflowActionDependency : Entity
    {
        public int IdParent { get; set; }

        public WorkflowExecutor Parent { get; set; }

        public int IdDependent { get; set; }

        public WorkflowExecutor Dependent { get; set; }
    }
}
