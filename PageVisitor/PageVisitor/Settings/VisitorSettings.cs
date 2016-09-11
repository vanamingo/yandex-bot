using System.Collections.Generic;
using System.Configuration;
using System.Security.Policy;

namespace PageVisitor.Settings
{
    public class VisitorSettings : ConfigurationSection
    {
        [ConfigurationProperty("Request")]
        public string Request
        {
            get { return ((string)(base["Request"])); }
            set { base["Request"] = value; }
        }
        [ConfigurationProperty("OurSite")]
        public string OurSite
        {
            get { return ((string)(base["OurSite"])); }
            set { base["OurSite"] = value; }
        }
        [ConfigurationProperty("Words")]
        public string Words
        {
            get { return ((string)(base["Words"])); }
            set { base["Words"] = value; }
        }
    }
}