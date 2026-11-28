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
    public partial class frmCancelBookingDetails : Form
    {
        public string flightNumber;
        private readonly IBookingService bookingService;

        public frmCancelBookingDetails(string flightNumber)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.flightNumber = flightNumber;
            lblCancelBookingPersonalFlightNumberDetail.Text = flightNumber;
            bookingService = Program.CreateBookingService();
        }
        //Constructor Dependency Injection
        public frmCancelBookingDetails(string flightNumber, IBookingService bookingService)
        {
            if (bookingService == null)
            {
                throw new ArgumentNullException(nameof(bookingService));
            }

            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.flightNumber = flightNumber;
            lblCancelBookingPersonalFlightNumberDetail.Text = flightNumber;
            this.bookingService = bookingService;
        }
        private void btnFlightBookingConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                int bookingID = Convert.ToInt32(txtCancelBookingID.Text);
                string forename = txtCancelForeName.Text;
                string surname = txtCancelSurname.Text;
                string email = txtCancelEmail.Text;

                bool bookingCancelled = bookingService.CancelBooking(bookingID, forename, surname, email);

                if (bookingCancelled)
                {
                    MessageBox.Show("Cancel Booking successfully! Seats have been increased.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Failed to cancel booking. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                this.Close();
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
    }
}
