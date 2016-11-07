using System.Configuration;

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
        [ConfigurationProperty("Queries")]
        public QueriesCollection Queries
        {
            get { return ((QueriesCollection)(base["Queries"])); }
            set { base["Queries"] = value; }
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
            set { ; }
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
    }
}