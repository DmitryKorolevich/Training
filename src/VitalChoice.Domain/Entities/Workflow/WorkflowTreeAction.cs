using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Domain.Entities.Workflow
{
    public class WorkflowTreeAction :Entity
    {
        public int IdTree { get; set; }

        public int IdExecutor { get; set; }

        public WorkflowExecutor Executor { get; set; }

        public WorkflowTree Tree { get; set; }
    }
}
