using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LockBox
{
    public partial class LockBoxRESTClient
    {
        public byte[] GetEntityPublicKeyMaterial(String Entity, LockBoxEntityIDType EntityType, LockBoxEntityAsymmetricKey PublicKeyToRetrieve)
        {
            RESTAction_EntityPublicKeyData RESTAction = new RESTAction_EntityPublicKeyData(ServiceBaseUrl);

            // Configure action base settings from REST base
            m_ConfigureRESTActionBase(RESTAction);  

            byte[] Result = RESTAction.GetEntityPublicKeyData(APIVersion, Entity, EntityType, PublicKeyToRetrieve);
            LastError = RESTAction.LastError;
            return (Result);
        }


        public byte[] GetEntityPrivateKeyMaterial(String Entity, LockBoxEntityIDType EntityType, String Password, LockBoxEntityAsymmetricKey PrivateKey)
        {

            RESTAction_EntityPrivateKeyData RESTAction = new RESTAction_EntityPrivateKeyData(ServiceBaseUrl);
            // Configure action base settings from REST base
            m_ConfigureRESTActionBase(RESTAction);  

            byte[] Result = RESTAction.GetEntityPrivateKeyData(APIVersion, Entity, EntityType, Password, PrivateKey);
            if ((Result != null) && EnableCaching)
            {
                // Cache the public key in a protected state
            }
            LastError = RESTAction.LastError;
            return (Result);
        }



        
    }
}
