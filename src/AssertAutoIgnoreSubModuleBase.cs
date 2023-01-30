using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace AssertAutoIgnore
{
    public class AssertAutoIgnoreSubModuleBase : MBSubModuleBase
    {
        private Thread endDialogThread;

        [DllImport("user32.dll")]
        static extern IntPtr SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern bool EnableWindow(IntPtr hWnd, bool enabled);

        [DllImport("user32.dll", ExactSpelling = true)]
        static extern bool EndDialog(IntPtr hWnd, IntPtr result);


        protected override void OnSubModuleLoad()
        {
            endDialogThread = new Thread(() => EndDialog());
            endDialogThread.Start();
        }

        protected override void OnSubModuleUnloaded()
        {
            endDialogThread.Abort();
        }

        private void EndDialog()
        {
            while (true)
            {
                CheckDialogBox();
            }
        }

        private void CheckDialogBox()
        {
            Process process = Process.GetCurrentProcess();
            if (process.MainWindowTitle == "ASSERT" | process.MainWindowTitle == "*_*" | process.MainWindowTitle == "Always Ignore?")
            {
                IntPtr windowHandle = process.MainWindowHandle;

                IntPtr returnResult = new IntPtr(5);
                if (!Module.CurrentModule.LoadingFinished || process.MainWindowTitle == "Always Ignore?") returnResult = new IntPtr(1);

                EndDialog(windowHandle, returnResult);

                if (Module.CurrentModule.LoadingFinished)
                {
                    process.Refresh();
                    IntPtr processHandle = process.MainWindowHandle;
                    SetForegroundWindow(processHandle);

                    process.Refresh();
                    processHandle = process.MainWindowHandle;
                    EnableWindow(processHandle, true);
                }
            }
        }
    }
}
