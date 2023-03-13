using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Automation;

namespace AssertAutoIgnore
{
    internal static class Program
    {
        private const string name = "Bannerlord Assert Auto Ignore - by poheniks";

        [STAThread]
        static void Main()
        {
            Console.Title = name;

            Console.WriteLine("Starting Assert Auto Ignore");
            Console.WriteLine("Looking for Bannerlord Process...");

            AssertChecker assertChecker = new AssertChecker();
            while (true)
            {
                assertChecker.OnTick();
            }
        }
    }


}
