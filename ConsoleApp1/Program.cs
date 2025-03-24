using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public static class Program
    {
        private static User _currentUser;

        static void Main(string[] args)
        {
            InitializeTestData();

            bool exitApplication = false;
            while (!exitApplication)
            {
                if (_currentUser == null)
                {
                    exitApplication = ShowMainMenu();
                }
                else if (_currentUser is Passenger)
                {
                    ShowPassengerMenu();
                }
                else if (_currentUser is Driver)
                {
                    ShowDriverMenu();
                }

            }
        }

        private static void InitializeTestData()
        {
            try
            {
                // Add some test passengers
                UserManager.Instance.RegisterPassenger(new Passenger("Kabelo", "Kabelo@gmail.com", "12345", "1234567890"));
                UserManager.Instance.RegisterPassenger(new Passenger("Tshepo", "Tshepo@gmail.com", "12345", "0987654321"));

                // Add some test drivers with random locations
                var random = new Random();
                var driverLocations = new List<Location>
                {
                    new Location(40.7128, -74.0060),
                    new Location(34.0522, -118.2437),
                    new Location(41.8781, -87.6298),
                    new Location(29.7604, -95.3698),
                    new Location(39.9526, -75.1652)
                };

                foreach (var location in driverLocations)
                {
                    var driverName = $"Driver{driverLocations.IndexOf(location) + 1}";
                    UserManager.Instance.RegisterDriver(new Driver(
                        driverName,
                        $"{driverName.ToLower()}@gmail.com",
                        "pass123",
                        $"555{random.Next(1000000, 9999999)}",
                        location
                    ));
                }

                Console.WriteLine("Test data successfully.");
                Console.WriteLine("You can login with:");
                Console.WriteLine("# Passenger: Kabelo@gmail.com, password=12345");
                Console.WriteLine("# Driver: email=Tshepo@gmail.com, password=12345");
                Console.WriteLine("# Admin: email=admin@system.com, password=admin123");
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error test data: {ex.Message}");
            }
        }

        private static bool ShowMainMenu()
        {
            Console.Clear();
            Console.WriteLine("*************** RIDE SHARING SYSTEM ************************");
            Console.WriteLine("1. Register as Passenger");
            Console.WriteLine("2. Register as Driver");
            Console.WriteLine("3. Login");
            Console.WriteLine("4. Exit");
            Console.WriteLine("*************** RIDE SHARING SYSTEM ************************");
            Console.Write("Choose an option: ");

            string choice = Console.ReadLine();


            switch (choice)
            {
                case "1":
                    RegisterPassenger();
                    return false;
                case "2":
                    RegisterDriver();
                    return false;
                case "3":
                    Login();
                    return false;
                case "4":
                    Console.WriteLine("Thank you for using our Ride Sharing System!");
                    return true;
                default:
                    Console.WriteLine("Invalid option. Press any key to continue...");
                    Console.ReadKey();
                    return false;
            }
        }

        private static void RegisterPassenger()
        {
            Console.Clear();
            Console.WriteLine("********************** PASSENGER REGISTRATION ********************************");

            try
            {
                Console.Write("Name: ");
                string name = Console.ReadLine();

                Console.Write("Email: ");
                string email = Console.ReadLine();

                Console.Write("Password: ");
                string password = Console.ReadLine();

                Console.Write("Phone Number: ");
                string phoneNumber = Console.ReadLine();

                var passenger = new Passenger(name, email, password, phoneNumber);
                UserManager.Instance.RegisterPassenger(passenger);

                Console.WriteLine("Registration successful! Press any key to continue...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Registration failed: {ex.Message}");
                Console.WriteLine("********************** PASSENGER REGISTRATION ********************************");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        private static void RegisterDriver()
        {
            Console.Clear();
            Console.WriteLine("************************ DRIVER REGISTRATION ******************************");

            try
            {
                Console.Write("Name: ");
                string name = Console.ReadLine();

                Console.Write("Email: ");
                string email = Console.ReadLine();

                Console.Write("Password: ");
                string password = Console.ReadLine();

                Console.Write("Phone Number: ");
                string phoneNumber = Console.ReadLine();

                Console.WriteLine("Enter your current location:");
                Console.Write("Latitude: ");
                if (!double.TryParse(Console.ReadLine(), out double latitude))
                {
                    throw new ArgumentException("Invalid latitude");
                }

                Console.Write("Longitude: ");
                if (!double.TryParse(Console.ReadLine(), out double longitude))
                {
                    throw new ArgumentException("Invalid longitude");
                }

                var location = new Location(latitude, longitude);
                var driver = new Driver(name, email, password, phoneNumber, location);
                UserManager.Instance.RegisterDriver(driver);

                Console.WriteLine("Registration successful! Press any key to continue...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Registration failed: {ex.Message}");
                Console.WriteLine("************************ DRIVER REGISTRATION ******************************");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        private static void Login()
        {
            Console.Clear();
            Console.WriteLine("******************************* LOGIN *********************************************");

            try
            {
                Console.Write("Email: ");
                string email = Console.ReadLine();

                Console.Write("Password: ");
                string password = Console.ReadLine();

                _currentUser = UserManager.Instance.AuthenticateUser(email, password);

                if (_currentUser == null)
                {
                    Console.WriteLine("Invalid credentials. Press any key to continue...");
                    Console.ReadKey();
                }
                else
                {
                    Console.WriteLine($"Welcome, {_currentUser.Name}!");
                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login failed: {ex.Message}");
                Console.WriteLine("******************************* LOGIN *********************************************");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        private static void ShowPassengerMenu()
        {
            var passenger = (Passenger)_currentUser;

            Console.Clear();
            Console.WriteLine($"************************ PASSENGER MENU - {passenger.Name} **********************");
            Console.WriteLine($"Wallet Balance: R{passenger.GetBalance():F2}");
            Console.WriteLine("1. Request a Ride");
            Console.WriteLine("2. View Ride History");
            Console.WriteLine("3. Add Funds to Wallet");
            Console.WriteLine("4. Rate a Driver");
            Console.WriteLine("5. Logout");
            Console.Write("Choose an option: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    RequestRide(passenger);
                    break;
                case "2":
                    ViewRideHistory(passenger);
                    break;
                case "3":
                    AddFunds(passenger);
                    break;
                case "4":
                    RateDriver(passenger);
                    break;
                case "5":
                    _currentUser = null;
                    Console.WriteLine("Logged out successfully.");
                    Thread.Sleep(1000);
                    break;
                default:
                    Console.WriteLine("Invalid option. Press any key to continue...");
                    Console.ReadKey();
                    break;
            }
        }

        private static void RequestRide(Passenger passenger)
        {
            Console.Clear();
            Console.WriteLine("************************* REQUEST A RIDE *************************************");

            try
            {
                Console.WriteLine("Enter pickup location:");
                Console.Write("Latitude: ");
                if (!double.TryParse(Console.ReadLine(), out double pickupLat))
                {
                    throw new ArgumentException("Invalid latitude");
                }

                Console.Write("Longitude: ");
                if (!double.TryParse(Console.ReadLine(), out double pickupLon))
                {
                    throw new ArgumentException("Invalid longitude");
                }

                Console.WriteLine("Enter dropoff location:");
                Console.Write("Latitude: ");
                if (!double.TryParse(Console.ReadLine(), out double dropoffLat))
                {
                    throw new ArgumentException("Invalid latitude");
                }

                Console.Write("Longitude: ");
                if (!double.TryParse(Console.ReadLine(), out double dropoffLon))
                {
                    throw new ArgumentException("Invalid longitude");
                }

                var pickup = new Location(pickupLat, pickupLon);
                var dropoff = new Location(dropoffLat, dropoffLon);

                passenger.RequestRide(pickup, dropoff);

                Console.WriteLine("Ride requested successfully! Waiting for driver acceptance.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        private static void ViewRideHistory(Passenger passenger)
        {
            Console.Clear();
            Console.WriteLine("************************* RIDE HISTORY ****************************");

            var rideHistory = passenger.GetRideHistory();

            if (rideHistory.Count == 0)
            {
                Console.WriteLine("No ride history found.");
            }
            else
            {
                foreach (var ride in rideHistory)
                {
                    string status = ride.Status.ToString();
                    string driverName = UserManager.Instance.GetDriverById(ride.DriverId)?.Name ?? "Unknown";
                    string completionTime = ride.CompletionTime.HasValue ? ride.CompletionTime.Value.ToString("g") : "N/A";
                    string rating = ride.DriverRating.HasValue ? $"{ride.DriverRating}/5" : "Not rated";

                    Console.WriteLine($"Ride ID: {ride.Id}");
                    Console.WriteLine($"Driver: {driverName}");
                    Console.WriteLine($"From: {ride.Pickup} To: {ride.Dropoff}");
                    Console.WriteLine($"Start Time: {ride.StartTime:g}");
                    Console.WriteLine($"Completion Time: {completionTime}");
                    Console.WriteLine($"Status: {status}");
                    Console.WriteLine($"Fare: R{ride.Fare:F2}");
                    Console.WriteLine($"Rating: {rating}");
                    Console.WriteLine("------------------------");
                }
            }

            Console.WriteLine("Press any key to return to the menu...");
            Console.ReadKey();
        }

        private static void AddFunds(Passenger passenger)
        {
            Console.Clear();
            Console.WriteLine("********************* ADD FUNDS TO WALLET *******************************");
            Console.WriteLine($"Current Balance: R{passenger.GetBalance():F2}");

            try
            {
                Console.Write("Enter amount to add: R");
                if (!double.TryParse(Console.ReadLine(), out double amount))
                {
                    throw new ArgumentException("Invalid amount");
                }



                passenger.AddFunds(amount);
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        private static void RateDriver(Passenger passenger)
        {
            Console.Clear();
            Console.WriteLine("********************** RATE A DRIVER ***************************");

            try
            {
                var completedRides = passenger.GetRideHistory()
                    .Where(r => r.Status == RideStatus.Completed && !r.DriverRating.HasValue)
                    .ToList();

                if (completedRides.Count == 0)
                {
                    Console.WriteLine("No completed rides available for rating.");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    return;
                }

                Console.WriteLine("Select a ride to rate:");
                for (int i = 0; i < completedRides.Count; i++)
                {
                    var ride = completedRides[i];
                    string driverName = UserManager.Instance.GetDriverById(ride.DriverId)?.Name ?? "Unknown";
                    Console.WriteLine($"{i + 1}. {driverName} - From {ride.Pickup} To {ride.Dropoff} - {ride.CompletionTime:g}");
                }

                Console.Write("Enter ride number: ");
                if (!int.TryParse(Console.ReadLine(), out int rideIndex) || rideIndex < 1 || rideIndex > completedRides.Count)
                {
                    throw new ArgumentException("Invalid selection");
                }

                var selectedRide = completedRides[rideIndex - 1];
                string selectedDriverName = UserManager.Instance.GetDriverById(selectedRide.DriverId)?.Name ?? "Unknown";

                Console.Write($"Rate {selectedDriverName} (1-5 stars): ");
                if (!int.TryParse(Console.ReadLine(), out int rating) || rating < 1 || rating > 5)
                {
                    throw new ArgumentException("Rating must be between 1 and 5");
                }

                passenger.RateDriver(selectedRide.DriverId, rating);
                Console.WriteLine("Driver rated successfully!");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        private static void ShowDriverMenu()
        {
            var driver = (Driver)_currentUser;

            Console.Clear();
            Console.WriteLine($"*************************** DRIVER MENU - {driver.Name} ********************************");
            Console.WriteLine($"Status: {(driver.IsAvailable ? "Available" : "Busy")}");
            Console.WriteLine($"Current Location: {driver.CurrentLocation}");
            Console.WriteLine($"Total Earnings: R{driver.TotalEarnings:F2}");
            Console.WriteLine($"Rating: {driver.AverageRating:F1}/5.0 ({driver.TotalRatings} ratings)");
            Console.WriteLine("1. View Available Ride Requests");
            Console.WriteLine("2. View Active Rides");
            Console.WriteLine("3. Complete a Ride");
            Console.WriteLine("4. Update Location");
            Console.WriteLine("5. Toggle Availability");
            Console.WriteLine("6. View Completed Rides");
            Console.WriteLine("7. Logout");
            Console.Write("Choose an option: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ViewAvailableRideRequests(driver);
                    break;
                case "2":
                    ViewActiveRides(driver);
                    break;
                case "3":
                    CompleteRide(driver);
                    break;
                case "4":
                    UpdateLocation(driver);
                    break;
                case "5":
                    driver.IsAvailable = !driver.IsAvailable;
                    Console.WriteLine($"Your status is now {(driver.IsAvailable ? "Available" : "Unavailable")}");
                    Thread.Sleep(1500);
                    break;
                case "6":
                    ViewCompletedRides(driver);
                    break;
                case "7":
                    _currentUser = null;
                    Console.WriteLine("Logged out successfully.");
                    Thread.Sleep(1000);
                    break;
                default:
                    Console.WriteLine("Invalid option. Press any key to continue...");
                    Console.ReadKey();
                    break;
            }
        }

        private static void ViewAvailableRideRequests(Driver driver)
        {
            Console.Clear();
            Console.WriteLine("*********************** AVAILABLE RIDE REQUESTS ******************************");

            if (!driver.IsAvailable)
            {
                Console.WriteLine("You are currently unavailable. Change your status to view requests.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            var availableRequests = driver.GetAvailableRideRequests();

            if (availableRequests.Count == 0)
            {
                Console.WriteLine("No ride requests available nearby.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Nearby Ride Requests:");
            for (int i = 0; i < availableRequests.Count; i++)
            {
                var request = availableRequests[i];
                var passengerName = UserManager.Instance.GetPassengerById(request.PassengerId)?.Name ?? "Unknown";
                var distance = driver.CurrentLocation.CalculateDistance(request.Pickup);

                Console.WriteLine($"{i + 1}. Passenger: {passengerName}");
                Console.WriteLine($"   Pickup: {request.Pickup} ({distance:F2} km away)");
                Console.WriteLine($"   Dropoff: {request.Dropoff}");
                Console.WriteLine($"   Estimated Fare: R{request.EstimatedFare:F2}");
                Console.WriteLine($"   Request ID: {request.Id}");
                Console.WriteLine("   ------------------------");
            }

            Console.Write("Enter request number to accept (0 to cancel): ");
            if (int.TryParse(Console.ReadLine(), out int requestIndex) && requestIndex > 0 && requestIndex <= availableRequests.Count)
            {
                try
                {
                    driver.AcceptRide(availableRequests[requestIndex - 1].Id);
                    Console.WriteLine("Ride accepted! You are now on your way to pickup the passenger.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private static void ViewActiveRides(Driver driver)
        {
            Console.Clear();
            Console.WriteLine("********************* ACTIVE RIDES ************************************");

            var activeRides = RideManager.Instance.GetActiveRidesForDriver(driver.Id);

            if (activeRides.Count == 0)
            {
                Console.WriteLine("You have no active rides.");
            }
            else
            {
                foreach (var ride in activeRides)
                {
                    var passengerName = UserManager.Instance.GetPassengerById(ride.PassengerId)?.Name ?? "Unknown";

                    Console.WriteLine($"Ride ID: {ride.Id}");
                    Console.WriteLine($"Passenger: {passengerName}");
                    Console.WriteLine($"Pickup: {ride.Pickup}");
                    Console.WriteLine($"Dropoff: {ride.Dropoff}");
                    Console.WriteLine($"Start Time: {ride.StartTime:g}");
                    Console.WriteLine($"Fare: R{ride.Fare:F2}");
                    Console.WriteLine("------------------------");
                }
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private static void CompleteRide(Driver driver)
        {
            Console.Clear();
            Console.WriteLine("**************************** COMPLETE A RIDE *********************************");

            var activeRides = RideManager.Instance.GetActiveRidesForDriver(driver.Id);

            if (activeRides.Count == 0)
            {
                Console.WriteLine("You have no active rides to complete.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Select a ride to complete:");
            for (int i = 0; i < activeRides.Count; i++)
            {
                var ride = activeRides[i];
                var passengerName = UserManager.Instance.GetPassengerById(ride.PassengerId)?.Name ?? "Unknown";
                Console.WriteLine($"{i + 1}. Passenger: {passengerName} - To: {ride.Dropoff} - Fare: R{ride.Fare:F2}");
            }

            Console.Write("Enter ride number: ");
            if (int.TryParse(Console.ReadLine(), out int rideIndex) && rideIndex > 0 && rideIndex <= activeRides.Count)
            {
                try
                {
                    driver.CompleteRide(activeRides[rideIndex - 1].Id);
                    Console.WriteLine("Ride completed successfully!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Invalid selection.");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private static void UpdateLocation(Driver driver)
        {
            Console.Clear();
            Console.WriteLine("******************* UPDATE LOCATION ***************************");
            Console.WriteLine($"Current Location: {driver.CurrentLocation}");

            try
            {
                Console.Write("New Latitude: ");
                if (!double.TryParse(Console.ReadLine(), out double latitude))
                {
                    throw new ArgumentException("Invalid latitude");
                }

                Console.Write("New Longitude: ");
                if (!double.TryParse(Console.ReadLine(), out double longitude))
                {
                    throw new ArgumentException("Invalid longitude");
                }

                driver.UpdateLocation(new Location(latitude, longitude));
                Console.WriteLine("Location updated successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private static void ViewCompletedRides(Driver driver)
        {
            Console.Clear();
            Console.WriteLine("**************** COMPLETED RIDES ******************************");

            var completedRides = driver.CompletedRides;

            if (completedRides.Count == 0)
            {
                Console.WriteLine("You have no completed rides.");
            }
            else
            {
                foreach (var ride in completedRides)
                {
                    var passengerName = UserManager.Instance.GetPassengerById(ride.PassengerId)?.Name ?? "Unknown";
                    string rating = ride.DriverRating.HasValue ? $"{ride.DriverRating}/5" : "Not rated";

                    Console.WriteLine($"Ride ID: {ride.Id}");
                    Console.WriteLine($"Passenger: {passengerName}");
                    Console.WriteLine($"From: {ride.Pickup} To: {ride.Dropoff}");
                    Console.WriteLine($"Completion Time: {ride.CompletionTime:g}");
                    Console.WriteLine($"Fare: R{ride.Fare:F2}");
                    Console.WriteLine($"Rating: {rating}");
                    Console.WriteLine("------------------------");
                }
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

    }



    // Interfaces 
    public interface IRideable
    {
        void RequestRide(Location pickup, Location dropoff);
        void RateDriver(string driverId, int rating);
        List<Ride> GetRideHistory();
    }

    public interface IPayable
    {
        bool ProcessPayment(double amount);
        void AddFunds(double amount);
        double GetBalance();
    }


    public enum RideStatus
    {
        Requested,
        Accepted,
        InProgress,
        Completed,
        Cancelled
    }



    public class Report
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime GeneratedTime { get; set; }

        public Report(string title, string content)
        {
            Title = title;
            Content = content;
            GeneratedTime = DateTime.Now;
        }
    }

    // Custom exceptions
    public class InsufficientBalanceException : Exception
    {
        public InsufficientBalanceException(string message) : base(message) { }
    }





}
