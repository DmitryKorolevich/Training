﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Domain.Entities.Workflow
{
    public class WorkflowResolverPath : Entity
    {
        public int IdResolver { get; set; }

        public int IdExecutor { get; set; }

        public string Name { get; set; }

        public int Path { get; set; }

        public WorkflowExecutor Executor { get; set; }

        public WorkflowExecutor Resolver { get; set; }
    }
}
