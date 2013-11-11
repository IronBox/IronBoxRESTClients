using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LockBox.Common;
using LockBox.Common.Collections.Data;

namespace LockBox
{
    public class LockBoxContainerAPIAccessDataCollection 
    {

        
        private List<LockBoxContainerAPIAccessData> m_APIExtendedAccessObjects = null;
        
        public LockBoxContainerAPIAccessDataCollection()            
        {
            Reset();
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Accessor for the API list collection
        /// </summary>
        //---------------------------------------------------------------------
        public List<LockBoxContainerAPIAccessData> APIItems
        {
            get
            {
                return (m_APIExtendedAccessObjects);
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Resets this extended access data collection
        /// </summary>
        //---------------------------------------------------------------------
        public void Reset()
        {
            if (m_APIExtendedAccessObjects == null)
            {
                m_APIExtendedAccessObjects = new List<LockBoxContainerAPIAccessData>();
            }
            m_APIExtendedAccessObjects.Clear();
        }

        

        //---------------------------------------------------------------------
        /// <summary>
        ///     Gets the API access object with the given API user name
        /// </summary>
        /// <param name="APIUserName"></param>
        /// <returns>
        ///     Returns the API access object with the specified API 
        ///     user name, null otherwise
        /// </returns>
        //---------------------------------------------------------------------
        public LockBoxContainerAPIAccessData GetAPIAccessObject(String APIUserName)
        {
            int Dummy;
            return (m_GetAPIAccessObject(APIUserName, out Dummy));
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Gets an API access object by access user name, and the 
        ///     index within this collection
        /// </summary>
        /// <param name="APIUserName">Access user name to lookup</param>
        /// <param name="index">Index of the object, -1 on error</param>
        /// <returns>
        ///     Returns an API access object on success, null otherwise
        /// </returns>
        //---------------------------------------------------------------------
        private LockBoxContainerAPIAccessData m_GetAPIAccessObject(String APIUserName, out int index)
        {
            index = -1;
            for (int i = 0; i < m_APIExtendedAccessObjects.Count; i++)
            {
                if (m_APIExtendedAccessObjects[i].APIUserName == APIUserName)
                {
                    // Found the match
                    index = i;
                    return (m_APIExtendedAccessObjects[i]);
                }
            }
            // No match found
            return (null);
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Indicates if an API access object exists in this collection
        /// </summary>
        /// <param name="AccessUserName">Access user name to look up</param>
        /// <returns>Returns true on success, false otherwise</returns>
        //---------------------------------------------------------------------
        public bool APIAccessObjectExists(String AccessUserName)
        {
            return (GetAPIAccessObject(AccessUserName) != null);
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Removes an API access object by Acess Username
        /// </summary>
        /// <param name="AccessUserName">Access user name</param>
        /// <returns>
        ///     Returns true on success, false otherwise
        /// </returns>
        //---------------------------------------------------------------------
        public bool RemoveAPIAcessObject(String AccessUserName)
        {
            int index;
            m_GetAPIAccessObject(AccessUserName, out index);
            bool Result = false;
            if (index != -1)
            {
                Result = true;
                m_APIExtendedAccessObjects.RemoveAt(index);
            }
            return (Result);
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Adds an API access object
        /// </summary>
        /// <param name="APIAccessObj">Object to add</param>
        /// <returns>
        ///     Returns true on success, false otherwise
        /// </returns>
        //---------------------------------------------------------------------
        public bool AddAPIAccessObject(LockBoxContainerAPIAccessData APIAccessObj)
        {
            // Input validation
            if ((APIAccessObj == null) || String.IsNullOrEmpty(APIAccessObj.APIUserName))
            {
                LockBoxDebugHelper.Debug_Log("AddAPIAccessObject", "Invalid API access config object", true);
                return (false);
            }

            // Make sure an object with the same AccessUserName does not exist
            if (APIAccessObjectExists(APIAccessObj.APIUserName))
            {
                LockBoxDebugHelper.Debug_Log("AddAPIAccessObject", "API access user name already exists", true);
                return (false);
            }

            // Passed all tests, go ahead and add
            m_APIExtendedAccessObjects.Add(APIAccessObj);
            return (true);
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Returns all the API access objects as an array
        /// </summary>
        /// <returns>
        ///     Array of API access objects
        /// </returns>
        //---------------------------------------------------------------------
        public LockBoxContainerAPIAccessData[] GetAllAPIAccessObjects()
        {
            return (m_APIExtendedAccessObjects.ToArray());
        }
    }
}
