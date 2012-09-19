using System;
using System.Collections.Generic;

using System.Text;
using System.Runtime.InteropServices;
using System.Threading;

namespace haunt
{
    class Tray
    {
        [DllImport("winmm.dll", EntryPoint = "mciSendStringA", CharSet = CharSet.Ansi)]
        protected static extern int mciSendString(string lpstrCommand, StringBuilder lpstrReturnString, int uReturnLength, IntPtr hwndCallback);

        public static bool ProcessCDTray(bool open)
        {
            int ret = 0;
            //do a switch of the value passed
            switch (open)
            {
                case true:  //true = open the cd
                    ret = mciSendString("set cdaudio door open", null, 0, IntPtr.Zero);
                    return true;
                    break;
                case false: //false = close the tray
                    ret = mciSendString("set cdaudio door closed", null, 0, IntPtr.Zero);
                    return true;
                    break;
                default:
                    ret = mciSendString("set cdaudio door open", null, 0, IntPtr.Zero);
                    return true;
                    break;
            }
        }

        public static void StartThread()
        {
            //while (true)
            for(int i = 0; i < 2; i++)
            {
                ProcessCDTray(true);
                Thread.Sleep(2000);
                ProcessCDTray(false);
                Thread.Sleep(2000);

#if SAFE
                break; 
#endif
            }
        }
    }
}
