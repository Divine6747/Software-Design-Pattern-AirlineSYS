
using System;
using Oracle.ManagedDataAccess.Client;

namespace AirlineSYS.Services
{
    public class BookingService : IBookingService
    {
        private readonly string connectionString;

        public BookingService() : this(DBConnect.oradb)
        {
        }

        public BookingService(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("Connection string cannot be empty.", nameof(connectionString));
            }

            this.connectionString = connectionString;
        }

        public bool CancelBooking(int bookingId, string forename, string surname, string email)
        {
            using (var conn = new OracleConnection(connectionString))
            {
                conn.Open();

                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string flightNumber;

                        string findSql = @"
                            SELECT b.FlightNumber
                            FROM Bookings b
                            JOIN Passengers p ON b.PassengerID = p.PassengerID
                            WHERE b.BookingID = :bookingID
                            AND b.Status = 'CONFIRMED'
                            AND p.Forename = :forename
                            AND p.Surname = :surname
                            AND p.Email = :email
                            FOR UPDATE OF b.Status";

                        using (var cmd = new OracleCommand(findSql, conn))
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
                                transaction.Rollback();
                                return false;
                            }

                            flightNumber = Convert.ToString(result);
                        }

                        string cancelSql = @"
                            UPDATE Bookings
                            SET Status = 'CANCELLED'
                            WHERE BookingID = :bookingID
                            AND Status = 'CONFIRMED'";

                        using (var cmd = new OracleCommand(cancelSql, conn))
                        {
                            cmd.Transaction = transaction;
                            cmd.BindByName = true;
                            cmd.Parameters.Add("bookingID", OracleDbType.Int32).Value = bookingId;

                            if (cmd.ExecuteNonQuery() != 1)
                            {
                                transaction.Rollback();
                                return false;
                            }
                        }

                        string seatSql = @"
                            UPDATE Flights
                            SET NUMSEATAVAIL = NUMSEATAVAIL + 1
                            WHERE FLIGHTNUMBER = :flightNumber
                            AND NUMSEATAVAIL < NUMSEATS";

                        using (var cmd = new OracleCommand(seatSql, conn))
                        {
                            cmd.Transaction = transaction;
                            cmd.BindByName = true;
                            cmd.Parameters.Add("flightNumber", OracleDbType.Varchar2).Value = flightNumber;

                            if (cmd.ExecuteNonQuery() != 1)
                            {
                                transaction.Rollback();
                                return false;
                            }
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

                string sqlQuery = "UPDATE Passengers SET " +
                                  "Forename = :Forename, " +
                                  "Surname = :Surname, " +
                                  "DateOfBirth = :DateOfBirth, " +
                                  "Email = :Email, " +
                                  "Phone = :Phone, " +
                                  "Eircode = :Eircode " +
                                  "WHERE PassengerID = :PassengerID";

                using (OracleCommand cmd = new OracleCommand(sqlQuery, conn))
                {
                    cmd.BindByName = true;
                    cmd.Parameters.Add(":Forename", OracleDbType.Varchar2).Value = forename;
                    cmd.Parameters.Add(":Surname", OracleDbType.Varchar2).Value = surname;
                    cmd.Parameters.Add(":DateOfBirth", OracleDbType.Date).Value = dateOfBirth;
                    cmd.Parameters.Add(":Email", OracleDbType.Varchar2).Value = email;
                    cmd.Parameters.Add(":Phone", OracleDbType.Varchar2).Value = phone;
                    cmd.Parameters.Add(":Eircode", OracleDbType.Varchar2).Value = eircode;
                    cmd.Parameters.Add(":PassengerID", OracleDbType.Int32).Value = passengerId;

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
}
