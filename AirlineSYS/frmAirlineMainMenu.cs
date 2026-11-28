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
    public partial class frmAirlineMainMenu : Form
    {
        frmAirlineMainMenu parent;
        private readonly IBookingService bookingService;

        public frmAirlineMainMenu()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            bookingService = Program.CreateBookingService();
        }
        public frmAirlineMainMenu(frmAirlineMainMenu parent)
        {
            InitializeComponent();
            this.parent = parent;
            bookingService = Program.CreateBookingService();
        }

        public frmAirlineMainMenu(IBookingService bookingService)
        {
            if (bookingService == null)
            {
                throw new ArgumentNullException(nameof(bookingService));
            }

            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.bookingService = bookingService;
        }

        private void mnuEndRoute_Click_1(object sender, EventArgs e)
        {
            frmEndRoute frmAirlineMainMenu = new frmEndRoute();
            this.Hide();
            frmAirlineMainMenu.Show();
        }


        private void mnuScheduleFlight_Click_1(object sender, EventArgs e)
        {
            frmScheduleFlight frmAirlineMainMenu = new frmScheduleFlight();
            this.Hide();
            frmAirlineMainMenu.Show();
        }

        private void mnuUpdateFlight_Click_1(object sender, EventArgs e)
        {
            frmUpdateFlight frmAirlineMainMenu = new frmUpdateFlight();
            this.Hide();
            frmAirlineMainMenu.Show();
        }

        private void mnuCancelFlight_Click_1(object sender, EventArgs e)
        {
            frmCancelFlight frmAirlineMainMenu = new frmCancelFlight();
            this.Hide();
            frmAirlineMainMenu.Show();
        }

        private void mnuCreateBooking_Click_1(object sender, EventArgs e)
        {
            frmCreateBooking frmCreateBooking = new frmCreateBooking();
            this.Hide();
            frmCreateBooking.Show();
        }

        private void mnuUpdateBooking_Click_1(object sender, EventArgs e)
        {
            frmUpdateBooking frmAirlineMainMenu = new frmUpdateBooking(this, bookingService);
            this.Hide();
            frmAirlineMainMenu.Show();
        }

        private void mnuCancelBooking_Click_1(object sender, EventArgs e)
        {
            frmCancelBooking frmAirlineMainMenu = new frmCancelBooking(this, bookingService);
            this.Hide();
            frmAirlineMainMenu.Show();
        }

        private void mnuAddAirport_Click_1(object sender, EventArgs e)
        {
            frmAddAirport frmAirlineMainMenu = new frmAddAirport();
            this.Hide();
            frmAirlineMainMenu.Show();
        }
        private void mnuAddOperator_Click_1(object sender, EventArgs e)
        {
            frmAddOperator frmAirlineMainMenu = new frmAddOperator();
            this.Hide();
            frmAirlineMainMenu.Show();
        }

        private void mnuCreateRoute_Click_1(object sender, EventArgs e)
        {
            frmCreateRoute frmAirlineMainMenu = new frmCreateRoute();
            this.Hide();
            frmAirlineMainMenu.Show();
        }

        private void mnuYearRevenue_Click(object sender, EventArgs e)
        {
            frmYearlyRevenueAnalysis frmAirlineMainMenu = new frmYearlyRevenueAnalysis();
            this.Hide();
            frmAirlineMainMenu.Show();
        }

        private void mnuYearRouteAnalysis_Click(object sender, EventArgs e)
        {
            frmYearlyRouteAnalysis frmAirlineMainMenu = new frmYearlyRouteAnalysis();
            this.Hide();
            frmAirlineMainMenu.Show();
        }

        private void munExit_Click_1(object sender, EventArgs e)
        {
             Application.Exit();
        }
    }
}
