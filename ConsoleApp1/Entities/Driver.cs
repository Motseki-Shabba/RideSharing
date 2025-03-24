using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class Driver : User
    {
        public bool IsAvailable { get; set; }
        public Location CurrentLocation { get; set; }
        public double TotalEarnings { get; private set; }
        public double AverageRating { get; private set; }
        public int TotalRatings { get; private set; }
        public List<Ride> CompletedRides { get; private set; }

        public Driver(string name, string email, string password, string phoneNumber, Location initialLocation)
            : base(name, email, password, phoneNumber)
        {
            CurrentLocation = initialLocation;
            IsAvailable = true;
            TotalEarnings = 0;
            AverageRating = 0;
            TotalRatings = 0;
            CompletedRides = new List<Ride>();
        }

        public List<RideRequest> GetAvailableRideRequests()
        {
            const double MaxDistance = 5.0; // kilometers
            return RideManager.Instance.GetPendingRideRequests()
                .Where(r =>
                {
                    var pickupLocation = r.Pickup;
                    var distance = CurrentLocation.CalculateDistance(pickupLocation);
                    return distance <= MaxDistance;
                })
                .ToList();
        }

        public void AcceptRide(string requestId)
        {
            var request = RideManager.Instance.GetRideRequestById(requestId);

            if (request == null)
            {
                throw new ArgumentException("Invalid ride request ID");
            }

            if (request.Status != RideStatus.Requested)
            {
                throw new Exception("This ride request is no longer available");
            }

            if (!IsAvailable)
            {
                throw new Exception("You are currently unavailable");
            }

            // Check if driver is close enough
            var distance = CurrentLocation.CalculateDistance(request.Pickup);
            if (distance > 5.0) // 5 km max distance
            {
                throw new Exception("You are too far from the pickup location");
            }

            // Create a new ride from the request
            var ride = new Ride
            {
                Id = Guid.NewGuid().ToString(),
                PassengerId = request.PassengerId,
                DriverId = this.Id,
                Pickup = request.Pickup,
                Dropoff = request.Dropoff,
                StartTime = DateTime.Now,
                Status = RideStatus.InProgress,
                Fare = request.EstimatedFare
            };

            // Update request status
            request.Status = RideStatus.Accepted;
            request.DriverId = this.Id;

            // Update driver status
            IsAvailable = false;

            // Add ride to the system
            RideManager.Instance.AddRide(ride);

            Console.WriteLine("Ride accepted. Navigate to pickup location.");
        }

        public void CompleteRide(string rideId)
        {
            var ride = RideManager.Instance.GetRideById(rideId);

            if (ride == null || ride.DriverId != this.Id)
            {
                throw new ArgumentException("Invalid ride ID");
            }

            if (ride.Status != RideStatus.InProgress)
            {
                throw new Exception("This ride is not in progress");
            }

            // Update ride status
            ride.Status = RideStatus.Completed;
            ride.CompletionTime = DateTime.Now;

            // Process payment
            var passenger = UserManager.Instance.GetPassengerById(ride.PassengerId);
            if (passenger == null)
            {
                throw new Exception("Passenger not found");
            }

            bool paymentSuccessful = passenger.ProcessPayment(ride.Fare);
            if (!paymentSuccessful)
            {
                throw new InsufficientBalanceException("Payment failed: Insufficient balance");
            }

            // Update driver
            TotalEarnings += ride.Fare;
            IsAvailable = true;
            CurrentLocation = ride.Dropoff; // Update driver location to dropoff

            // Add ride to passenger's history
            passenger.GetRideHistory().Add(ride);
            CompletedRides.Add(ride);

            Console.WriteLine($"Ride completed. Earned: R{ride.Fare:F2}");
        }

        public void UpdateRating(int newRating)
        {
            TotalRatings++;
            // Update average rating using the formula: newAvg = oldAvg + (newValue - oldAvg) / newCount
            AverageRating = AverageRating + (newRating - AverageRating) / TotalRatings;
        }

        public void UpdateLocation(Location newLocation)
        {
            CurrentLocation = newLocation;
        }
    }
}
