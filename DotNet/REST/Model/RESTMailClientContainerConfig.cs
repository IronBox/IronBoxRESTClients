using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.Net.Http.Formatting;
using RestSharp;
using System.Reflection;
using LockBox.Common;
using LockBox.Common.Data;

namespace LockBox
{
    public class RESTMailClientContainerConfig
    {
        // Input parameters
        public String Name { set; get; }
        public String Description { set; get; }
        
        public String ContainerType { set; get; }
        public String IronBoxMemberEmailsCSV { set; get; }
        public String UseEasyAccess { set; get; }
        public String EasyAccessPassword { set; get; }
        
        // Response parameters
        public String EasyAccessUrl { set; get; }
        public String NumExpirationDays { set; get; }
        public String FriendlyID { set; get; }
        public String ContainerID { set; get; }


        public void SetContainerID(long ID)
        {
            ContainerID = ID.ToString();
        }

        public long GetContainerID()
        {
            long Result = -1;
            if (!Int64.TryParse(ContainerID, out Result))
            {
                Result = -1;
            }
            return (Result);
        }


        public double GetNumExpirationDays()
        {
            double Result = -1;
            if (!Double.TryParse(NumExpirationDays, out Result))
            {
                Result = -1;
            }
            return (Result);
        }

        public void SetNumExpirationDays(double NumDays)
        {
            NumExpirationDays = NumDays.ToString();
        }


        public bool GetUseEasyAccess()
        {
            bool Result = false;
            if (!Boolean.TryParse(UseEasyAccess, out Result))
            {
                Result = false;
            }
            return (Result);
        }

        public void SetUseEasyAccess(bool Enable)
        {
            UseEasyAccess = Enable ? Boolean.TrueString : Boolean.FalseString;
        }

        public String[] GetIronBoxMemberEmails()
        {
            List<String> Result = new List<string>();
            if (!String.IsNullOrEmpty(IronBoxMemberEmailsCSV))
            {
                String[] TempResults = null;
                if (StringHelper.ParseCSVString(IronBoxMemberEmailsCSV, out TempResults))
                {
                    // Iterate and add
                    foreach (String s in TempResults)
                    {
                        if (!String.IsNullOrEmpty(s))
                        {
                            Result.Add(DataTransformer.NormalizeEmail(s));
                        }
                    }
                }
            }
            return (Result.ToArray());
        }


        //---------------------------------------------------------------------
        /// <summary>
        ///     Sets the container type
        /// </summary>
        /// <param name="ThisContainerType"></param>
        //---------------------------------------------------------------------
        public void SetContainerType(RESTMailClientContainerType ThisContainerType)
        {
            ContainerType = ThisContainerType.ToString();
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Gets the container type
        /// </summary>
        /// <returns></returns>
        //---------------------------------------------------------------------
        public RESTMailClientContainerType GetContainerType()
        {
            RESTMailClientContainerType Result = RESTMailClientContainerType.ReceiveOnly;
            if (!String.IsNullOrEmpty(ContainerType))
            {
                Result = (RESTMailClientContainerType)Enum.Parse(typeof(RESTMailClientContainerType), ContainerType);
            }
            return (Result);
        }



        //---------------------------------------------------------------------
        /// <summary>
        ///     Loads this object into a REST request
        /// </summary>
        /// <param name="ThisRequestObj"></param>
        //---------------------------------------------------------------------
        public void LoadIntoRestRequest(RestRequest ThisRequestObj)
        {
            

            // Input validation
            if (ThisRequestObj == null)
            {
                throw new ArgumentNullException("LoadIntoFormDataCollection: Rest request was null");
            }

            

            foreach (var prop in this.GetType().GetProperties())
            {
                //LockBoxDebugHelper.DoLogToFile(String.Format("Adding: {0}={1}", prop.Name, prop.GetValue(this, null)));
                ThisRequestObj.AddParameter(prop.Name, prop.GetValue(this, null));
            }

            /*
            LockBoxDebugHelper.DoLogToFile("Here2");

            var body = ThisRequestObj.Parameters.Where(p => p.Type == ParameterType.RequestBody).FirstOrDefault();
            if (body != null)
            {
                LockBoxDebugHelper.DoLogToFile(body.Value.ToString());
                //Debug.WriteLine("CurrentBody={0}", body.Value);

            }
             */
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Loads information from a form collection into this object
        /// </summary>
        /// <param name="ThisCollection"></param>
        /// <returns></returns>
        //---------------------------------------------------------------------
        public bool LoadFromFormDataCollection(FormDataCollection ThisCollection)
        {
            try
            {
                // Input validation
                if (ThisCollection == null)
                {
                    throw new ArgumentNullException("LoadFromFormDataCollection: Form data was null");
                }

                foreach (KeyValuePair<String, String> kvp in ThisCollection)
                {
                    // Use reflection to see if this object has a property with the current key name, 
                    // and if so set the value of this object's property to the one in the form data
                    PropertyInfo prop = this.GetType().GetProperty(kvp.Key, BindingFlags.Public | BindingFlags.Instance);
                    if (null != prop && prop.CanWrite)
                    {
                        prop.SetValue(this, kvp.Value, null);
                    }
                }

                // Done, return true
                return (true);
            }
            catch (Exception e)
            {
                LockBoxDebugHelper.DoLogToFile(e.ToString());
                return (false);
            }
        }



        
    }
}
