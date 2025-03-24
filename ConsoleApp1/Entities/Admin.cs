using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class Admin : User
    {
        public Admin(string name, string email, string password, string phoneNumber)
            : base(name, email, password, phoneNumber)
        {
        }

        public List<Report> GenerateReports()
        {
            var reports = new List<Report>();

            // Total rides report
            var totalRides = RideManager.Instance.GetAllRides().Count;
            var completedRides = RideManager.Instance.GetAllRides().Count(r => r.Status == RideStatus.Completed);
            reports.Add(new Report("Ride Statistics", $"Total Rides: {totalRides}, Completed Rides: {completedRides}"));

            // Total earnings report
            var totalEarnings = RideManager.Instance.GetAllRides()
                .Where(r => r.Status == RideStatus.Completed)
                .Sum(r => r.Fare);
            reports.Add(new Report("Financial Report", $"Total Platform Earnings: R{totalEarnings:F2}"));

            // Average ratings report
            var drivers = UserManager.Instance.GetAllDrivers();
            var avgRating = drivers.Any()
                ? drivers.Average(d => d.AverageRating)
                : 0;
            reports.Add(new Report("Driver Performance", $"Average Driver Rating: {avgRating:F1}/5.0"));

            return reports;
        }

        public List<Driver> GetLowRatedDrivers(double threshold = 3.0)
        {
            return UserManager.Instance.GetAllDrivers()
                .Where(d => d.AverageRating < threshold && d.TotalRatings >= 5)
                .ToList();
        }
    }
}
