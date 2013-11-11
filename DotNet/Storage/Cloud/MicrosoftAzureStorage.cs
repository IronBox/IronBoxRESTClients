using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure;
using System.Collections.Specialized;
using LockBox.Common.Security.Cryptography;
using LockBox.Common.IO.Compression;
using LockBox.Common;
using System.Diagnostics;
using System.Security.Cryptography;

namespace LockBox
{
    public partial class MicrosoftAzureStorage : ILockBoxStorage
    {

        private CloudBlobClient GetBlobClient()
        {
            ClearLastError();

            // Input validation
            try
            {
                CloudBlobClient c = null;
                if ((StorageEndpoint != null) && (!String.IsNullOrEmpty(AccountName)) && (!String.IsNullOrEmpty(AccountKey)))
                {
                    /*
                    LockBoxDebugHelper.Debug_Log("GetBlobClient", AccountName, false);
                    LockBoxDebugHelper.Debug_Log("GetBlobClient", AccountKey, false);
                    LockBoxDebugHelper.Debug_Log("GetBlobClient", StorageEndpoint.ToString(), false);
                     */
                    c = new CloudBlobClient(StorageEndpoint, new StorageCredentialsAccountAndKey(AccountName, AccountKey));
                }
                else if ((StorageEndpoint != null) && !String.IsNullOrEmpty(SharedAccessSignature))
                {
                    //Debug.WriteLine("Creating with signature");
                    c = new CloudBlobClient(StorageEndpoint, new StorageCredentialsSharedAccessSignature(SharedAccessSignature));
                }
                else
                {
                    throw new Exception("Not enough supplied credentials");
                }

                //LockBoxDebugHelper.Debug_Log("GetBlobClient", "Returning blob client", false);
                return (c);
            }
            catch (Exception e)
            {
                LastError = e.Message;
                return (null);
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Sets or gets the last error message
        /// </summary>
        //---------------------------------------------------------------------
        public string LastError
        {
            set;
            get;
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Clears the last error
        /// </summary>
        //---------------------------------------------------------------------
        private void ClearLastError()
        {
            LastError = String.Empty;
        }



        

        public Uri StorageEndpoint
        {
            get;
            set;
        }

        public string AccountName
        {
            get;
            set;
        }

        public string AccountKey
        {
            get;
            set;
        }

        public string SharedAccessSignature
        {
            get;
            set;
        }


        public bool SharedAccessSupported
        {
            get
            {
                return (true);
            }
            set
            {
                // nop
            }
        }


        
        public bool EndPointIsReady()
        {
            try
            {
                String ContainerName = StringHelper.CreateContainerName();
                


                return (true);
            }
            catch (Exception)
            {
                return (false);
            }
        }



        //---------------------------------------------------------------------
        /// <summary>
        ///     Returns the storage type for this object
        /// </summary>
        //---------------------------------------------------------------------
        public LockBoxStorageType StorageType
        {
            get
            {
                return LockBoxStorageType.MicrosoftCloud;
            }
            set
            {
                // nop
            }
        }











        #region ILockBoxStorage Members


        public bool SnapShotsSupported
        {
            get
            {
                return (true);
            }
            set
            {
                // nop
            }
        }

        

        #endregion





        #region ILockBoxStorage Members

        private SymmetricAlgorithm m_ContainerSymmetricKeyData = null;

        //---------------------------------------------------------------------
        /// <summary>
        ///     Sets or gets the symmetric key data that will be used to 
        ///     automatically encrypt ot decrypt container data (blobs, etc.)
        /// </summary>
        //---------------------------------------------------------------------
        public SymmetricAlgorithm ContainerSymmetricKeyData
        {
            get
            {
                return (m_ContainerSymmetricKeyData);
                
            }
            set
            {
                m_ContainerSymmetricKeyData = value;
                if (m_ContainerSymmetricKeyData != null)
                {
                    m_ContainerSymmetricKeyData.Padding = PaddingMode.PKCS7;
                }
            }
        }

        #endregion
    }
}
