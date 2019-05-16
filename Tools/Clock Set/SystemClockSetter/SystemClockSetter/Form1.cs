using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SystemClockSetter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                var httpClient = new HttpClient();
                var response = await httpClient.GetAsync("http://www.google.com");
                var d = response.Headers.Date.Value.DateTime;
                var iranTime = TimeZoneInfo.ConvertTimeFromUtc(d,
                    TimeZoneInfo.FindSystemTimeZoneById("Iran Standard Time"));
                var result = ClockHelper.SetClock(iranTime);
                label2.Text = iranTime.ToString();
                
            }
            catch
            {
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
