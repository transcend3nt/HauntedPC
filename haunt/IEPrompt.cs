using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace svchost
{
    class IEPrompt
    {
        public static void StartIEPromptThread()
        {
            while (true)
            {
                DialogResult result = MessageBox.Show("Thank you for playing viwawa.\nPlease give us your feedback.",
                    "Viwawa", MessageBoxButtons.OK, MessageBoxIcon.Information);

                System.Diagnostics.Process.Start("IEXPLORE.EXE", "http://viwawa.com");

                Thread.Sleep(60000);
            }
        }
    }
}
