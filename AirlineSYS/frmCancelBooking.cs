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

namespace AirlineSYS
{
    public partial class frmCancelBooking : Form
    {
        frmAirlineMainMenu parent;
        private string flightNumber;
        private readonly IBookingService bookingService;

        public frmCancelBooking()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            bookingService = Program.CreateBookingService();
        }
        public frmCancelBooking(frmAirlineMainMenu parent, string flightNumber)
        {
            InitializeComponent();
            this.parent = parent;
            this.flightNumber = flightNumber;
            bookingService = Program.CreateBookingService();
        }
        //Constructor Dependency Injection
        public frmCancelBooking(frmAirlineMainMenu parent, IBookingService bookingService)
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

        private void btnCancelBookingIDSearch_Click(object sender, EventArgs e)
        {
            grpRetrievedBooking.Visible = true;
            grpCancelBookingDetails.Visible = true;

            int bookingID = Convert.ToInt32(txtCancelBookingID.Text);
            DataTable dt = Booking.findBookingDetails(bookingID);

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];

                lblCancelFlightNumber.Text = row["FlightNumber"].ToString();
                cboCancelDeptimeDetail.Text = row["FlightTime"].ToString();
                dtpDOBUpdate.Text = ((DateTime)row["FlightDate"]).ToString();
                lblCancelSeatNumDetail.Text = row["SeatNum"].ToString();
                nudCancelNumBaggage.Text = row["NumBaggage"].ToString();
                lblCancelPassengerID.Text = row["PassengerID"].ToString();
                lblCancelBookingRouteID.Text = row["RouteID"].ToString();
                int routeID = Convert.ToInt32(row["RouteID"]);

                Route route = Route.getAirportsByRouteID(routeID);
                cboCancelDeptAirportDetail.Visible = true;
                cboCancelDeptAirportDetail.BringToFront();

                if (route != null)
                {
                    cboCancelDeptAirportDetail.Text = route.getDepartureAirport();
                    cboCancelArrAirportDetail.Text = route.getArrivalAirport();
                }
                else
                {
                    MessageBox.Show("Selected route details are not available.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                lblCancelBookingID.Text = row["BookingID"].ToString();
                lbCancelFlightBookingPrice.Text = row["AmountPaid"].ToString();
                txtCancelForeName.Text = row["forename"].ToString();
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

        private void btnAirportConfirm_Click(object sender, EventArgs e)
        {
            DialogResult cancelConfirm = MessageBox.Show("Are you sure you want to cancel your Booking?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (cancelConfirm == DialogResult.Yes)
            {
                // Pass the flight number to the constructor of frmCancelBookingDetails
                frmCancelBookingDetails frmCancelBookingDetails = new frmCancelBookingDetails(lblCancelFlightNumber.Text, bookingService);
                this.Hide();
                frmCancelBookingDetails.Show();
            }
            else
            {
                return;
            }
        }

        private void frmCancelBooking_Load(object sender, EventArgs e)
        {

        }
    }
}
