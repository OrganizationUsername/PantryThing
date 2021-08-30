using System;
using System.Collections.Generic;

namespace Pantry.Core.Scheduler
{
    public interface IScheduler
    {
        void TrySchedule(DateTime Goal);
        List<CookPlan> ScheduledTasks { get; set; }
        List<Equipment> Equipments { get; set; }
    }


}