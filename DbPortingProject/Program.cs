using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Resources;
using System.Globalization;
using NLog;
using NLog.Targets;
using System.IO;
using Microsoft.Win32;
using System.Runtime.InteropServices;
//using AdminAdditionOperation;

namespace DBPorting
{
    static class Program
    {
        [DllImport("user32.dll")]
        private static extern Boolean ShowWindow(IntPtr hWnd, Int32 nCmdShow);
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        static RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Intusoft\\");

        static Logger Exception_Log = LogManager.GetLogger("ExceptionLog");
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                if (registryKey != null)
                    IVLVariables.appDirPathName = (string)registryKey.GetValue("AppDataPath");

                if (string.IsNullOrEmpty(IVLVariables.appDirPathName))
                    IVLVariables.appDirPathName = new FileInfo(Application.ExecutablePath).Directory.FullName + Path.DirectorySeparatorChar;
                else if (Directory.Exists(IVLVariables.appDirPathName))
                    IVLVariables.appDirPathName = IVLVariables.appDirPathName + Path.DirectorySeparatorChar;// new FileInfo(Application.ExecutablePath).Directory.FullName + Path.DirectorySeparatorChar;
                else
                    IVLVariables.appDirPathName = new FileInfo(Application.ExecutablePath).Directory.FullName + Path.DirectorySeparatorChar;

                LogManager.Configuration.Variables["dir"] = IVLVariables.appDirPathName;
                LogManager.Configuration.Variables["dir2"] = DateTime.Now.ToString("yyyy-MM-dd");
                LogManager.Configuration.Variables["dir3"] = DateTime.Now.ToString("HH:mm:ss");

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new DbTransferForm());
            }
            catch (Exception ex)
            {

                Common.ExceptionLogWriter.WriteLog(ex, Exception_Log);

            }
          
            //InitializationOfResourceStrings();
            //Application.Run(new AdminAdditionOperation.Forms.MainForm());
        }
    }
}
