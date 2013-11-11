using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel;
using System.Security.Cryptography;
using LockBox.Common.IO;

namespace LockBox
{
    public partial class LockBoxRESTClient
    {
        private bool m_EnableCaching = false;
        private MemoryProtectionScope m_MemoryProtectionScope = MemoryProtectionScope.SameProcess;

        //---------------------------------------------------------------------
        /// <summary>
        ///     Sets or gets flag indicating whether or not to cache data 
        ///     retrieved
        /// </summary>
        //---------------------------------------------------------------------
        [DefaultValue(false)]
        public bool EnableCaching
        {
            set
            {
                m_EnableCaching = value;
                if (!m_EnableCaching)
                {
                    m_DropAllCachedData();
                }
            }
            
            get
            {
                return (m_EnableCaching);
            }
        }


        private void m_ProtectByteBuffer(byte[] ThisBuffer)
        {
            ProtectedMemory.Protect(ThisBuffer, m_MemoryProtectionScope);
        }

        private void m_UnprotectByteBuffer(byte[] ThisBuffer)
        {
            ProtectedMemory.Unprotect(ThisBuffer, m_MemoryProtectionScope);
        }



        #region CONTAINER_SESSION_KEYS

        private Dictionary<long, ProtectedContainerKeyData> m_ContainerKeyDataCache = null;

        private void m_Cache_AddContainerKeyData(long ContainerID, ProtectedContainerKeyData ProtectedContainerKeyData)
        {
            if (EnableCaching && (ProtectedContainerKeyData != null))
            {
                // Create a protected container key and store values there
                m_ContainerKeyDataCache[ContainerID] = ProtectedContainerKeyData;
            }
        }

        private ProtectedContainerKeyData m_Cache_GetContainerKeyData(long ContainerID)
        {
            ProtectedContainerKeyData Result = null;
            if (EnableCaching && m_ContainerKeyDataCache.ContainsKey(ContainerID))
            {
                Result = m_ContainerKeyDataCache[ContainerID];
            }
            return (Result);
        }


        /*
        //---------------------------------------------------------------------
        //  Container session key caching
        //---------------------------------------------------------------------
        
        private Dictionary<long, byte[]> m_ContainerSessionKeyCache = null;

        //---------------------------------------------------------------------
        /// <summary>
        ///     Caches and protects a given container session key if 
        ///     caching enabling is enabled
        /// </summary>
        /// <param name="ContainerID">Container ID</param>
        /// <param name="ContainerSessionKey">Container session key</param>
        //---------------------------------------------------------------------
        private void m_Cache_AddContainerSessionKey(long ContainerID, byte[] ContainerSessionKey)
        {
            if (EnableCaching && (ContainerSessionKey != null))
            {
                // Copy the buffer
                byte[] ContainerSessionKeyCopy = StreamHelper.CopyBuffer(ContainerSessionKey);
                m_ProtectByteBuffer(ContainerSessionKeyCopy);
                m_ContainerSessionKeyCache[ContainerID] = ContainerSessionKeyCopy;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Retrieves and unprotects a container session key from cache
        ///     if it exists in the cache
        /// </summary>
        /// <param name="ContainerID">Container ID</param>
        /// <returns>
        ///     Returns the cached container session key, otherwise
        ///     null if the container session key does not exist or 
        ///     caching is not enabled
        /// </returns>
        //---------------------------------------------------------------------
        private byte[] m_Cache_GetContainerSessionKey(long ContainerID)
        {
            byte[] Result = null;
            if (EnableCaching && m_ContainerSessionKeyCache.ContainsKey(ContainerID))
            {
                Result = m_ContainerSessionKeyCache[ContainerID];
                m_UnprotectByteBuffer(Result);
            }
            return (Result);
        }
         */

        #endregion

        //---------------------------------------------------------------------
        /// <summary>
        ///     Drops all cached data
        /// </summary>
        //---------------------------------------------------------------------
        private void m_DropAllCachedData()
        {
            m_ContainerKeyDataCache.Clear();

            //m_ContainerSessionKeyCache.Clear();
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Initializes all cache structures
        /// </summary>
        //---------------------------------------------------------------------
        private void m_InitCaches()
        {
            m_ContainerKeyDataCache = new Dictionary<long, ProtectedContainerKeyData>();

            //m_ContainerSessionKeyCache = new Dictionary<long, byte[]>();
        }
    }
}
