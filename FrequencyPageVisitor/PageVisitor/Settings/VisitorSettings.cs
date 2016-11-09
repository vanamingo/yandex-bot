using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace FrequencyPageVisitor.Settings
{
    public class VisitorSettings : ConfigurationSection
    {
        [ConfigurationProperty("WriteLogs")]
        public bool WriteLogs
        {
            get
            {
                return (bool)base["WriteLogs"]; 
                
            }
            set
            {
                base["WriteLogs"] = value.ToString(); 
                
            }
        }

        [ConfigurationProperty("DelayInSeconds")]
        public int DelayInSeconds
        {
            get { return (int)base["DelayInSeconds"]; }
            set { base["DelayInSeconds"] = value; }
        }
        
        [ConfigurationProperty("RivalsOnPage")]
        public int RivalsOnPage
        {
            get { return (int)base["RivalsOnPage"]; }
            set { base["RivalsOnPage"] = value; }
        }

        [ConfigurationProperty("DeserializeMode")]
        public bool DeserializeMode
        {
            get
            {
                return (bool)base["DeserializeMode"];

            }
            set
            {
                base["DeserializeMode"] = value.ToString();

            }
        }

        [ConfigurationProperty("MakeScreenshots")]
        public bool MakeScreenshots
        {
            get
            {
                return (bool)base["MakeScreenshots"];

            }
            set
            {
                base["MakeScreenshots"] = value.ToString();

            }
        }

        [ConfigurationProperty("NewBrowserForQuery")]
        public bool NewBrowserForQuery
        {
            get
            {
                return (bool)base["NewBrowserForQuery"];

            }
            set
            {
                base["NewBrowserForQuery"] = value.ToString();

            }
        }

        [ConfigurationProperty("Queries")]
        public QueriesCollection Queries
        {
            get { return ((QueriesCollection)(base["Queries"])); }
            set { base["Queries"] = value; }
        }

        public List<QueryElement> QueriesList
        {
            get
            {
                return Queries.Cast<QueryElement>().ToList();
            }
        }
    }

    [ConfigurationCollection(typeof(QueryElement))]
    public class QueriesCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new QueryElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((QueryElement)(element)).Query;
        }

        public QueryElement this[int idx]
        {
            get { return (QueryElement)BaseGet(idx); }
            set
            {
                var b = value ;
                b.Query = b.Query;
            }
        }
    }

    public class QueryElement : ConfigurationElement
    {
        public QueryElement()
        {
        }

        [ConfigurationProperty("Query")]
        public string Query
        {
            get { return ((string)(base["Query"])); }
            set { base["Query"] = value; }
        }

        [ConfigurationProperty("Frequency")]
        public string Frequency
        {
            get { return ((string)(base["Frequency"])); }
            set { base["Frequency"] = value; }
        }

        [ConfigurationProperty("Group")]
        public string GroupRaw
        {
            get { return ((string)(base["Group"])); }
            set { base["Group"] = value; }
        }

        public List<string> Group
        {
            get
            {
                if (string.IsNullOrEmpty(GroupRaw))
                {
                    return new List<string>();
                }

                return GroupRaw.Split('/').ToList();
            }
        }
    }
}