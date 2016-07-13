using System;

namespace VitalChoice.Infrastructure.Domain
{
    public static class UniqueDateGenerator
    {
        private static readonly object LockObj = new object();
        private static DateTime _lastDate;

        public static DateTime GetUniqueDate()
        {
            lock (LockObj)
            {
                var now = DateTime.Now;
                if (now == _lastDate)
                {
                    _lastDate = now.AddTicks(1);
                    return _lastDate;
                }
                _lastDate = now;
                return _lastDate;
            }
        }
    }
}