using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class RideManager
    {
        private static RideManager _instance;
        public static RideManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new RideManager();
                }
                return _instance;
            }
        }

        private List<RideRequest> _rideRequests;
        private List<Ride> _rides;

        private RideManager()
        {
            _rideRequests = new List<RideRequest>();
            _rides = new List<Ride>();
        }

        public void AddRideRequest(RideRequest request)
        {
            _rideRequests.Add(request);
        }

        public void AddRide(Ride ride)
        {
            _rides.Add(ride);
        }

        public List<RideRequest> GetPendingRideRequests()
        {
            return _rideRequests.Where(r => r.Status == RideStatus.Requested).ToList();
        }

        public RideRequest GetRideRequestById(string id)
        {
            return _rideRequests.FirstOrDefault(r => r.Id == id);
        }

        public Ride GetRideById(string id)
        {
            return _rides.FirstOrDefault(r => r.Id == id);
        }

        public List<Ride> GetAllRides()
        {
            return _rides;
        }

        public List<Ride> GetActiveRidesForDriver(string driverId)
        {
            return _rides.Where(r => r.DriverId == driverId && r.Status == RideStatus.InProgress).ToList();
        }
    }
}
