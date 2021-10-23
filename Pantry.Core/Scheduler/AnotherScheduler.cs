using System;
using System.Collections.Generic;
using System.Linq;
using Pantry.Core.Extensions;
using Pantry.Core.Models;

namespace Pantry.Core.Scheduler
{
    public class AnotherScheduler
    {
        private readonly IEnumerable<RecipeDag> _manyDags;
        private readonly List<Equipment> _equipments;
        private readonly DateTime _goal;
        public AnotherScheduler(DateTime goal, IEnumerable<RecipeDag> manyDags, List<Equipment> equipments)
        {
            _goal = goal;
            _manyDags = manyDags;
            _equipments = equipments;
        }
        public static IEnumerable<RecipeDag> DecomposeDags(RecipeDag dag)
        {
            if (dag.SubordinateBetterRecipes is null || dag.SubordinateBetterRecipes.Count == 0) { return new List<RecipeDag>() { dag }; }

            return dag.SubordinateBetterRecipes.SelectMany(DecomposeDags).Select(x => new RecipeDag()
            {
                MainRecipe = dag.MainRecipe,
                SubordinateBetterRecipes = new List<RecipeDag>() { x }
            }).ToList();
        }

        public static double? GetDagTime(RecipeDag dag)
        {
            if (dag is null) { return null; }
            var thisGuysCost = dag.MainRecipe.RecipeSteps.Sum(x => x.TimeCost);
            if (dag.SubordinateBetterRecipes is null || dag.SubordinateBetterRecipes.Count == 0)
            {
                return thisGuysCost;
            }
            return thisGuysCost + dag.SubordinateBetterRecipes.Max(GetDagTime);
        }

        public static RecipeDag GetLongestUnresolvedDag(IEnumerable<RecipeDag> dags)
        {
            return dags.Where(x => !x.Scheduled).OrderBy(x => GetDagTime(x) ?? -1).Last();
        }

        public void TrySchedule()
        {
            var scheduledTasks = _manyDags.SelectMany(DecomposeDags).OrderBy(GetDagTime);
            foreach (var scheduledTask in _manyDags)
            {
                Schedule(scheduledTask);
            }
            Console.WriteLine("-----");
            Console.WriteLine("Equipments:");
            foreach (var equipment in _equipments)
            {
                if (equipment.EquipmentCommitments is null || !equipment.EquipmentCommitments.Any()) { Console.WriteLine(equipment.EquipmentName + "- Nothing."); }
                else { Console.WriteLine($"{equipment.EquipmentName}"); }
                foreach (var x in equipment.EquipmentCommitments.OrderBy(z => z.StartTime))
                {
                    Console.WriteLine($"{x.StartTime.ToShortTimeString()}:{x.EndTime.ToShortTimeString()}: {x.Description} {x.RecipeStep.Instruction}");
                }
            }
        }

        public void Schedule(RecipeDag dag)
        {
            var offset = 0;
            for (var index = dag.MainRecipe.RecipeSteps.Count - 1; index >= 0; index--)
            {
                var recipeStep = dag.MainRecipe.RecipeSteps[index];
                var satisfied = false;
                for (; !satisfied; offset++)
                { //ToDo: Make this work with EquipmentType
                    if (recipeStep.RecipeStepEquipmentType.Select(x => x.Equipment).All(y =>
                        y.IsAvailable(_goal.AddMinutes(-(offset + recipeStep.TimeCost)), _goal.AddMinutes(-offset))))
                    {
                        satisfied = true;
                        foreach (var y in recipeStep.RecipeStepEquipmentType.Select(x => x.Equipment))
                        {
                            y.EquipmentCommitments.Add(new EquipmentCommitment()
                            {
                                StartTime = _goal.AddMinutes(-(offset + recipeStep.TimeCost)),
                                EndTime = _goal.AddMinutes(-offset),
                                Description = $"_ {dag.MainRecipe.RecipeFoods.First(x => x.Amount < 0).Food.FoodName}",
                                RecipeStep = recipeStep
                            });
                        }
                    }
                }
            }

            if (dag.SubordinateBetterRecipes is null || dag.SubordinateBetterRecipes.Count <= 0) return;
            foreach (var recipeStep in dag.SubordinateBetterRecipes)
            {
                Schedule(recipeStep);
            }
        }
    }
}