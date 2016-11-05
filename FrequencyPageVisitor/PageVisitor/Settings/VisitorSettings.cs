using System.Collections.Generic;
using System.Configuration;
using System.Security.Policy;

namespace PageVisitor.Settings
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
        [ConfigurationProperty("Запросы")]
        public QueriesCollection Queries
        {
            get { return ((QueriesCollection)(base["Запросы"])); }
            set { base["Запросы"] = value; }
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
        }
    }

    public class QueryElement : ConfigurationElement
    {
        [ConfigurationProperty("Запрос")]
        public string Query
        {
            get { return ((string)(base["Запрос"])); }
            set { base["Запрос"] = value; }
        }

        [ConfigurationProperty("Запрос")]
        public string Frequency
        {
            get { return ((string)(base["Запрос"])); }
            set { base["Запрос"] = value; }
        }

    }
}