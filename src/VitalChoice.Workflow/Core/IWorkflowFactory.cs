using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Workflow.Core
{
    public interface IWorkflowFactory
    {
        Task<TTree> CreateTree<TTree, TContext, TResult>(string name)
            where TContext : WorkflowContext<TResult>
            where TTree : IWorkflowTree<TContext, TResult>;
    }
}