using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AirlineSYS.Services;

namespace AirlineSYS.Tests
{
    [TestClass]
    public class DependencyInjectionTests
    {
        [TestMethod]
        public void MainMenu_NullBookingService_ThrowsException()
        {
            IBookingService bookingService = null;

            try
            {
                frmAirlineMainMenu mainMenu = new frmAirlineMainMenu(bookingService);
                Assert.Fail("Expected ArgumentNullException was not thrown.");
            }
            catch (ArgumentNullException)
            {
                //TestED passed
            }
        }

        [TestMethod]
        public void UpdateBooking_NullBookingService_ThrowsException()
        {
            frmAirlineMainMenu parent = null;
            IBookingService bookingService = null;

            try
            {
                frmUpdateBooking updateBooking = new frmUpdateBooking(parent, bookingService);
                Assert.Fail("Expected ArgumentNullException was not thrown.");
            }
            catch (ArgumentNullException)
            {
                //TestED passed
            }
        }

        [TestMethod]
        public void CancelBooking_NullBookingService_ThrowsException()
        {
            frmAirlineMainMenu parent = null;
            IBookingService bookingService = null;

            try
            {
                frmCancelBooking cancelBooking = new frmCancelBooking(parent, bookingService);
                Assert.Fail("Expected ArgumentNullException was not thrown.");
            }
            catch (ArgumentNullException)
            {
                //TestED passed
            }
        }

        [TestMethod]
        public void CancelBookingDetails_NullBookingService_ThrowsException()
        {
            string flightNumber = "TS0001";
            IBookingService bookingService = null;

            try
            {
                frmCancelBookingDetails cancelBookingDetails = new frmCancelBookingDetails(flightNumber, bookingService);
                Assert.Fail("Expected ArgumentNullException was not thrown.");
            }
            catch (ArgumentNullException)
            {
                //TestED passed
            }
        }
    }
}