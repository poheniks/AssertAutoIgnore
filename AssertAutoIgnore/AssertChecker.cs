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

        public virtual void OnTick()
        {
            if (BannerlordProcessID.HasValue)
            {
                Process bannerlordProcess;
                try
                {
                    Process.GetProcessById(BannerlordProcessID.Value).Refresh();
                    bannerlordProcess = Process.GetProcessById(BannerlordProcessID.Value);
                }
                catch
                {
                    Console.WriteLine("Bannerlord process lost!");
                    BannerlordProcessID = null;
                    return;
                }


                if (bannerlordProcess == null || (!bannerlordProcess.ProcessName.Contains("Bannerlord") & !bannerlordProcess.ProcessName.Contains("MountAndBlade")))
                {
                    Console.WriteLine("Bannerlord process lost!");
                    BannerlordProcessID = GetBannerlordProcess();
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
                    case "RGL CONTENT WARNING":
                    case "RGL CONTENT ERROR":
                        FindAndInvokeButtonFromProcess(process, "OK");
                        break;
                    case "Safe Mode":
                    case "*_*":
                        FindAndInvokeButtonFromProcess(process, "No");
                        break;
                    case "Always Ignore?":
                        FindAndInvokeButtonFromProcess(process, "OK");
                        break;
                    case "ASSERT":
                        FindAndInvokeButtonFromProcess(process, "Ignore");
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

        private void FindAndInvokeButtonFromProcess(Process process, string buttonName)
        {
            Console.WriteLine($"Closing: {process.MainWindowTitle}");

            AutomationElement bannerlordRoot = AutomationElement.FromHandle(process.MainWindowHandle);

            PropertyCondition findAssert = new PropertyCondition(AutomationElement.NameProperty, buttonName);
            AutomationElement buttonElement = bannerlordRoot.FindFirst(TreeScope.Descendants, findAssert);

            InvokePattern pattern = buttonElement.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
            pattern.Invoke();
        }
    }
}
