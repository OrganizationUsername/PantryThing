using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Pantry.Core.Models;

namespace Pantry.Core.Test.Inventory_Tests
{
    //Lol, what about actually purchasing food?
    public class InventoryTests
    {
        private readonly Food _slicedBread = new() { FoodId = 9, FoodName = "Sliced Bread" };
        private readonly DateTime _now = DateTime.Now;

        [Test]
        public void CheckFoodSpoilage()
        {
            var fridge = new Location() { LocationName = "Fridge" };
            var x = new LocationFoods()
            {
                Location = fridge,
                Food = _slicedBread,
                OpenDate = _now,
                ExpiryDate = _now + TimeSpan.FromDays(7)
            };
            var y = new LocationFoods()
            {
                Location = fridge,
                Food = _slicedBread,
                OpenDate = _now + TimeSpan.FromDays(-8),
                ExpiryDate = _now + TimeSpan.FromDays(-1)
            };

            Assert.IsFalse(x.IsSpoiledAtTime(_now));
            Assert.IsTrue(y.IsSpoiledAtTime(_now));
        }

        [Test]
        public void GetUnSpoiledFoods()
        {
            var fridge = new Location() { LocationName = "Fridge" };
            var locationFoods = new List<LocationFoods>
            {
                new ()
                {
                    Location = fridge,
                    Food = _slicedBread,
                    PurchaseDate = _now,
                    ExpiryDate = _now + TimeSpan.FromDays(7)
                },
                new()
                {
                    Location = fridge,
                    Food = _slicedBread,
                    PurchaseDate = _now + TimeSpan.FromDays(-8),
                    ExpiryDate = _now + TimeSpan.FromDays(-1)
                }
            };

            Assert.AreEqual(0, locationFoods.Where(x => x.IsOkayAtTime(
                _now + TimeSpan.FromDays(-8))).ToList().Count);
            Assert.AreEqual(1, locationFoods.Where(x => x.IsOkayAtTime(
                _now)).ToList().Count);
            Assert.AreEqual(0, locationFoods.Where(x => x.IsOkayAtTime(
                _now + TimeSpan.FromDays(9))).ToList().Count);
        }
    }
}
