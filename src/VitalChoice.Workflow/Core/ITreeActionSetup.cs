﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Workflow.Core
{
    public interface ITreeActionSetup<out TContext, in TResult>
        where TContext : WorkflowContext<TResult>
    {
        ITreeActionSetup<TContext, TResult> Action<T>()
            where T : IWorkflowExecutor<TContext, TResult>;
    }
}