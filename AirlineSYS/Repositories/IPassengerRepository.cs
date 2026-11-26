
using System;
using Oracle.ManagedDataAccess.Client;

namespace AirlineSYS.Repositories
{
    public interface IPassengerRepository
    {
        bool UpdatePassenger(int passengerId, string forename, string surname, DateTime 
            dateOfBirth, string email, string phone, string eircode, OracleTransaction transaction);
    }
}
