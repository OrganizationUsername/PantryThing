using System;
using System.Collections.Generic;
using System.Linq;

namespace Pantry.Core.Scheduler
{
    public class SimpleScheduler : IScheduler
    {
        public SimpleScheduler(List<GetCookPlan> scheduledTasks, List<Equipment> equipments)
        {
            this.ScheduledTasks = scheduledTasks;
            this.Equipments = equipments;
        }

        public List<GetCookPlan> ScheduledTasks { get; set; } = new();
        public List<Equipment> Equipments { get; set; } = new();
        public void TrySchedule(DateTime goal)
        {
            foreach (var scheduledTask in ScheduledTasks)
            {
                var recipeSteps = scheduledTask.RecipesTouched.SelectMany(b => b.RecipeSteps).Reverse();
                int offset = 0;
                foreach (var recipeStep in recipeSteps)
                {
                    bool satisfied = false;
                    for (; !satisfied; offset++)
                    {
                        if (recipeStep.Equipments.All(y =>
                            y.IsAvailable(goal.AddMinutes(-(offset + recipeStep.TimeCost)), goal.AddMinutes(-offset))))
                        {
                            satisfied = true;
                            foreach (var y in recipeStep.Equipments)
                            {
                                y.BookedTimes.Add(
                                    (goal.AddMinutes(-(offset + recipeStep.TimeCost)),
                                        goal.AddMinutes(-offset), recipeStep.Instruction + $"_ {scheduledTask.RecipeName}")
                                );
                            }
                        }
                    }
                }
            }

            foreach (var equipment in Equipments)
            {
                Console.WriteLine(equipment.Name);
                foreach (var y in equipment.BookedTimes.OrderBy(z => z.startTime))
                {
                    Console.WriteLine($"{y.startTime.ToShortTimeString()}:{y.endTime.ToShortTimeString()}: {y.StepName}");
                }
            }
        }
    }
}