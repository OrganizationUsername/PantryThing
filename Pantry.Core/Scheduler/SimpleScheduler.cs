using System;
using System.Collections.Generic;
using System.Linq;
using Pantry.Core.Extensions;
using Pantry.Core.Models;

namespace Pantry.Core.Scheduler
{

    public class NaiveScheduler : IScheduler
    {
        public List<CookPlan> ScheduledTasks { get; set; }
        public List<Equipment> Equipments { get; set; }

        public NaiveScheduler(List<CookPlan> scheduledTasks, List<Equipment> equipments)
        {
            this.ScheduledTasks = scheduledTasks;
            this.Equipments = equipments;
        }

        public void TrySchedule(DateTime goal)
        {
            int offset = 0;
            foreach (var scheduledTask in ScheduledTasks)
            {
                var recipeSteps = scheduledTask.RecipesTouched.SelectMany(b => b.RecipeSteps).Reverse();
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
                                y.EquipmentCommitments.Add(new EquipmentCommitment()
                                {
                                    StartTime = goal.AddMinutes(-(offset + recipeStep.TimeCost)),
                                    EndTime = goal.AddMinutes(-offset),
                                    Description = recipeStep.Instruction + $"_ {scheduledTask.RecipeName}"
                                });
                                //y.BookedTimes.Add(
                                //    (goal.AddMinutes(-(offset + recipeStep.TimeCost)),
                                //        goal.AddMinutes(-offset), recipeStep.Instruction + $"_ {scheduledTask.RecipeName}")
                                //);
                            }
                        }
                    }
                }
            }
            //Bread Machine
            //5:18 PM: 5:19 PM: Fill / operate bread machine._ Cheese sandwich
            //5:19 PM: 5:59 PM: Do its thing._ Cheese sandwich
            //Human
            //4:36 PM: 4:37 PM: Fill / operate rice machine._ CurriedRice
            //5:13 PM: 5:18 PM: Heat and plate._ CurriedRice
            //5:18 PM: 5:19 PM: Fill / operate bread machine._ Cheese sandwich
            //5:57 PM: 6:00 PM: Assemble Sandwich._ Cheese sandwich
            //Rice Machine
            //4:36 PM: 4:37 PM: Fill / operate rice machine._ CurriedRice
            //4:37 PM: 5:17 PM: Do its thing._ CurriedRice
            //StoveTop
            //5:13 PM: 5:18 PM: Heat and plate._ CurriedRice

            foreach (var equipment in Equipments)
            {
                Console.WriteLine(equipment.EquipmentName);
                foreach (var x in equipment.EquipmentCommitments.OrderBy(z => z.StartTime))
                {
                    Console.WriteLine($"{x.StartTime.ToShortTimeString()}:{x.EndTime.ToShortTimeString()}: {x.Description} {x.RecipeStep.Instruction}");
                }
            }
        }
    }


    public class SimpleScheduler : IScheduler
    {
        public SimpleScheduler(List<CookPlan> scheduledTasks, List<Equipment> equipments)
        {
            this.ScheduledTasks = scheduledTasks;
            this.Equipments = equipments;
        }

        public List<CookPlan> ScheduledTasks { get; set; } = new();
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
                                y.EquipmentCommitments.Add(new EquipmentCommitment()
                                {
                                    StartTime = goal.AddMinutes(-(offset + recipeStep.TimeCost)),
                                    EndTime = goal.AddMinutes(-offset),
                                    Description = recipeStep.Instruction + $"_ {scheduledTask.RecipeName}"
                                });
                                //y.BookedTimes.Add(
                                //    (goal.AddMinutes(-(offset + recipeStep.TimeCost)),
                                //        goal.AddMinutes(-offset), recipeStep.Instruction + $"_ {scheduledTask.RecipeName}")
                                //);
                            }
                        }
                    }
                }
            }
            //Bread Machine
            //5:18 PM: 5:19 PM: Fill / operate bread machine._ Cheese sandwich
            //5:19 PM: 5:59 PM: Do its thing._ Cheese sandwich
            //Human
            //5:15 PM: 5:16 PM: Fill / operate rice machine._ CurriedRice
            //5:18 PM: 5:19 PM: Fill / operate bread machine._ Cheese sandwich
            //5:52 PM: 5:57 PM: Heat and plate._ CurriedRice
            //5:57 PM: 6:00 PM: Assemble Sandwich._ Cheese sandwich
            //Rice Machine
            //5:15 PM: 5:16 PM: Fill / operate rice machine._ CurriedRice
            //5:16 PM: 5:56 PM: Do its thing._ CurriedRice
            //StoveTop
            //5:52 PM: 5:57 PM: Heat and plate._ CurriedRice

            foreach (var equipment in Equipments)
            {
                Console.WriteLine(equipment.EquipmentName);
                foreach (var x in equipment.EquipmentCommitments.OrderBy(z => z.StartTime))
                {
                    Console.WriteLine($"{x.StartTime.ToShortTimeString()}:{x.EndTime.ToShortTimeString()}: {x.Description}");
                }
            }
        }
    }
}