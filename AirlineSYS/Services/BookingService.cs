
using System;
using Oracle.ManagedDataAccess.Client;
using AirlineSYS.Repositories;

namespace AirlineSYS.Services
{
    public class BookingService : IBookingService
    {
        private readonly string connectionString;
        private readonly IBookingRepository bookingRepository;
        private readonly IPassengerRepository passengerRepository;
        private readonly IFlightRepository flightRepository;

        public BookingService() : this(DBConnect.oradb)
        {
        }

        public BookingService(string connectionString) : this(connectionString, new BookingRepository(), new PassengerRepository(), new FlightRepository())
        {
        }

        public BookingService(string connectionString, IBookingRepository bookingRepository, IPassengerRepository passengerRepository, IFlightRepository flightRepository)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("Connection string cannot be empty.", nameof(connectionString));
            }

            if (bookingRepository == null)
            {
                throw new ArgumentNullException(nameof(bookingRepository));
            }

            if (passengerRepository == null)
            {
                throw new ArgumentNullException(nameof(passengerRepository));
            }

            if (flightRepository == null)
            {
                throw new ArgumentNullException(nameof(flightRepository));
            }

            this.connectionString = connectionString;
            this.bookingRepository = bookingRepository;
            this.passengerRepository = passengerRepository;
            this.flightRepository = flightRepository;
        }

        public bool CancelBooking(int bookingId, string forename, string surname, string email)
        {
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                conn.Open();

                using (OracleTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string flightNumber = bookingRepository.FindConfirmedBookingFlightNumber(bookingId, forename, surname, email, transaction);

                        if (flightNumber == null)
                        {
                            transaction.Rollback();
                            return false;
                        }

                        bool bookingCancelled = bookingRepository.CancelBooking(bookingId, transaction);

                        if (!bookingCancelled)
                        {
                            transaction.Rollback();
                            return false;
                        }

                        bool seatsUpdated = flightRepository.IncreaseAvailableSeats(flightNumber, transaction);

                        if (!seatsUpdated)
                        {
                            transaction.Rollback();
                            return false;
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public bool UpdatePassenger(int passengerId, string forename, string surname, DateTime dateOfBirth, string email, string phone, string eircode)
        {
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                conn.Open();

                using (OracleTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        bool passengerUpdated = passengerRepository.UpdatePassenger(passengerId, forename, surname, dateOfBirth, email, phone, eircode, transaction);

                        if (!passengerUpdated)
                        {
                            transaction.Rollback();
                            return false;
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
