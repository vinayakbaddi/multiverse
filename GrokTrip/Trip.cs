public class Trip
{
    public string VehicleId { get; set; }
    public int StartTime { get; set; }
    public int EndTime { get; set; }
    public double Distance { get; set; }
}

public class VehicleReport
{
    public string VehicleId { get; set; }
    public int TotalTrips { get; set; }
    public double TotalDistance { get; set; }
    public double AverageDistance { get; set; }
}

public class RunTrip
{


    public List<VehicleReport> GenerateReport(List<Trip> trips, double minDistance)
    {
        Console.WriteLine("TE");
        if (trips == null)
        {
            return new List<VehicleReport>();
        }

        var report = trips
            // Step 1: keep only valid trips
            .Where(t => t.EndTime > t.StartTime && t.Distance > 0)
            // Step 2: group by vehicle
            .GroupBy(t => t.VehicleId)
            // Step 3: aggregate per vehicle
            .Select(g =>
            {
                int totalTrips = g.Count();
                double totalDistance = g.Sum(t => t.Distance);
                double avgDistance = totalTrips > 0 ? totalDistance / totalTrips : 0.0;
                return new VehicleReport
                {
                    VehicleId = g.Key,
                    TotalTrips = totalTrips,
                    TotalDistance = totalDistance,
                    AverageDistance = avgDistance
                };
            })
            // Step 4: apply minDistance threshold
            .Where(v => v.TotalDistance >= minDistance)
            // Step 5: sort by total distance desc, then vehicleId asc
            .OrderByDescending(v => v.TotalDistance)
            .ThenBy(v => v.VehicleId, StringComparer.Ordinal)
            .ToList();

        return report;
    }

    public List<VehicleReport> GenerateReport2(List<Trip> trips, double minDistance)
    {
        var filTrips = trips
        .Where(x => x.EndTime > x.StartTime && x.Distance > 0)
        .GroupBy(v => v.VehicleId)
        .Select(g =>
        {
            var trips = g.Count();
            var distance = g.Sum(x => x.Distance);
            var avg = trips > 0 ? distance / trips : 0.0;
            return new VehicleReport()
            {
                TotalDistance = distance,
                VehicleId = g.Key,
                AverageDistance = avg,
                TotalTrips = trips
            };
        })
        .Where(x=> x.TotalDistance > minDistance)
        .OrderByDescending(x=> x.TotalDistance)
        .ThenBy(v=> v.VehicleId, StringComparer.Ordinal)
        .ToList();


        // .Select(g=>new Trip
        // {
        //     VehicleId = g.Key,
        //     trips = g.ToList<Trip>()
        // });

        List<VehicleReport> vehicleReports = new List<VehicleReport>();
        var groupedResult = trips
            .GroupBy(t => t.VehicleId)
            .Where(g => g.Sum(t => t.Distance) >= minDistance)
            .OrderByDescending(g => g.Sum(t => t.Distance));

        foreach (var v in groupedResult)
        {
            Console.WriteLine($"VehicleId: {v.Key}, Total Distance: {v.Sum(t => t.Distance)}");
            Console.WriteLine($" {v.Key} -> Trips {v.Count()} , Total : {v.Sum(t => t.Distance)} Avg : {v.Average(t => t.Distance)}");
            foreach (var trip in v)
            {
            }
        }

        // foreach (var vr in filTrips)
        // {
        //     var veh = filTrips.Where(x => x.VehicleId == vr.VehicleId);
        //     var v=  new VehicleReport()
        //     {
        //         AverageDistance = GetAverage(veh),
        //         TotalDistance = GetDistance(veh),
        //         TotalTrips = GetTotalTrips(veh),
        //         VehicleId = vr.VehicleId
        //     };

        //     Console.WriteLine($" {v.VehicleId} -> Trips {v.TotalTrips} , Total : {v.TotalDistance} Avg : {v.AverageDistance}");
        //     vehicleReports.Add(v);
        // }
        return vehicleReports;

    }

    public double GetDistance(IEnumerable<Trip> trips)
    {
        return trips.Sum(x => x.Distance);
    }
    public int GetTotalTrips(IEnumerable<Trip> trips)
    {
        return trips.Count();
    }

    public double GetAverage(IEnumerable<Trip> trips)
    {
        return trips.Average(x => x.Distance);
    }

    public void Init()
    {
        var trips = new List<Trip>
        {
            new Trip { VehicleId = "V001", StartTime = 100, EndTime = 150, Distance = 45.5 },
            new Trip { VehicleId = "V002", StartTime = 200, EndTime = 180, Distance = 30.0 }, // invalid (end < start)
            new Trip { VehicleId = "V001", StartTime = 300, EndTime = 380, Distance = 60.0 },
            new Trip { VehicleId = "V003", StartTime = 50,  EndTime = 90,  Distance = 25.0 },
            new Trip { VehicleId = "V002", StartTime = 400, EndTime = 480, Distance = 55.5 },
            new Trip { VehicleId = "V003", StartTime = 100, EndTime = 140, Distance = 0.0 },  // invalid
            new Trip { VehicleId = "V001", StartTime = 500, EndTime = 560, Distance = 40.0 }
        };

        double minDistance = 50.0;

        var r = GenerateReport(trips, minDistance);
        foreach (var v in r)
        {
            Console.WriteLine($"VehicleId: {v.VehicleId}, Total Distance: {v.TotalDistance} Average {v.AverageDistance} Total Trips : {v.TotalTrips}");
        }
    }
}