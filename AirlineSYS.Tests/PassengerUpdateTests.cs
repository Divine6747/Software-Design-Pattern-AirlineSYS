
using System;
using System.Configuration;
using Oracle.ManagedDataAccess.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AirlineSYS.Services;

namespace AirlineSYS.Tests
{
    [TestClass]
    [DoNotParallelize]
    public class PassengerUpdateTests
    {
        private IBookingService bookingService;
        private string testConnectionString;

        [TestInitialize]
        public void Setup()
        {
            ConnectionStringSettings connectionSettings = ConfigurationManager.ConnectionStrings["AirlineSYSTest"];
            Assert.IsNotNull(connectionSettings, "Test database connection string is missing.");
            testConnectionString = connectionSettings.ConnectionString;
            Assert.IsFalse(string.IsNullOrWhiteSpace(testConnectionString), "Test database connection string is empty.");

            ResetTestData();

            bookingService = new BookingService(testConnectionString);
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (!string.IsNullOrWhiteSpace(testConnectionString))
            {
                ResetTestData();
            }
        }

        private void ResetTestData()
        {
            using (OracleConnection conn = new OracleConnection(testConnectionString))
            {
                conn.Open();

                using (OracleCommand cmd = new OracleCommand("SELECT USER FROM DUAL", conn))
                {
                    string databaseUser = Convert.ToString(cmd.ExecuteScalar());
                    Assert.AreEqual("C##AIRLINESYS_TEST", databaseUser, "Test data must only be modified in the test database.");
                }

                using (OracleTransaction transaction = conn.BeginTransaction())
                {
                    string sqlQuery = @"UPDATE PASSENGERS
                                        SET FORENAME = 'Test',
                                            SURNAME = 'PassengerOne',
                                            DATEOFBIRTH = DATE '1995-01-01',
                                            EMAIL = 'test1@example.com',
                                            PHONE = '0871234567',
                                            EIRCODE = 'V92AB12'
                                        WHERE PASSENGERID = 1";

                    using (OracleCommand cmd = new OracleCommand(sqlQuery, conn))
                    {
                        cmd.Transaction = transaction;
                        int affectedRows = cmd.ExecuteNonQuery();
                        Assert.AreEqual(1, affectedRows, "One test passenger should be restored.");
                    }

                    transaction.Commit();
                }
            }
        }

        [TestMethod]
        public void BT10_UpdatePassengerDetails()
        {
            // Arrange
            int passengerId = 1;
            string forename = "Updated";
            string surname = "PassengerUpdated";
            DateTime dateOfBirth = new DateTime(1994, 5, 15);
            string email = "updated@example.com";
            string phone = "0879876543";
            string eircode = "V92CD34";

            // Act
            bool result = bookingService.UpdatePassenger(passengerId, forename, surname, dateOfBirth, email, phone, eircode);

            // Assert
            Assert.IsTrue(result, "Passenger details should be updated successfully.");

            using (OracleConnection conn = new OracleConnection(testConnectionString))
            {
                conn.Open();

                string sqlQuery = @"SELECT FORENAME, SURNAME, DATEOFBIRTH, EMAIL, PHONE, EIRCODE
                            FROM PASSENGERS
                            WHERE PASSENGERID = :passengerId";

                using (OracleCommand cmd = new OracleCommand(sqlQuery, conn))
                {
                    cmd.BindByName = true;
                    cmd.Parameters.Add("passengerId", OracleDbType.Int32).Value = passengerId;

                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        Assert.IsTrue(reader.Read(), "The passenger should exist in the database.");

                        Assert.AreEqual(forename, reader.GetString(0), "Forename should be updated.");
                        Assert.AreEqual(surname, reader.GetString(1), "Surname should be updated.");
                        Assert.AreEqual(dateOfBirth, reader.GetDateTime(2), "Date of birth should be updated.");
                        Assert.AreEqual(email, reader.GetString(3), "Email should be updated.");
                        Assert.AreEqual(phone, reader.GetString(4), "Phone should be updated.");
                        Assert.AreEqual(eircode, reader.GetString(5), "Eircode should be updated.");
                    }
                }
            }
        }

    }
}
