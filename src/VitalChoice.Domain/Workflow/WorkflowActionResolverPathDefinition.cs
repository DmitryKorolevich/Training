using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Domain.Workflow
{
    public class WorkflowActionResolverPathDefinition
    {
        public int Path { get; set; }

        public Type Type { get; set; }

        public string Name { get; set; }
    }
}
