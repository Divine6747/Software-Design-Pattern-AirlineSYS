
using Oracle.ManagedDataAccess.Client;

namespace AirlineSYS.Repositories
{
    public class FlightRepository : IFlightRepository
    {
        public bool IncreaseAvailableSeats(string flightNumber, OracleTransaction transaction)
        {
            string sqlQuery = @"
                UPDATE Flights
                SET NUMSEATAVAIL = NUMSEATAVAIL + 1
                WHERE FLIGHTNUMBER = :flightNumber
                AND NUMSEATAVAIL < NUMSEATS";

            using (OracleCommand cmd = new OracleCommand(sqlQuery, transaction.Connection))
            {
                cmd.Transaction = transaction;
                cmd.BindByName = true;
                cmd.Parameters.Add("flightNumber", OracleDbType.Varchar2).Value = flightNumber;

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
