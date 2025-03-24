using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class Passenger : User, IRideable, IPayable
    {
        public double WalletBalance { get; private set; }
        public List<Ride> RideHistory { get; private set; }

        public Passenger(string name, string email, string password, string phoneNumber)
            : base(name, email, password, phoneNumber)
        {
            WalletBalance = 900.0; // Starting balance
            RideHistory = new List<Ride>();
        }

        public void RequestRide(Location pickup, Location dropoff)
        {
            var distance = pickup.CalculateDistance(dropoff);
            var fare = CalculateFare(distance);

            if (WalletBalance < fare)
            {
                throw new InsufficientBalanceException($"Insufficient balance. Required: R{fare:F2}, Available: R{WalletBalance:F2}");
            }

            var rideRequest = new RideRequest
            {
                PassengerId = this.Id,
                Pickup = pickup,
                Dropoff = dropoff,
                RequestTime = DateTime.Now,
                Status = RideStatus.Requested,
                EstimatedFare = fare
            };

            RideManager.Instance.AddRideRequest(rideRequest);
            Console.WriteLine($"Ride requested. Estimated fare: R{fare:F2}");
        }

        public double CalculateFare(double distance)
        {
            const double BaseRate = 2.50;
            const double RatePerKm = 1.50;
            return BaseRate + (distance * RatePerKm);
        }

        public void RateDriver(string driverId, int rating)
        {
            if (rating < 1 || rating > 5)
            {
                throw new ArgumentException("Rating must be between 1 and 5");
            }

            // Find the most recent ride with this driver
            var ride = RideHistory
                .Where(r => r.DriverId == driverId && r.Status == RideStatus.Completed)
                .OrderByDescending(r => r.CompletionTime)
                .FirstOrDefault();

            if (ride == null)
            {
                throw new Exception("No completed rides found with this driver");
            }

            ride.DriverRating = rating;

            // Update driver's average rating
            Driver driver = UserManager.Instance.GetDriverById(driverId);
            if (driver != null)
            {
                driver.UpdateRating(rating);
            }

            Console.WriteLine($"You've rated driver {driver?.Name} with {rating} stars.");
        }

        public List<Ride> GetRideHistory()
        {
            return RideHistory;
        }

        public bool ProcessPayment(double amount)
        {
            if (WalletBalance >= amount)
            {
                WalletBalance -= amount;
                return true;
            }
            return false;
        }

        public void AddFunds(double amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Amount must be positive");
            }
            WalletBalance += amount;
            Console.WriteLine($"R{amount:F2} added to your wallet. New balance: ${WalletBalance:F2}");
        }

        public double GetBalance()
        {
            return WalletBalance;
        }
    }
}
