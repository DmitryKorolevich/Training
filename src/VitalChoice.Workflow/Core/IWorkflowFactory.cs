using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Workflow.Core
{
    public interface IWorkflowFactory
    {
        Task<IWorkflowTree<TContext, TResult>> CreateTree<TContext, TResult>(string name)
            where TContext : WorkflowContext<TResult>;
    }
}