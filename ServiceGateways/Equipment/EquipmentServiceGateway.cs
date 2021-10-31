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

        public bool AddEquipmentType(string equipmentTypeName)
        {
            if (string.IsNullOrWhiteSpace(equipmentTypeName)) return false;
            using (var db = _dbFactory())
            {
                var existingIdentical = db.EquipmentTypes.FirstOrDefault(x => x.EquipmentTypeName == equipmentTypeName.Trim());
                if (existingIdentical is null)
                {
                    db.EquipmentTypes.Add(new EquipmentType() { EquipmentTypeName = equipmentTypeName.Trim() });
                    db.SaveChanges();
                    return true;
                }

                return false;
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

        public List<Core.Models.Equipment> GetEquipments(int equipmentTypeId)
        {
            using (var db = _dbFactory())
            {
                return db.Equipments.Where(x => x.EquipmentTypeId == equipmentTypeId).ToList();
            }
        }

    }
}