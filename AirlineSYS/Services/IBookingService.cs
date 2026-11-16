using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AirlineSYS.Services
{
    public interface IBookingService
    {
        bool CancelBooking(
            int bookingId,
            string forename,
            string surname,
            string email
        );

        bool UpdatePassenger(
            int passengerId,
            string forename,
            string surname,
            string email
        );
    }
}
