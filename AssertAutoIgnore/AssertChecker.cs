using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Runtime.InteropServices;

namespace AssertAutoIgnore
{
    public class AssertChecker
    {
        public int? BannerlordProcessID;
        public int KillCount { get; set; }


        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindow(IntPtr ZeroOnly, string lpWindowName);

        public static Dictionary<string, int> WindowAutomationDictionary = new Dictionary<string, int>()
        {
            {"Always Ignore?", 0},
            {"RGL CONTENT WARNING", 0},
            {"RGL CONTENT ERROR", 0},
            {"RGL WARNING", 0},

            {"Safe Mode", 1},
            {"*_*", 1},

            {"ASSERT", 2},
            {"SAFE ASSERT", 2},
        };



        public virtual void OnTick()
        {
            if (BannerlordProcessID.HasValue)
            {
                Process bannerlordProcess;
                try
                {
                    Process.GetProcessById(BannerlordProcessID.Value).Refresh();
                    bannerlordProcess = Process.GetProcessById(BannerlordProcessID.Value);

                    if (bannerlordProcess == null || (!bannerlordProcess.ProcessName.Contains("Bannerlord") & !bannerlordProcess.ProcessName.Contains("MountAndBlade")))
                    {
                        Console.WriteLine("Bannerlord process lost!");
                        GetProcesses();
                        return;
                    }

                }
                catch
                {
                    Console.WriteLine("Bannerlord process lost!");
                    BannerlordProcessID = null;
                    return;
                }
                CheckForDialogBoxes();

            }
            else GetProcesses();
        }

        private void CheckForDialogBoxes()
        {
            foreach(KeyValuePair<string, int> windowAutomation in WindowAutomationDictionary)
            {
                IntPtr windowPointer = FindWindow(IntPtr.Zero, windowAutomation.Key);
                if (windowPointer != IntPtr.Zero)
                {
                    try 
                    {
                        FindAndInvokeButtonFromProcess(windowPointer, windowAutomation.Value, windowAutomation.Key);
                    }
                    catch { }
                }
            }
        }

        private void GetProcesses()
        {
            Process[] processes = Process.GetProcesses();
            Process bannerlordProcess = processes.Where(process => process.ProcessName.Contains("Bannerlord") || process.ProcessName.Contains("MountAndBlade")).FirstOrDefault();

            if (bannerlordProcess != null)
            {
                Console.WriteLine("Found Bannerlord process!");
                BannerlordProcessID = bannerlordProcess.Id;
            }
        }

        private void FindAndInvokeButtonFromProcess(IntPtr pointerHandle, int buttonIndex, string windowName)
        {
            Console.WriteLine($"Closing: {windowName}");

            AutomationElement bannerlordRoot = AutomationElement.FromHandle(pointerHandle);

            PropertyCondition buttonProperty = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button);
            AutomationElementCollection allButtons = bannerlordRoot.FindAll(TreeScope.Children, buttonProperty);
            AutomationElement button = allButtons[buttonIndex];

            InvokePattern pattern = button.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
            pattern.Invoke();

            KillCount++;
            Console.WriteLine($"Asserts killed: {KillCount}");
        }

    }


}
