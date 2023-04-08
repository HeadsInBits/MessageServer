using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;
using System.Windows;


namespace NetworkManager
{
    public partial class NotificationForm : Form
    {
        public string RoomText;
        public string MessageText;
        Timer t1 = new Timer();

        public NotificationForm()
        {
            InitializeComponent();
        }

        private void NotificationForm_Load(object sender, EventArgs e)
        {
            RoomNameLabel.Text = RoomText;
            MessageTextLaebl.Text = MessageText;

            this.Location = new Point(Screen.PrimaryScreen.Bounds.Width - 400, Screen.PrimaryScreen.Bounds.Height - 200);
            this.StartPosition = FormStartPosition.Manual;
            Opacity = 0;      //first the opacity is 0

            t1.Interval = 10;  //we'll increase the opacity every 10ms
            t1.Tick += new EventHandler(fadeIn);  //this calls the function that changes opacity 
            t1.Start();

        }

        void fadeIn(object sender, EventArgs e)
        {
            if (Opacity >= 1)
            {
                t1.Interval = 120;
                t1.Tick += new EventHandler(fadeOut); //this stops the timer if the form is completely displayed
            }
            else
                Opacity += 0.005;
        }

        void fadeOut(object sender, EventArgs e)
        {
            if (Opacity <= 0)
            {
                t1.Stop();
                this.Close();
            }
            else
                Opacity -= 0.05;
        }

    }
}
