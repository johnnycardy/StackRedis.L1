using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace StackRedis.L1.Notifications
{
    public static class ProcessId
    {
        private static readonly object _processIdLock = new object();
        private static string _processId;

        /// <summary>
        /// Returns the current machine unique ID and the process.
        /// </summary>
        public static string GetCurrent()
        {
            if (string.IsNullOrEmpty(_processId))
            {
                lock (_processIdLock)
                {
                    if (string.IsNullOrEmpty(_processId))
                    {
                        string result = NetworkInterface.GetAllNetworkInterfaces()
                                        .Where(nic => nic.OperationalStatus == OperationalStatus.Up)
                                        .Select(nic => nic.GetPhysicalAddress().ToString()).FirstOrDefault();

                        if (string.IsNullOrEmpty(result))
                            result = Environment.MachineName;

                        _processId = result + Process.GetCurrentProcess().Id.ToString();
                    }
                }
            }

            return _processId;
        }
    }
}
