using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Ecommerce.Utils
{
    public static class TaskExtensions
    {
        public static void RunSyncWait(this Task task)
        {
            task.RunSynchronously();
            task.Wait();
        }

        public static T RunSyncWait<T>(this Task<T> task)
        {
            task.RunSynchronously();
            task.Wait();
            return task.Result;
        }

        public static T GetResult<T>(this Task<T> task) => task.GetAwaiter().GetResult();

        public static void WaitResult(this Task task) => task.GetAwaiter().GetResult();
    }
}