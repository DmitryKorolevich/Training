﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Workflow.Core
{
    public interface ITreeSetupCleaner
    {
        Task<bool> CleanAllTrees();
    }
}