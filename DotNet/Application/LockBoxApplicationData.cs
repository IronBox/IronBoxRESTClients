using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LockBox.Common.Data;

namespace LockBox
{
    public class LockBoxApplicationData
    {
        public long ApplicationID { set; get; }
        public String Name { set; get; }
        public String Description { set; get; }
        public String APIUserName { set; get; }
        public String APIKey { set; get; }

        public Dictionary<long, LockBoxApplicationContextData> Contexts = null;


        public LockBoxApplicationData()
        {
            Contexts = new Dictionary<long, LockBoxApplicationContextData>();
        }


        public void CreateNewAPIKey()
        {
            APIKey = DataGenerator.CreateAPIKey();
        }


        public bool ContextExists(String ContextValue)
        {
            return (GetContext(ContextValue) != null);            
        }

        public LockBoxApplicationContextData GetContext(String ContextValue)
        {
            String ContextToUse = LockBoxApplicationContextData.NormalizeContext(ContextValue);
            if ((Contexts != null) && !String.IsNullOrEmpty(ContextToUse))
            {
                foreach (KeyValuePair<long, LockBoxApplicationContextData> kvp in Contexts)
                {
                    if (kvp.Value.Value == ContextToUse)
                    {
                        return (Contexts[kvp.Key]);
                    }
                }
            }
            // Didn't find the correct context object
            return (null);
        }

    }
}
