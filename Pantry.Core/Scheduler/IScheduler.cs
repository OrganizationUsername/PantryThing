using System;
using System.Collections.Generic;
using Pantry.Core.Models;

namespace Pantry.Core.Scheduler
{
    public interface IScheduler
    {
        void TrySchedule(DateTime goal);
        List<CookPlan> ScheduledTasks { get; set; }
        List<Equipment> Equipments { get; set; }
    }
}