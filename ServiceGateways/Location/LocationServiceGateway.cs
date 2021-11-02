using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pantry.Data;
using Serilog.Core;

namespace Pantry.ServiceGateways.Location
{

    public class LocationServiceGateway
    {
        private readonly Func<DataBase> _dbFactory;
        private readonly Logger _logger;

        public LocationServiceGateway(Func<DataBase> dbFactory, Logger logger)
        {
            _dbFactory = dbFactory;
            _logger = logger;
        }

        public List<Core.Models.Location> GetLocations()
        {
            var sw = new Stopwatch();
            sw.Start();
            using (var db = _dbFactory())
            {
                var result = db.Locations.ToList();
                _logger.Verbose(
                    $"{sw.ElapsedMilliseconds} ms to get {result.Count} locations.");
                return result;
            }
        }

        public async Task<int> AddNewLocation(string newLocationName)
        {
            using (var db = _dbFactory())
            {
                if (await db.Locations.AnyAsync(x => x.LocationName == newLocationName)) return 0;
                await db.Locations.AddAsync(new Core.Models.Location() { LocationName = newLocationName.Trim() });
                int rowChange = await db.SaveChangesAsync();
                return rowChange;
            }



        }

        public async Task<int?> DeleteLocation(int locationId)
        {
            using (var db = _dbFactory())
            {
                var locationToRemove = await db.Locations.FirstAsync(x => x.LocationId == locationId);
                if (locationToRemove.LocationName == "Default") return null;
                var locationFoods = await db.LocationFoods.Where(x => x.LocationId == locationId).ToListAsync();
                var defaultLocation = await db.Locations.FirstAsync(x => x.LocationName == "Default");
                foreach (var lf in locationFoods)
                {
                    lf.LocationId = defaultLocation.LocationId;
                }
                int rowCount = await db.SaveChangesAsync();

                // something something this changes state and the remove happens on the savechanges
                db.Locations.Remove(locationToRemove);

                await db.SaveChangesAsync();
                return rowCount;
            }
        }
    }

}
