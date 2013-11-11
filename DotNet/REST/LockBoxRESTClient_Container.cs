using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LockBox
{
    public partial class LockBoxRESTClient
    {

        public long[] GetContainerIDsFromName(String Entity, LockBoxEntityIDType EntityType, String Password, String ContainerName)
        {
            RESTAction_GetContainerIDsFromName RESTAction = new RESTAction_GetContainerIDsFromName(ServiceBaseUrl);

            // Configure action base settings from REST base
            m_ConfigureRESTActionBase(RESTAction);  

            long[] Result = RESTAction.GetContainerIDsFromName(APIVersion, Entity, EntityType, Password, ContainerName);
            LastError = RESTAction.LastError;
            return (Result);
        }

        public long GetEntityContainerIDByFriendlyID(String Entity, LockBoxEntityIDType EntityType, String Password, String ContainerFriendlyID)
        {
            RESTAction_GetContainerIDFromFriendlyID RESTAction = new RESTAction_GetContainerIDFromFriendlyID(ServiceBaseUrl);

            // Configure action base settings from REST base
            m_ConfigureRESTActionBase(RESTAction);  

            long Result = RESTAction.GetContainerIDFromFriendlyID(APIVersion, Entity, EntityType, Password, ContainerFriendlyID);
            LastError = RESTAction.LastError;
            return (Result);
        }

        public long[] GetEntityContainerIDsByType(String Entity, LockBoxEntityIDType EntityType, String Password, LockBoxContainerProtectionMode[] ContainerTypesToRetrieve)
        {
            RESTAction_EntityContainersIDByType RESTAction = new RESTAction_EntityContainersIDByType(ServiceBaseUrl);

            // Configure action base settings from REST base
            m_ConfigureRESTActionBase(RESTAction);  

            long[] Result = RESTAction.GetEntityContainersIDByType(APIVersion, Entity, EntityType, Password, ContainerTypesToRetrieve);
            LastError = RESTAction.LastError;
            return (Result);
        }


        public bool CreateNewEntityContainer(String Entity, LockBoxEntityIDType EntityType, String Password, EntityContainer ContainerConfig, out long CreatedContainerID)
        {
            RESTAction_CreateEntityContainer RESTAction = new RESTAction_CreateEntityContainer(ServiceBaseUrl);

            // Configure action base settings from REST base
            m_ConfigureRESTActionBase(RESTAction);  

            CreatedContainerID = RESTAction.CreateEntityContainer(APIVersion, Entity, EntityType, Password, ContainerConfig);
            LastError = RESTAction.LastError;
            return (CreatedContainerID != -1);
        }

        public bool SetEntityContainerEnabled(String Entity, LockBoxEntityIDType EntityType, String Password, long ContainerID, bool IsEnabled)
        {
            RESTAction_SetEntityContainerEnabled RESTAction = new RESTAction_SetEntityContainerEnabled(ServiceBaseUrl);

            // Configure action base settings from REST base
            m_ConfigureRESTActionBase(RESTAction);  

            bool Result = RESTAction.SetEntityContainerEnabled(APIVersion, Entity, EntityType, Password, ContainerID, IsEnabled);
            LastError = RESTAction.LastError;
            return (Result);
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Removes an entity container
        /// </summary>
        /// <param name="Entity">Entity</param>
        /// <param name="EntityType">Entity type</param>
        /// <param name="Password">Password</param>
        /// <param name="ContainerID">Container ID</param>
        /// <returns>
        ///     Returns true on success, false otherwise
        /// </returns>
        //---------------------------------------------------------------------
        public bool RemoveEntityContainer(String Entity, LockBoxEntityIDType EntityType, String Password, long ContainerID)
        {
            RESTAction_RemoveEntityContainer RESTAction = new RESTAction_RemoveEntityContainer(ServiceBaseUrl);

            // Configure action base settings from REST base
            m_ConfigureRESTActionBase(RESTAction);  

            bool Result = RESTAction.RemoveEntityContainer(APIVersion, Entity, EntityType, Password, ContainerID);
            LastError = RESTAction.LastError;
            return(Result);
        }


        public ProtectedContainerKeyData GetContainerKeyData(String Entity, LockBoxEntityIDType EntityType, String Password, long ContainerID)
        {
            // Read from cache, if enabled and we have an entry then this will return
            // a non-null result
            ProtectedContainerKeyData Result = m_Cache_GetContainerKeyData(ContainerID);
            if (Result != null)
            {   
                return (Result);
            }
            
            // Not in cache, so call REST service for container key data
            RESTAction_ContainerKeyData RESTAction = new RESTAction_ContainerKeyData(ServiceBaseUrl);

            // Configure action base settings from REST base
            m_ConfigureRESTActionBase(RESTAction);  

            RESTContainerKeyData RESTResult = RESTAction.GetContainerKeyData(APIVersion, Entity, EntityType, Password, ContainerID);
            if (RESTResult != null)
            {
                
                // Init the result and save in cache, if caching disabled then it will nop
                Result = new ProtectedContainerKeyData(RESTResult.GetSymmetricKey(), RESTResult.GetSymmetricIV(), RESTResult.GetSymmetricKeyStrength());
                m_Cache_AddContainerKeyData(ContainerID,Result);
            }
            LastError = RESTAction.LastError;

            // Done, return the result
            return (Result);
        }
    }
}
