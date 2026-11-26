
using System;
using Oracle.ManagedDataAccess.Client;

namespace AirlineSYS.Repositories
{
    public class PassengerRepository : IPassengerRepository
    {
        public bool UpdatePassenger(int passengerId, string forename, string surname, DateTime dateOfBirth, string email, string phone, string eircode, OracleTransaction transaction)
        {
            string sqlQuery = @"UPDATE Passengers SET
                                Forename = :Forename,
                                Surname = :Surname,
                                DateOfBirth = :DateOfBirth,
                                Email = :Email,
                                Phone = :Phone,
                                Eircode = :Eircode
                                WHERE PassengerID = :PassengerID";

            using (OracleCommand cmd = new OracleCommand(sqlQuery, transaction.Connection))
            {
                cmd.Transaction = transaction;
                cmd.BindByName = true;
                cmd.Parameters.Add("Forename", OracleDbType.Varchar2).Value = forename;
                cmd.Parameters.Add("Surname", OracleDbType.Varchar2).Value = surname;
                cmd.Parameters.Add("DateOfBirth", OracleDbType.Date).Value = dateOfBirth;
                cmd.Parameters.Add("Email", OracleDbType.Varchar2).Value = email;
                cmd.Parameters.Add("Phone", OracleDbType.Varchar2).Value = phone;
                cmd.Parameters.Add("Eircode", OracleDbType.Varchar2).Value = eircode;
                cmd.Parameters.Add("PassengerID", OracleDbType.Int32).Value = passengerId;

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
