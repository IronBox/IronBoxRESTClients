using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Configuration;

namespace LockBox
{
    //-------------------------------------------------------------------------
    /// <summary>
    ///     Defines web.config/app.config "lockbox" file sections
    /// </summary>
    //-------------------------------------------------------------------------
    public class LockBoxSection : ConfigurationSection
    {
        public static LockBoxSection Current
        {
            get { return (LockBoxSection)ConfigurationManager.GetSection("lockbox-client"); }
        }


        //---------------------------------------------------------------------
        /// <summary>
        ///     API username
        /// </summary>
        //---------------------------------------------------------------------
        [ConfigurationProperty("api-username", IsRequired = false)]
        public string APIUserName
        {
            get { return (string)base["api-username"]; }
            set { base["api-username"] = value; }
        }



        //---------------------------------------------------------------------
        /// <summary>
        ///     API key
        /// </summary>
        //---------------------------------------------------------------------
        [ConfigurationProperty("api-key", IsRequired = false)]
        //[StringValidator(MinLength=32, MaxLength=32)]
        public string APIKey
        {
            get { return (string)base["api-key"]; }
            set { base["api-key"] = value; }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Lockbox RESTServerURL
        /// </summary>
        //---------------------------------------------------------------------
        [ConfigurationProperty("entity-manager-rest-servicebus-url", IsRequired = false)]
        public string EntityManagerRESTServiceBusURL
        {
            get { return (string)base["entity-manager-rest-servicebus-url"]; }
            set { base["entity-manager-rest-servicebus-url"] = value; }
        }


        //---------------------------------------------------------------------
        /// <summary>
        /// LockBox user email 
        /// Issue #BB239 - Add support for user email in lockbox config section
        /// </summary>
        //---------------------------------------------------------------------
        [ConfigurationProperty("user-email", IsRequired = false)]
        public string UserEmail
        {
            get { return (string)base["user-email"]; }
            set { base["user-email"] = value; }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// LockBox user password 
        /// Issue #BB239 - Add support for user email in lockbox config section
        /// </summary>
        //---------------------------------------------------------------------
        [ConfigurationProperty("user-password", IsRequired = false)]
        public string UserPassword
        {
            get { return (string)base["user-password"]; }
            set { base["user-password"] = value; }
        }
    }
}
