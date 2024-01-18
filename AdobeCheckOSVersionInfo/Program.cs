using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using Microsoft.VisualBasic.Devices;
using System.IO;
using Newtonsoft.Json;
using Common;
using System.Management;

namespace AdobeCheckOSVersionInfo
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                StreamWriter st = new StreamWriter("AdobeOsInfo.json", false);
                AdobeAndOSInfo info = new AdobeAndOSInfo();
                info.IsAdobeInstalled = isAdobeInstallationCheck();
                try
                {
                     info.OSInfo = GetOSVersionInfo();
                }
                catch (Exception)
                {
                    info.OSInfo = "Windows 11";
                }
                string jsonValue = JsonConvert.SerializeObject(info);
                st.Write(jsonValue);
                st.Flush();
                st.Close();
                st.Dispose();
                Console.WriteLine(jsonValue);
                //Console.ReadLine();
            }
            catch (Exception ex)
            {

                StringBuilder stringBuilder = new StringBuilder();

                while (ex != null) // to get all the exception
                {
                    stringBuilder.AppendLine(ex.Message);
                    stringBuilder.AppendLine(ex.StackTrace);
                    ex = ex.InnerException;
                }
                string excepStr = stringBuilder.ToString();
                Console.WriteLine(excepStr); //returning the exception string
            }
           
        }

        private static bool isAdobeInstallationCheck()
        {
            RegistryKey adobe = Registry.LocalMachine.OpenSubKey("Software").OpenSubKey("Adobe");
            Console.WriteLine(adobe);
            if (adobe != null)
            {
                RegistryKey acroRead = adobe.OpenSubKey("Acrobat Reader");
                Console.WriteLine(acroRead);

                if (acroRead != null)
                {
                    return true;
                }
                else
                {
                    Console.WriteLine("Reader is null");
                    return false;

                }
            }
            else
            {
                Console.WriteLine("Adobe is null");
                return false;
            }
        }

        private static string GetOSVersionInfo()
        {
            string osVersion = string.Empty;
            using (var objOS = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem"))
            {
                foreach (ManagementObject objMgmt in objOS.Get())
                {
                    osVersion = $"{objMgmt.Properties["Caption"].Value}";
                    Console.WriteLine("{0}", objMgmt.Properties["Caption"].Value);
                }
            }
            return osVersion;
        }
    }

}
