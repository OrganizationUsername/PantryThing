using System;
using System.Collections.Generic;
// ReSharper disable UnusedMember.Global

namespace Pantry.Core.Models
{
    public class Equipment
    {
        // ReSharper disable once UnusedMember.Global
        public int EquipmentId { get; set; }
        public string EquipmentName { get; set; }
        public int EquipmentTypeId { get; set; }
        public EquipmentType EquipmentType { get; set; }
        public ICollection<EquipmentCommitment> EquipmentCommitments { get; set; }
        public ICollection<RecipeStepEquipmentType> RecipeStepEquipment { get; set; }
    }

    public class Consumer
    {
        public int ConsumerId { get; set; }
        public string ConsumerName { get; set; }
        public ICollection<MealInstance> MealInstances { get; set; }
    }

    public class MealInstance
    {
        public int MealInstanceId { get; set; }
        public int ConsumerId { get; set; }
        public Consumer Consumer { get; set; }
        public DateTime MealInstanceDateTime { get; set; }
        public int DaySinceMillennium { get; set; } //For db.
        public int MealOfTheDayId { get; set; }
        public MealOfTheDay MealOfTheDay { get; set; }
        public ICollection<MealInstanceRow> MealInstanceRows { get; set; }
    }

    public class MealInstanceRow //A meal may consist of multiple foods, which we may not want to store?
    {
        public int MealInstanceRowId { get; set; }
        public int FoodId { get; set; }
        public Food Food { get; set; }
        public double Quantity { get; set; }
        public int MealInstanceId { get; set; }
        public MealInstance MealInstance { get; set; }
    }

    public class MealOfTheDay //Breakfast, lunch, dinner, snacks, ad hoc
    {
        public int MealOfTheDayId { get; set; }
        public string MealOfTheDayName { get; set; }
        public DateTime MealOfTheDayDateTime { get; set; }
        public ICollection<MealInstance> MealInstances { get; set; }
        public ICollection<Consumer> Consumers { get; set; }
    }

    public class PlannedCook //Every time a recipe will be finished?
    {
        public int PlannedCookId { get; set; }
        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; }
        public double RecipeMultiplier { get; set; }
        public DateTime EndTime { get; set; }
        public ICollection<PlannedCookStep> PlannedCookSteps { get; set; }
        public ICollection<ItemReservation> ItemReservations { get; set; }
    }

    public class ItemReservation
    {
        //Inventory shows current, actual. This will help for future planning.
        public int ItemReservationId { get; set; }
        public int ItemId { get; set; }
        public Item Item { get; set; }
        public double Quantity { get; set; }
        public int PlannedCookId { get; set; }
        public PlannedCook PlannedCook { get; set; }
    }

    public class PlannedCookStep
    {
        public int PlannedCookStepId { get; set; }
        public int PlannedCookId { get; set; }
        public PlannedCook PlannedCook { get; set; }
        public DateTime StartTime { get; set; }
    }


}