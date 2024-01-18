using System.IO;
using System.Collections.Generic;
using System.Timers;
using System.Text;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace AIAnalysisProject
{
    public class AIAnalysis
    {

        public Timer AI_AnalysisTimer;
        public string[] fileNamePaths = { @"C:\IVLImageRepo\Images\20190723123128.png", @"C:\IVLImageRepo\Images\20190723130616.png" };
        public string[] AI_Impressions = { "Referable", "Non-Referable", "Gradable", "Non-Gradable" };
        public int patientID = 2;
        public int visitID = 1;
        private AIAnalysis localAnalysis;
        private List<FileInfo> fileInfos;
        private bool isBusy = false;

        public AIAnalysis()
        {
            AI_AnalysisTimer = new Timer();
            AI_AnalysisTimer.Interval = 10000;
            AI_AnalysisTimer.Elapsed += AI_AnalysisTimer_Elapsed;
            fileInfos = new List<FileInfo>();
            //CreateJsonFile();
        }


        /// <summary>
        /// Read and Move the json file whenever the timer elapsed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AI_AnalysisTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!isBusy)
            {
                isBusy = true;
                FileInfo[] mydir = new DirectoryInfo(@"D:\Inbox").GetFiles("*.json");
                if (!fileInfos.Any())
                    fileInfos = mydir.ToList();
                else
                {
                    foreach (var item in mydir)
                    {
                        if (!fileInfos.Where(x => x.Name == item.Name).ToList().Any())
                            fileInfos.Add(item);

                    }
                }

                foreach (var item in fileInfos)
                {
                    string jsonText = File.ReadAllText(item.FullName);
                    localAnalysis = (AIAnalysis)JsonConvert.DeserializeObject(jsonText, typeof(AIAnalysis));
                    string readAnalysisFile = JsonConvert.SerializeObject(localAnalysis);
                    File.Move(item.FullName, " " + item.Name ) ;
                    fileInfos.Remove(item);
                }
                isBusy = false;
            }
            
        }

        public void CreateJsonFile(AIAnalysis aiAnalysis)
        {
            string jsonString = JsonConvert.SerializeObject(aiAnalysis);
            File.WriteAllText(@"D:\Inbox\AiDetails.json", jsonString);
        }
    }
}
