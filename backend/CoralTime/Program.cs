using System;
using Microsoft.AspNetCore.Hosting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace CoralTime
{
    public class Program
    {
        [DllImport("Kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("User32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int cmdShow);

        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .ConfigureStartup<StartupConfigurationService>()
                .Build();

#if DEBUG
            // Hide Kestrel console.
            var hWnd = GetConsoleWindow();
            if (hWnd != IntPtr.Zero)
            {
                ShowWindow(hWnd, 0);
            }

            // Run application at browser tab instead of new window.
            Process.Start(new ProcessStartInfo("cmd", "/c start http://localhost:5000"));
#endif
            host.Run();
        }
    }
}
