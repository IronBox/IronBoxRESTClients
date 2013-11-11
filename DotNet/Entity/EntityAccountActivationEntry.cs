using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LockBox
{
    public class EntityAccountActivationEntry
    {
        public long ActivationID { set; get; }
        public String ActivationCode { set; get; }
        public DateTime DateCreatedUtc { set; get; }
        public String Email { set; get; }
        public EntityAccountActivationType ActivationType { set; get; }

        public Dictionary<EntityAccountActivationProperty, String> ActivationProperties = null;

        //---------------------------------------------------------------------
        /// <summary>
        ///     Constructor
        /// </summary>
        //---------------------------------------------------------------------
        public EntityAccountActivationEntry()
        {
            ActivationProperties = new Dictionary<EntityAccountActivationProperty, string>();
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Indicates if the account activation entry is expired or not given 
        ///     the number of days that entry is allowed to exist
        /// </summary>
        /// <param name="DaysToLive"></param>
        /// <returns></returns>
        //---------------------------------------------------------------------
        public bool IsExpired(double DaysToLive)
        {
            return (DateTime.Compare(DateTime.UtcNow, DateCreatedUtc.AddDays(DaysToLive)) > 0);
        }


        public bool ContainsActivationProperty(EntityAccountActivationProperty PropertyKey)
        {
            return (ActivationProperties.ContainsKey(PropertyKey));
        }

        public bool GetActivationPropertyAsBool(EntityAccountActivationProperty PropertyKey)
        {
            bool Parsed = false;
            Boolean.TryParse(GetActivationPropertyAsString(PropertyKey), out Parsed);
            return (Parsed);
        }

        public String GetActivationPropertyAsString(EntityAccountActivationProperty PropertyKey)
        {
            try
            {
                return (ActivationProperties[PropertyKey]);
            }
            catch (Exception)
            {
                return (null);
            }
        }
    }
}
