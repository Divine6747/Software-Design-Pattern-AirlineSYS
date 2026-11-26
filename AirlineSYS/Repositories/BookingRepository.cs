
using System;
using Oracle.ManagedDataAccess.Client;

namespace AirlineSYS.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        public string FindConfirmedBookingFlightNumber(int bookingId, string forename, string surname, string email, OracleTransaction transaction)
        {
            string sqlQuery = @"
                SELECT b.FlightNumber
                FROM Bookings b
                JOIN Passengers p ON b.PassengerID = p.PassengerID
                WHERE b.BookingID = :bookingID
                AND b.Status = 'CONFIRMED'
                AND p.Forename = :forename
                AND p.Surname = :surname
                AND p.Email = :email
                FOR UPDATE OF b.Status";

            using (OracleCommand cmd = new OracleCommand(sqlQuery, transaction.Connection))
            {
                cmd.Transaction = transaction;
                cmd.BindByName = true;
                cmd.Parameters.Add("bookingID", OracleDbType.Int32).Value = bookingId;
                cmd.Parameters.Add("forename", OracleDbType.Varchar2).Value = forename;
                cmd.Parameters.Add("surname", OracleDbType.Varchar2).Value = surname;
                cmd.Parameters.Add("email", OracleDbType.Varchar2).Value = email;

                object result = cmd.ExecuteScalar();

                if (result == null || result == DBNull.Value)
                {
                    return null;
                }

                return Convert.ToString(result);
            }
        }

        public bool CancelBooking(int bookingId, OracleTransaction transaction)
        {
            string sqlQuery = @"
                UPDATE Bookings
                SET Status = 'CANCELLED'
                WHERE BookingID = :bookingID
                AND Status = 'CONFIRMED'";

            using (OracleCommand cmd = new OracleCommand(sqlQuery, transaction.Connection))
            {
                cmd.Transaction = transaction;
                cmd.BindByName = true;
                cmd.Parameters.Add("bookingID", OracleDbType.Int32).Value = bookingId;

                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
