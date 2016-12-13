using System;

namespace VitalChoice.Ecommerce.Utils
{
    public static class ExceptionFilter
    {
        public static bool SkipFilter(Action work)
        {
            work();
            return false;
        }

        public static bool SkipFilter<T>(T item, Action<T> work)
        {
            work(item);
            return false;
        }

        public static bool SkipFilter<T1, T2>(T1 item1, T2 item2, Action<T1, T2> work)
        {
            work(item1, item2);
            return false;
        }

        public static bool SkipFilter<T1, T2, T3>(T1 item1, T2 item2, T3 item3, Action<T1, T2, T3> work)
        {
            work(item1, item2, item3);
            return false;
        }
    }
}