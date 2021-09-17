using System;
using System.Linq;
using Pantry.Core.Models;

namespace Pantry.Core.Extensions
{
    public static class SchedulerExtensions
    {
        public static bool IsAvailable(this Equipment e, DateTime start, DateTime end)
        {
            if (!e.EquipmentCommitments.Any())
            {
                return true;
            }
            //if (!e.BookedTimes.Any())
            //{
            //    return true;
            //}

            foreach (var x in e.EquipmentCommitments)
            {
                if (false
                    || (x.StartTime < end && x.EndTime >= start))
                {
                    return false;
                }
            }



            //foreach (var (startTime, endTime, _) in e.BookedTimes)
            //{
            //    if (false
            //        || (startTime < end && endTime >= start))
            //    {
            //        return false;
            //    }
            //}
            return true;
        }
    }
}