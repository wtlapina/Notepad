using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Notepad
{
    public partial class Notepad : Form
    {
        Int16 _xx = 10;
        Int32 _x_coor = 0;
        Int32 _y_coor = 0;
        int _screenWidth = 0;
        int _screenHeight = 0;
        public Notepad()
        {
            _screenWidth = Screen.PrimaryScreen.Bounds.Width;
            _screenHeight = Screen.PrimaryScreen.Bounds.Height;
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var interval = 5;
            TimeSpan timespent = TimeSpan.FromMilliseconds(IdleTimeFinder.GetIdleTime());
            var sec = timespent.ToString("ss");
            label1.Text = sec;

            var inputNumber = textBox1.Text.Trim();
            if (inputNumber != "")
            {
                int number1 = 0;
                bool isNumeric = int.TryParse(inputNumber, out number1);
                if (isNumeric)
                {
                    interval = Convert.ToInt32(inputNumber);
                }
            }
            _x_coor = NativeMethods.GetCursorPosition().X;
            _y_coor = NativeMethods.GetCursorPosition().Y;

            if (Convert.ToInt32(sec) >= interval)
            {
                if (_xx == 10)
                {
                    _y_coor = _y_coor + 50;
                    _xx = 20;
                }
                else
                {
                    _y_coor = _y_coor - 50;
                    _xx = 10;
                }

                if(checkBox2.Checked)
                   NativeMethods.SendMouseInput(_x_coor, _y_coor, _screenWidth - 1, _screenHeight - 1, false);
                if(checkBox1.Checked)
                    SendKeys.Send("%{TAB}");
            }
            //label2.Text = screenWidth + " " + screenHeight;
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }

        private void Notepad_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon1.Visible = true;
            }
        }
    }

    internal struct LASTINPUTINFO
    {
        public uint cbSize;

        public uint dwTime;
    }

    /// <summary>
    /// Helps to find the idle time, (in milliseconds) spent since the last user input
    /// </summary>
    public class IdleTimeFinder
    {
        [DllImport("User32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        [DllImport("Kernel32.dll")]
        private static extern uint GetLastError();

        public static uint GetIdleTime()
        {
            LASTINPUTINFO lastInPut = new LASTINPUTINFO();
            lastInPut.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(lastInPut);
            GetLastInputInfo(ref lastInPut);

            return ((uint)Environment.TickCount - lastInPut.dwTime);
        }
        /// <summary>
        /// Get the Last input time in milliseconds
        /// </summary>
        /// <returns></returns>
        public static long GetLastInputTime()
        {
            LASTINPUTINFO lastInPut = new LASTINPUTINFO();
            lastInPut.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(lastInPut);
            if (!GetLastInputInfo(ref lastInPut))
            {
                throw new Exception(GetLastError().ToString());
            }
            return lastInPut.dwTime;
        }
    }
}
