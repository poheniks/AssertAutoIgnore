using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace AssertAutoIgnore
{
    public class AssertChecker
    {
        public int? BannerlordProcessID;

        public int KillCount { get; set; }

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
                        BannerlordProcessID = GetBannerlordProcess();
                        return;
                    }

                }
                catch
                {
                    Console.WriteLine("Bannerlord process lost!");
                    BannerlordProcessID = null;
                    return;
                }

                CheckForDialogBoxes(bannerlordProcess);
            }
            else BannerlordProcessID = GetBannerlordProcess();
        }

        private void CheckForDialogBoxes(Process process)
        {
            if (!process.MainWindowTitle.Contains("Bannerlord"))
            {
                switch (process.MainWindowTitle)
                {
                    case "Always Ignore?":
                    case "RGL CONTENT WARNING":
                    case "RGL CONTENT ERROR":
                    case "RGL WARNING":
                        FindAndInvokeButtonFromProcess(process, 0);
                        break;
                    case "Safe Mode":
                    case "*_*":
                        FindAndInvokeButtonFromProcess(process, 1);
                        break;
                    case "ASSERT":
                    case "SAFE ASSERT":
                        FindAndInvokeButtonFromProcess(process, 2);
                        break;
                    default:
                        break;
                }
            }
        }

        private int? GetBannerlordProcess()
        {
            Process[] processes = Process.GetProcesses();
            Process bannerlordProcess = processes.Where(process => process.ProcessName.Contains("Bannerlord") || process.ProcessName.Contains("MountAndBlade")).FirstOrDefault();
            if (bannerlordProcess != null)
            {
                Console.WriteLine("Found Bannerlord process!");
                int bannerlordProcessID = bannerlordProcess.Id;

                return bannerlordProcessID;
            }

            return null;
        }

        private void FindAndInvokeButtonFromProcess(Process process, int buttonIndex)
        {
            Console.WriteLine($"Closing: {process.MainWindowTitle}");

            AutomationElement bannerlordRoot = AutomationElement.FromHandle(process.MainWindowHandle);

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
