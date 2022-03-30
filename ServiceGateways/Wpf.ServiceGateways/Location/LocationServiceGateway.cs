using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

        public int AddNewLocation(string newLocationName)
        {
            using (var db = _dbFactory())
            {
                if (db.Locations.Any(x => x.LocationName == newLocationName)) return 0;
                db.Locations.Add(new Core.Models.Location() { LocationName = newLocationName.Trim() });
                int rowChange= db.SaveChanges();
                return rowChange;
            }



        }

        public int? DeleteLocation(int locationId)
        {
            using (var db = _dbFactory())
            {
                var locationToRemove = db.Locations.First(x => x.LocationId == locationId);
                if (locationToRemove.LocationName == "Default") return null;
                var locationFoods = db.LocationFoods.Where(x => x.LocationId == locationId).ToList();
                var defaultLocation = db.Locations.First(x => x.LocationName == "Default");
                foreach (var lf in locationFoods)
                {
                    lf.LocationId = defaultLocation.LocationId;
                }
                int rowCount = db.SaveChanges();
                db.Locations.Remove(locationToRemove);
                db.SaveChanges();
                return rowCount;
            }
        }
    }

}
