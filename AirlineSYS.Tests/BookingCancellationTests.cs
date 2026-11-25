using System;
using System.Configuration;
using Oracle.ManagedDataAccess.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AirlineSYS.Services;

namespace AirlineSYS.Tests
{

    [TestClass]
    [DoNotParallelize]
    public class BookingCancellationTests
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
                    using (OracleCommand cmd = new OracleCommand("UPDATE BOOKINGS SET STATUS = 'CONFIRMED' WHERE BOOKINGID IN (1, 2, 3)", conn))
                    {
                        cmd.Transaction = transaction;
                        int affectedRows = cmd.ExecuteNonQuery();
                        Assert.AreEqual(3, affectedRows, "Three test bookings should be restored.");
                    }

                    using (OracleCommand cmd = new OracleCommand("UPDATE FLIGHTS SET NUMSEATAVAIL = 7 WHERE FLIGHTNUMBER = 'TS0001'", conn))
                    {
                        cmd.Transaction = transaction;
                        int affectedRows = cmd.ExecuteNonQuery();
                        Assert.AreEqual(1, affectedRows, "One test flight should be restored.");
                    }

                    transaction.Commit();
                }
            }
        }



        [TestMethod]
        public void TestDatabaseConnection()
        {
            ConnectionStringSettings connectionSettings = ConfigurationManager.ConnectionStrings["AirlineSYSTest"];
            Assert.IsNotNull(connectionSettings, "Test database connection string is missing.");
            string testConnectionString = connectionSettings.ConnectionString;
            Assert.IsFalse(string.IsNullOrWhiteSpace(testConnectionString), "Test database connection string is empty.");

            using (OracleConnection conn = new OracleConnection(testConnectionString))
            {
                conn.Open();

                using (OracleCommand cmd = new OracleCommand("SELECT USER FROM DUAL", conn))
                {
                    string databaseUser = Convert.ToString(cmd.ExecuteScalar());
                    Assert.AreEqual("C##AIRLINESYS_TEST", databaseUser, "Connected to the wrong Oracle database account.");
                }

                Assert.AreEqual(System.Data.ConnectionState.Open, conn.State);
            }
        }

        [TestMethod]
        public void BT07_CancelConfirmedBooking()
        {
            bool result = bookingService.CancelBooking(1, "Test", "PassengerOne", "test1@example.com");
            Assert.IsTrue(result, "Cancellation qill succeed for a confirmed booking.");

            ConnectionStringSettings connectionSettings = ConfigurationManager.ConnectionStrings["AirlineSYSTest"];
            Assert.IsNotNull(connectionSettings, "Test database connection string is missing.");
            string testConnectionString = connectionSettings.ConnectionString;

            using (OracleConnection conn = new OracleConnection(testConnectionString))
            {
                conn.Open();

                using (OracleCommand cmd = new OracleCommand("SELECT STATUS FROM BOOKINGS WHERE BOOKINGID = 1", conn))
                {
                    string status = Convert.ToString(cmd.ExecuteScalar());
                    Assert.AreEqual("CANCELLED", status, "Booking status should be CANCELLED.");
                }

                using (OracleCommand cmd = new OracleCommand("SELECT NUMSEATAVAIL FROM FLIGHTS WHERE FLIGHTNUMBER = 'TS0001'", conn))
                {
                    int availableSeats = Convert.ToInt32(cmd.ExecuteScalar());
                    Assert.AreEqual(8, availableSeats, "Available seats should increase from 7 to 8.");
                }
            }
        }

        [TestMethod]
        public void BT08_CancelWithIncorrectPassengerDetails()
        {
            // Arrange
            int bookingId = 2;
            string forename = "Test";
            string surname = "PassengerTwo";
            string incorrectEmail = "wrong@example.com";

            // Act
            bool result = bookingService.CancelBooking(bookingId, forename, surname, incorrectEmail);

            // Assert
            Assert.IsFalse(result, "Cancellation should fail when incorrect passenger details are provided.");

            ConnectionStringSettings connectionSettings = ConfigurationManager.ConnectionStrings["AirlineSYSTest"];
            Assert.IsNotNull(connectionSettings, "Test database connection string is missing.");
            string testConnectionString = connectionSettings.ConnectionString;

            using (OracleConnection conn = new OracleConnection(testConnectionString))
            {
                conn.Open();

                using (OracleCommand cmd = new OracleCommand("SELECT STATUS FROM BOOKINGS WHERE BOOKINGID = 2", conn))
                {
                    string status = Convert.ToString(cmd.ExecuteScalar());
                    Assert.AreEqual("CONFIRMED", status, "Booking status should remain CONFIRMED.");
                }

                using (OracleCommand cmd = new OracleCommand("SELECT NUMSEATAVAIL FROM FLIGHTS WHERE FLIGHTNUMBER = 'TS0001'", conn))
                {
                    int availableSeats = Convert.ToInt32(cmd.ExecuteScalar());
                    Assert.AreEqual(7, availableSeats, "Available seats should remain unchanged at 7.");
                }
            }
        }

        [TestMethod]
        public void BT09_CancelAlreadyCancelledBooking()
        {
            // Arrange
            int bookingId = 3;
            string forename = "Test";
            string surname = "PassengerThree";
            string email = "test3@example.com";

            bool firstCancellation = bookingService.CancelBooking(bookingId, forename, surname, email);
            Assert.IsTrue(firstCancellation, "The first cancellation should succeed.");

            using (OracleConnection conn = new OracleConnection(testConnectionString))
            {
                conn.Open();

                using (OracleCommand cmd = new OracleCommand("SELECT STATUS FROM BOOKINGS WHERE BOOKINGID = 3", conn))
                {
                    string status = Convert.ToString(cmd.ExecuteScalar());
                    Assert.AreEqual("CANCELLED", status, "The booking should already be cancelled.");
                }

                using (OracleCommand cmd = new OracleCommand("SELECT NUMSEATAVAIL FROM FLIGHTS WHERE FLIGHTNUMBER = 'TS0001'", conn))
                {
                    int availableSeats = Convert.ToInt32(cmd.ExecuteScalar());
                    Assert.AreEqual(8, availableSeats, "Available seats should be 8 after the first cancellation.");
                }
            }

            // Act
            bool secondCancellation = bookingService.CancelBooking(bookingId, forename, surname, email);

            // Assert
            Assert.IsFalse(secondCancellation, "An already cancelled booking cannot be cancelled again.");

            using (OracleConnection conn = new OracleConnection(testConnectionString))
            {
                conn.Open();

                using (OracleCommand cmd = new OracleCommand("SELECT STATUS FROM BOOKINGS WHERE BOOKINGID = 3", conn))
                {
                    string status = Convert.ToString(cmd.ExecuteScalar());
                    Assert.AreEqual("CANCELLED", status, "Booking status should remain CANCELLED.");
                }

                using (OracleCommand cmd = new OracleCommand("SELECT NUMSEATAVAIL FROM FLIGHTS WHERE FLIGHTNUMBER = 'TS0001'", conn))
                {
                    int availableSeats = Convert.ToInt32(cmd.ExecuteScalar());
                    Assert.AreEqual(8, availableSeats, "No additional seat should be restored.");
                }
            }
        }

    }
}
