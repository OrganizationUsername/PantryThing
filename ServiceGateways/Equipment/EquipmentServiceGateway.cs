using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<List<EquipmentType>> GetEquipmentTypes()
        {
            using (var db = _dbFactory())
            {
                return await db.EquipmentTypes.ToListAsync();
            }
        }

        public async Task<List<Core.Models.Equipment>> GetEquipments()
        {
            using (var db = _dbFactory())
            {
                return await db.Equipments.Include(x => x.EquipmentType).ToListAsync();
            }
        }

        public async Task<bool> AddEquipmentType(string equipmentTypeName)
        {
            if (string.IsNullOrWhiteSpace(equipmentTypeName)) return false;
            using (var db = _dbFactory())
            {
                var existingIdentical = await db.EquipmentTypes.FirstOrDefaultAsync(x => x.EquipmentTypeName == equipmentTypeName.Trim());
                if (existingIdentical is null)
                {
                    await db.EquipmentTypes.AddAsync(new() { EquipmentTypeName = equipmentTypeName.Trim() });
                    await db.SaveChangesAsync();
                    return true;
                }

                return false;
            }
        }


        public async Task<bool> AddEquipment(string newEquipmentName, int newEquipmentTypeId)
        {
            using (var db = _dbFactory())
            {
                await db.Equipments.AddAsync(new()
                    {
                        EquipmentName = newEquipmentName,
                        EquipmentTypeId = newEquipmentTypeId
                    });
                await db.SaveChangesAsync();
            }
            return true;
        }

        public async Task<List<Core.Models.Equipment>> GetEquipments(int equipmentTypeId)
        {
            using (var db = _dbFactory())
            {
                return await db.Equipments.Where(x => x.EquipmentTypeId == equipmentTypeId).ToListAsync();
            }
        }

    }
}