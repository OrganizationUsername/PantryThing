using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Pantry.Core.Models;
using Pantry.Data;

namespace Pantry.ServiceGateways.Equipment
{
    public class EquipmentServiceGateway
    {
        private readonly Func<DataBase> _dbFactory;

        public EquipmentServiceGateway(Func<DataBase> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public List<EquipmentType> GetEquipmentTypes()
        {
            using (var db = _dbFactory())
            {
                return db.EquipmentTypes.ToList();
            }
        }

        public List<Core.Models.Equipment> GetEquipments()
        {
            using (var db = _dbFactory())
            {
                return db.Equipments.Include(x => x.EquipmentType).ToList();
            }
        }

        public void AddEquipment(string newEquipmentName, int newEquipmentTypeId)
        {
            using (var db = _dbFactory())
            {
                db.Equipments.Add(new()
                {
                    EquipmentName = newEquipmentName,
                    LocationId = db.Locations.First().LocationId,
                    EquipmentTypeId = newEquipmentTypeId
                });
                db.SaveChanges();
            }
        }

    }
}