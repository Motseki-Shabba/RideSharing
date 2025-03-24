using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class Ride
    {
        public string Id { get; set; }
        public string PassengerId { get; set; }
        public string DriverId { get; set; }
        public Location Pickup { get; set; }
        public Location Dropoff { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? CompletionTime { get; set; }
        public RideStatus Status { get; set; }
        public double Fare { get; set; }
        public int? DriverRating { get; set; }
    }
}
