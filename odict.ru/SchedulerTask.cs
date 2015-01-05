using System;
using System.Web;
using System.Web.Caching;

namespace odict.ru
{
    public class SchedulerTask
    {
        private readonly Action action;
        private readonly TimeSpan period;
        private readonly int plusMinusMs;
        private readonly string key = Guid.NewGuid ().ToString ();
        private readonly Random random = new Random ((int) DateTime.Now.Ticks);

        public SchedulerTask (Action action, TimeSpan period, TimeSpan plusMinus)
        {
            this.action = action;
            this.period = period;

            if (plusMinus > period) throw new Exception ("'plusMinus' cannot be greater than 'period'.");

            checked {this.plusMinusMs = (int) plusMinus.TotalMilliseconds;}

            ScheduleNextTick ();
        }

        private void ScheduleNextTick ()
        {
            TimeSpan nextPeriod = period + TimeSpan.FromMilliseconds (random.Next (-plusMinusMs, plusMinusMs));

            HttpRuntime.Cache.Insert (key, new object (), null,
                DateTime.Now + nextPeriod, Cache.NoSlidingExpiration,
                CacheItemPriority.NotRemovable, CacheItemRemoved);
        }

        public void CacheItemRemoved (string k, object v, CacheItemRemovedReason r)
        {
            try
            {
                action ();
            }
            finally
            {
                // re-add our task so it recurs
                ScheduleNextTick ();
            }
        }
    }
}