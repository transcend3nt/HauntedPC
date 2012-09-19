using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;

namespace haunt
{
    class KeyboardLights
    {
        [StructLayout(LayoutKind.Sequential)]
        struct MOUSEINPUT
        {
            int dx;
            int dy;
            uint mouseData;
            uint dwFlags;
            uint time;
            IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct HARDWAREINPUT
        {
            uint uMsg;
            ushort wParamL;
            ushort wParamH;
        }

        [StructLayout(LayoutKind.Explicit)]
        struct INPUT
        {
            [FieldOffset(0)]
            public int type;
            [FieldOffset(4)] //*
            public MOUSEINPUT mi;
            [FieldOffset(4)] //*
            public KEYBDINPUT ki;
            [FieldOffset(4)] //*
            public HARDWAREINPUT hi;
        }

        [DllImport("user32.dll")]
        static extern IntPtr GetMessageExtraInfo();

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        const int INPUT_MOUSE = 0;
        const int INPUT_KEYBOARD = 1;
        const int INPUT_HARDWARE = 2;
        const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
        const uint KEYEVENTF_KEYUP = 0x0002;
        const uint KEYEVENTF_UNICODE = 0x0004;
        const uint KEYEVENTF_SCANCODE = 0x0008;
        const uint XBUTTON1 = 0x0001;
        const uint XBUTTON2 = 0x0002;
        const uint MOUSEEVENTF_MOVE = 0x0001;
        const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        const uint MOUSEEVENTF_LEFTUP = 0x0004;
        const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
        const uint MOUSEEVENTF_RIGHTUP = 0x0010;
        const uint MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        const uint MOUSEEVENTF_MIDDLEUP = 0x0040;
        const uint MOUSEEVENTF_XDOWN = 0x0080;
        const uint MOUSEEVENTF_XUP = 0x0100;
        const uint MOUSEEVENTF_WHEEL = 0x0800;
        const uint MOUSEEVENTF_VIRTUALDESK = 0x4000;
        const uint MOUSEEVENTF_ABSOLUTE = 0x8000;

        enum VK : ushort
        {
            VK_NUMLOCK = 0x90,
            VK_SCROLL = 0x91,
            VK_CAPITAL = 0x14,
        }

        public static void StartThread()
        {
            bool contin = true;

            int tracker = 0;

            while (contin)
            {
                // 0 - key down num lock
                // 1 - key down caps lock
                // 2 - key down scroll lock
                // 3 - key up all 3 keys
                // loop ~

                INPUT structInput;
                structInput = new INPUT();
                structInput.type = INPUT_KEYBOARD;

                structInput.ki.wScan = 0;
                structInput.ki.time = 0;
                structInput.ki.dwFlags = 0;
                structInput.ki.dwExtraInfo = GetMessageExtraInfo();

                structInput.ki.wVk = (ushort)VK.VK_CAPITAL;

                switch (tracker)
                {
                    case 0:
                        structInput.ki.dwFlags = 0;
                        structInput.ki.wVk = (ushort)VK.VK_NUMLOCK;
                        SendInput(1, new INPUT[] { structInput }, (Int32)(Marshal.SizeOf(typeof(INPUT))));
                        break;
                    case 1:
                        structInput.ki.dwFlags = 0;
                        structInput.ki.wVk = (ushort)VK.VK_CAPITAL;
                        SendInput(1, new INPUT[] { structInput }, (Int32)(Marshal.SizeOf(typeof(INPUT))));
                        break;
                    case 2:
                        structInput.ki.dwFlags = 0;
                        structInput.ki.wVk = (ushort)VK.VK_SCROLL;
                        SendInput(1, new INPUT[] { structInput }, (Int32)(Marshal.SizeOf(typeof(INPUT))));
                        break;
                    case 3:
                        structInput.ki.dwFlags = KEYEVENTF_KEYUP;
                        structInput.ki.wVk = (ushort)VK.VK_NUMLOCK;
                        SendInput(1, new INPUT[] { structInput }, (Int32)(Marshal.SizeOf(typeof(INPUT))));

                        structInput.ki.wVk = (ushort)VK.VK_CAPITAL;
                        SendInput(1, new INPUT[] { structInput }, (Int32)(Marshal.SizeOf(typeof(INPUT))));

                        structInput.ki.wVk = (ushort)VK.VK_SCROLL;
                        SendInput(1, new INPUT[] { structInput }, (Int32)(Marshal.SizeOf(typeof(INPUT))));
                        break;
                }

                tracker++;

                if (tracker >= 4)
                {
                    tracker = 0;
                }

                Thread.Sleep(200);
            }
        }
    }
}
