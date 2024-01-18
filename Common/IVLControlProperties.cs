using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class IVLControlProperties
    {
        private static IVLControlProperties _controlProperties;
        public double min = 0.0;
        public double max = 200000.0;
        public string val = string.Empty;
        public string name = string.Empty;
        public string type = string.Empty;
        public string control = string.Empty;
        public string text = string.Empty;
        public string range = "";
        public int length = 40;

        public static IVLControlProperties createInstance()
        {
            return _controlProperties = new IVLControlProperties();
        }
        public static IVLControlProperties GetInstance()
        {
            return _controlProperties;
        }
        public IVLControlProperties()
        {
            range = min.ToString() + "to" + max.ToString();

        }
    }
}
