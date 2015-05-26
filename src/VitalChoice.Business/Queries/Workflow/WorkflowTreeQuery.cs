using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Data.Helpers;
using VitalChoice.Domain.Entities.Workflow;

namespace VitalChoice.Business.Queries.Workflow
{
    public class WorkflowTreeQuery : QueryObject<WorkflowTree>
    {
        public WorkflowTreeQuery WithName(string name)
        {
            Add(tree => tree.Name == name);
            return this;
        }

        public WorkflowTreeQuery WithType(Type implementationType)
        {
            if (implementationType == null)
                throw new ArgumentNullException(nameof(implementationType));

            Add(tree => tree.ImplementationType == implementationType.FullName);
            return this;
        }
    }
}
