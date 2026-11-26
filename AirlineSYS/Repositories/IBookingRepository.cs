
using Oracle.ManagedDataAccess.Client;

namespace AirlineSYS.Repositories
{
    public interface IBookingRepository
    {
        string FindConfirmedBookingFlightNumber(int bookingId, string forename, string surname, string email, OracleTransaction transaction);
        bool CancelBooking(int bookingId, OracleTransaction transaction);
    }
}
