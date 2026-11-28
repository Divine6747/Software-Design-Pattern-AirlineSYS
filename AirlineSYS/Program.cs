using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AirlineSYS.Services;
using AirlineSYS.Repositories;

namespace AirlineSYS
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            IBookingService bookingService = CreateBookingService();

            Application.Run(new frmAirlineMainMenu(bookingService));
        }

        //Centralised creation of booking service dependencies
        internal static IBookingService CreateBookingService()
        {
            IBookingRepository bookingRepository = new BookingRepository();
            IPassengerRepository passengerRepository = new PassengerRepository();
            IFlightRepository flightRepository = new FlightRepository();

            IBookingService bookingService = new BookingService(DBConnect.oradb, bookingRepository, passengerRepository, flightRepository);

            return bookingService;
        }
    }
}