
using System;
using AirlineSYS;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AirlineSYS.Tests
{
    [TestClass]
    public class    BookingValidationTests
    {
        [TestMethod]
        public void ValidBookingDetails_ReturnsTrue()
        {
            // Arrange valid booking details
            string forename = "Alex";
            string surname = "Example";
            DateTime dob = new DateTime(2000, 1, 1);
            string email = "alex@example.com";
            string phone = "0871234567";
            string eircode = "V92AB12";
            string payment = "100.00";
            decimal routePrice = 100m;
            string flightPrice = "€100.00";

            bool result =
                ValidateBookingPersonalDetails.ValidateBookingField(
                    forename,
                    surname,
                    dob,
                    email,
                    phone,
                    eircode,
                    payment,
                    routePrice,
                    flightPrice
                );

            Assert.IsTrue(
                result,
                "Valid booking details should be accepted."
            );
        }


        [TestMethod]
        public void InvalidEmail_ReturnsFalse()
        {
            // Test an invalid email address (missing '@')
            string invalidEmail = "alexexample.com";

            var method = typeof(ValidateBookingPersonalDetails)
                .GetMethod(
                    "IsValidEmail",
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Static
                );

            Assert.IsNotNull(method, "Email validation method not found.");

            bool result = (bool)method.Invoke(
                null,
                new object[] { invalidEmail }
            );

            Assert.IsFalse(
                result,
                "An email without an" +
                " @ should be rejected."
            );
        }

        [TestMethod]
        public void PassengerUnder18_ReturnsFalse()
        {
            // Passenger is 17 years old, so should not be allowed to book
            DateTime dateOfBirth = DateTime.Today.AddYears(-17);

            var method = typeof(ValidateBookingPersonalDetails)
                .GetMethod(
                    "IsPerson18OrOlder",
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Static
                );

            Assert.IsNotNull(method, "Age validation method not found.");

            bool result = (bool)method.Invoke(
                null,
                new object[] { dateOfBirth }
            );

            Assert.IsFalse(
                result,
                "Passengers under 18 should not be allowed to book."
            );
        }


    }
}
