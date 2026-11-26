
using Oracle.ManagedDataAccess.Client;

namespace AirlineSYS.Repositories
{
    public interface IFlightRepository
    {
        bool IncreaseAvailableSeats(string flightNumber, OracleTransaction transaction);
    }
}
