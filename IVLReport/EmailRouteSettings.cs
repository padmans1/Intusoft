using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace IVLReport
{
    [Serializable]
    public class EmailRouteSettings
    {
        public string fromaddr = "intusoft_emailer@intuvisionlabs.com";
        public string password = "IntusoftEmailer123";
        public string hostName = "mail.intuvisionlabs.com";
        public int hostPort = 587;

        private static EmailRouteSettings _instance;
        public static EmailRouteSettings GetEmailRouteValues()
        {
            if (_instance == null)
                if (File.Exists(@"emailRouteSettings.json"))
                {
                    var routeString = File.ReadAllText(@"emailRouteSettings.json");
                    try
                    {
                        _instance = JsonConvert.DeserializeObject<EmailRouteSettings>(routeString);

                    }
                    catch (Exception)
                    {
                        createNewInstance();
                    }

                }
                else
                    createNewInstance();
            return _instance; 
        }
        private static void createNewInstance()
        {
            _instance = new EmailRouteSettings();
            var routeString = JsonConvert.SerializeObject(_instance);
            File.WriteAllText(@"emailRouteSettings.json", routeString);
        }
    }
}
