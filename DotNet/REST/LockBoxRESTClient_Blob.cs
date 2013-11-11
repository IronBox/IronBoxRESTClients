using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using LockBox;

namespace LockBox
{
    public partial class LockBoxRESTClient
    {
        //---------------------------------------------------------------------
        /// <summary>
        ///     REST action for getting blob info 
        /// </summary>
        /// <param name="Entity">Entity</param>
        /// <param name="EntityType">Entity type</param>
        /// <param name="Password">Password</param>
        /// <param name="ContainerID">Container ID</param>
        /// <param name="State">
        ///     Indicates the state of the blobs to return
        /// </param>
        /// <returns>
        ///     Returns an array of RESTBlobInfo objects
        /// </returns>
        //---------------------------------------------------------------------
        public RESTBlobInfo[] GetEntityContainerBlobInfoListByState(String Entity, LockBoxEntityIDType EntityType, String Password, long ContainerID, LockBoxBlobStatusKey State)
        {
            RESTAction_GetContainerBlobInfoListByState RESTAction = new RESTAction_GetContainerBlobInfoListByState(ServiceBaseUrl);
            // Configure action base settings from REST base
            m_ConfigureRESTActionBase(RESTAction);  

            RESTBlobInfo[] Result = RESTAction.GetContainerBlobInfoListByState(APIVersion, Entity, EntityType, Password, ContainerID, State);
            LastError = RESTAction.LastError;
            return (Result);
        }


        //---------------------------------------------------------------------
        /// <summary>
        ///     REST action for removing an entity container blob
        /// </summary>
        /// <param name="Entity">Entity</param>
        /// <param name="EntityType">Entity type</param>
        /// <param name="Password">Password</param>
        /// <param name="ContainerID">Container ID</param>
        /// <param name="BlobID">BlobID to remove</param>
        /// <returns>
        ///     Returns true on success, false otherwise
        /// </returns>
        //---------------------------------------------------------------------
        public bool RemoveEntityContainerBlob(String Entity, LockBoxEntityIDType EntityType, String Password, long ContainerID, String BlobID)
        {
            RESTAction_RemoveEntityContainerBlob RESTAction = new RESTAction_RemoveEntityContainerBlob(ServiceBaseUrl);
            
            // Configure action base settings from REST base
            m_ConfigureRESTActionBase(RESTAction);  

            bool Result = RESTAction.RemoveEntityContainerBlob(APIVersion, Entity, EntityType, Password, ContainerID, BlobID);
            LastError = RESTAction.LastError;
            return (Result);
        }

        public bool WriteEntityContainerBlob(String Entity, LockBoxEntityIDType EntityType, String Password,
            long ContainerID, String BlobName, Stream BlobData, out String BlobID)
        {
            try
            {
                // Get the container protection key data, either get it from cache or 
                // get it fraom the REST server
                ProtectedContainerKeyData ProtectionKeyData = GetContainerKeyData(Entity, EntityType, Password, ContainerID);

                RESTAction_WriteEntityContainerBlob RESTAction = new RESTAction_WriteEntityContainerBlob(ServiceBaseUrl, ProtectionKeyData);

                // Configure action base settings from REST base
                m_ConfigureRESTActionBase(RESTAction);  

                BlobID = RESTAction.WriteEntityContainerBlob(APIVersion,
                    Entity, EntityType, Password, ContainerID, BlobName, BlobData);
                LastError = RESTAction.LastError;
                return (!String.IsNullOrEmpty(BlobID));
            }
            catch (Exception e)
            {
                BlobID = null;
                LastError = e.Message;
                return (false);
            }
        }


        public Stream ReadEntityContainerBlob(String Entity, LockBoxEntityIDType EntityType, String Password,
            long ContainerID, String BlobID)
        {
            // Get the container protection key data, either get it from cache or 
            // get it fraom the REST server
            try
            {
                ProtectedContainerKeyData ProtectionKeyData = GetContainerKeyData(Entity, EntityType, Password, ContainerID);
                if (ProtectionKeyData == null)
                {
                    throw new Exception("Unable to get container key data");
                }

                RESTAction_ReadEntityContainerBlob RESTAction = new RESTAction_ReadEntityContainerBlob(ServiceBaseUrl, ProtectionKeyData);

                // Configure action base settings from REST base
                m_ConfigureRESTActionBase(RESTAction);  

                Stream Result = RESTAction.ReadEntityContainerBlob(APIVersion, Entity, EntityType, Password, ContainerID, BlobID);
                LastError = RESTAction.LastError;
                return (Result);
            }
            catch (Exception e)
            {
                LastError = e.Message;
                return (null);
            }
        }
    }
}
