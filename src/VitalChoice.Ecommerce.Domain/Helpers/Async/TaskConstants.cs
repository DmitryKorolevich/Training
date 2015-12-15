using System.Threading.Tasks;

namespace VitalChoice.Ecommerce.Domain.Helpers.Async
{
    public static class TaskConstants<T>
    {
        private static Task<T> CanceledTask()
        {
            var tcs = new TaskCompletionSource<T>();
            tcs.SetCanceled();
            return tcs.Task;
        }

        /// <summary>
        /// A task that has been completed with the default value of <typeparamref name="T"/>.
        /// </summary>
        public static Task<T> Default { get; } = Task.FromResult(default(T));

        /// <summary>
        /// A <see cref="Task"/> that will never complete.
        /// </summary>
        public static Task<T> Never { get; } = new TaskCompletionSource<T>().Task;

        /// <summary>
        /// A task that has been canceled.
        /// </summary>
        public static Task<T> Canceled { get; } = CanceledTask();
    }
}