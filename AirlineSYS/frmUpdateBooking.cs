using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AirlineSYS.Services;
using Oracle.ManagedDataAccess.Client;

namespace AirlineSYS
{
    public partial class frmUpdateBooking : Form
    {
        frmAirlineMainMenu parent;
        private DateTime originalFlightDate;
        private readonly IBookingService bookingService;

        public frmUpdateBooking()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            bookingService = Program.CreateBookingService();
        }
        public frmUpdateBooking(frmAirlineMainMenu parent)
        {
            InitializeComponent();
            this.parent = parent;
            bookingService = Program.CreateBookingService();
        }

        public frmUpdateBooking(frmAirlineMainMenu parent, IBookingService bookingService)
        {
            if (bookingService == null)
            {
                throw new ArgumentNullException(nameof(bookingService));
            }

            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.parent = parent;
            this.bookingService = bookingService;
        }

        private void munBack_Click(object sender, EventArgs e)
        {
            if (parent != null && !parent.IsDisposed)
            {
                parent.Show();
            }
            else
            {
                frmAirlineMainMenu frmAirlineMainMenu = new frmAirlineMainMenu();
                frmAirlineMainMenu.Show();
            }

            this.Close();
        }


        private void btnUpdateBookingIDSearch_Click(object sender, EventArgs e)
        {
            grpUpdateCreateBookingDetails.Visible = true;
            grpRetriveUpdateBooking.Visible = true;
            int bookingID = Convert.ToInt32(txtBookingID.Text);
            DataTable dt = Booking.findBookingDetails(bookingID);

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];

                lblUpdateFlightNumber.Text = row["FlightNumber"].ToString();
                cboDeptimeDetail.Text = row["FlightTime"].ToString();
                dptUpdateBookingDate.Text = ((DateTime)row["FlightDate"]).ToString();
                lblSeatNumDetail.Text = row["SeatNum"].ToString();
                nudNumBaggage.Text = row["NumBaggage"].ToString();
                lblUpdatePassengerID.Text = row["PassengerID"].ToString();
                lblUpdateBookingRouteID.Text = row["RouteID"].ToString();
                int routeID = Convert.ToInt32(row["RouteID"]);

                originalFlightDate = (DateTime)row["FlightDate"];
                dptUpdateBookingDate.Text = originalFlightDate.ToString();

                Route route = Route.getAirportsByRouteID(routeID);

                if (route != null)
                {
                    cboUpdateDeptAirportDetail.Text = route.getDepartureAirport();
                    cboUpdateArrAirportDetail.Text = route.getArrivalAirport();
                }
                else
                {
                    MessageBox.Show("Selected route details are not available.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                lblUpdateBookingID.Text = row["BookingID"].ToString();
                lbFlightBookingPriceDetail.Text = row["AmountPaid"].ToString();
                txtUpdateForeName.Text = row["forename"].ToString();
                txtUpdateSurname.Text = row["surname"].ToString();
                txtUpdateBookingEmail.Text = row["Email"].ToString();
                dtpDOBUpdate.Text = ((DateTime)row["DateOfBirth"]).ToString();
                txtUpdateBooingPhone.Text = row["Phone"].ToString();
                txtUpdateEircode.Text = row["Eircode"].ToString();
            }
            else
            {
                MessageBox.Show("No booking found with the given ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }        
        private void btnUpadateBookingConfirm_Click(object sender, EventArgs e)
        {
            int passengerID = Convert.ToInt32(lblUpdatePassengerID.Text);

            if (!ValidateUpdatedBookingPersonalDetails.ValidatePersonalDetails(txtUpdateForeName.Text, txtUpdateSurname.Text, dtpDOBUpdate.Value, txtUpdateBookingEmail.Text, txtUpdateBooingPhone.Text, txtUpdateEircode.Text) || !ValidateUpdatedBookingPersonalDetails.ValidateNumBaggage(Convert.ToInt32(nudNumBaggage.Value)))
            {
                return;
            }

            try
            {
                string forename = txtUpdateForeName.Text;
                string surname = txtUpdateSurname.Text;
                DateTime dateOfBirth = DateTime.Parse(dtpDOBUpdate.Text);
                string email = txtUpdateBookingEmail.Text;
                string phone = txtUpdateBooingPhone.Text;
                string eircode = txtUpdateEircode.Text;

                bool passengerUpdated = bookingService.UpdatePassenger(passengerID, forename, surname, dateOfBirth, email, phone, eircode);

                if (passengerUpdated == false)
                {
                    MessageBox.Show("Passenger ID not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                MessageBox.Show("Passenger details updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                Booking updatedBooking = new Booking(Convert.ToInt32(lblUpdateBookingID.Text), Convert.ToInt32(lblUpdatePassengerID.Text), Convert.ToInt32(lblUpdateBookingRouteID.Text), lblUpdateFlightNumber.Text, cboDeptimeDetail.Text, DateTime.Parse(dptUpdateBookingDate.Text), Convert.ToInt32(lblSeatNumDetail.Text), Convert.ToInt32(nudNumBaggage.Value), Convert.ToDecimal(lbFlightBookingPriceDetail.Text), "CONFIRMED");

                updatedBooking.updateBooking();
            }
            catch (OracleException ex)
            {
                MessageBox.Show("Oracle Exception: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ifRouteChange()
        {
            MessageBox.Show("Sorry, but the route cannot be changed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private void ifDateChange()
        {
            DialogResult dateChange = MessageBox.Show("Sorry, but the date cannot be changed. Do you wish to book another flight?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dateChange == DialogResult.Yes)
            {
                frmCreateBooking frmCreateBooking = new frmCreateBooking();
                frmCreateBooking.Show();
                this.Hide();
            }
            else if (dateChange == DialogResult.No)
            {
                dptUpdateBookingDate.Value = originalFlightDate;
            }
        }
        private void ifTimeChange()
        {
            MessageBox.Show("Sorry, but the departure time cannot be changed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private void askToBookAnotherFlight()
        {
            DialogResult flightConfirm = MessageBox.Show("Do you wish to book another flight?", "Book Flight", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (flightConfirm == DialogResult.Yes)
            {
                frmCreateBooking frmCreateBooking = new frmCreateBooking();
                frmCreateBooking.Show();
                this.Hide();
            }
        }

        private void cboUpdateDeptAirportDetail_MouseClick(object sender, MouseEventArgs e)
        {
            ifRouteChange();
            askToBookAnotherFlight();
        }

        private void cboUpdateArrAirportDetail_MouseClick(object sender, MouseEventArgs e)
        {
            ifRouteChange();
            askToBookAnotherFlight();
        }

        private void dptUpdateBookingDate_ValueChanged(object sender, EventArgs e)
        {
            int bookingID = Convert.ToInt32(txtBookingID.Text);

            DataTable dt = Booking.findBookingDetails(bookingID);

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];

                DateTime originalFlightDate = (DateTime)row["FlightDate"];
                DateTime newFlightDate = dptUpdateBookingDate.Value;

                if (newFlightDate != originalFlightDate)
                {
                    ifDateChange();
                }
            }

        }

        private void cboDeptimeDetail_MouseClick(object sender, MouseEventArgs e)
        {
            ifTimeChange();
            askToBookAnotherFlight();
        }
    }
}
