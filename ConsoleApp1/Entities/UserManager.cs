using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class UserManager
    {
        private static UserManager _instance;
        public static UserManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new UserManager();
                }
                return _instance;
            }
        }

        private List<Passenger> _passengers;
        private List<Driver> _drivers;
        private List<Admin> _admins;

        private UserManager()
        {
            _passengers = new List<Passenger>();
            _drivers = new List<Driver>();
            _admins = new List<Admin>();

            // Add a default admin
            _admins.Add(new Admin("Admin", "admin@ridesystem.com", "admin123", "1234567890"));
        }

        public void RegisterPassenger(Passenger passenger)
        {
            if (_passengers.Any(p => p.Email == passenger.Email))
            {
                throw new Exception("A user with this email already exists");
            }
            _passengers.Add(passenger);
        }

        public void RegisterDriver(Driver driver)
        {
            if (_drivers.Any(d => d.Email == driver.Email))
            {
                throw new Exception("A user with this email already exists");
            }
            _drivers.Add(driver);
        }

        public User AuthenticateUser(string email, string password)
        {
            var passenger = _passengers.FirstOrDefault(p => p.Email == email && p.Password == password);
            if (passenger != null) return passenger;

            var driver = _drivers.FirstOrDefault(d => d.Email == email && d.Password == password);
            if (driver != null) return driver;

            var admin = _admins.FirstOrDefault(a => a.Email == email && a.Password == password);
            if (admin != null) return admin;

            return null;
        }

        public Passenger GetPassengerById(string id)
        {
            return _passengers.FirstOrDefault(p => p.Id == id);
        }

        public Driver GetDriverById(string id)
        {
            return _drivers.FirstOrDefault(d => d.Id == id);
        }

        public List<Driver> GetAllDrivers()
        {
            return _drivers;
        }
    }
}
