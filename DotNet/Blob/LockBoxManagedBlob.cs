using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LockBox.Common.Data;
using LockBox.Common.IO;
using LockBox.Common.Security.Hash;
using LockBox.Common;
using System.Collections.Specialized;
using System.IO;
using System.Security.AccessControl;
using System.Diagnostics;
using System.Security.Cryptography;
using LockBox.Common.Security.Cryptography;
using System.IO.Compression;

namespace LockBox
{
    public class LockBoxManagedBlob
    {
        public Stream BlobDataStream { set; get; }               
        private String BlobID { set; get; }
        public String TemporaryFileName { set; get; }

        private String ContainerName { set; get; }
        private ILockBoxStorage m_StorageObject = null;
        private LockBoxStorageType m_StorageType = LockBoxStorageType.MicrosoftCloud;

        public SymmetricAlgorithm ContainerSymmetricKeyData
        {
            /*
            set
            {
                m_StorageObject.ContainerSymmetricKeyData = value;
            }
             */
            
            set
            {
                m_StorageObject.ContainerSymmetricKeyData = value;
            }
            
            
            get
            {
                return (m_StorageObject.ContainerSymmetricKeyData);
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Option that sets or gets a flag to encrypt any downloaded
        ///     blob data at rest
        /// </summary>
        //---------------------------------------------------------------------
        public bool EncryptDownloadedDataAtRest { set; get; }


        /*
        //---------------------------------------------------------------------
        /// <summary>
        ///     Constructor, does not do encryption, primarily used for 
        ///     testing only
        /// </summary>
        //---------------------------------------------------------------------
        public LockBoxManagedBlob(Uri StorageEndpointUri, String BlobSharedAccessSignature, LockBoxStorageType StorageType, String ContainerName,
            String BlobID)
        {
            m_InitStorageObject(StorageEndpointUri, BlobSharedAccessSignature, StorageType, ContainerName, BlobID, null);
        }
         */

        //---------------------------------------------------------------------
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="StorageEndpointUri"></param>
        /// <param name="BlobSharedAccessSignature"></param>
        /// <param name="StorageType"></param>
        /// <param name="ContainerName"></param>
        /// <param name="BlobID"></param>
        /// <param name="ProtectionKeyData"></param>
        //---------------------------------------------------------------------
        public LockBoxManagedBlob(Uri StorageEndpointUri, String BlobSharedAccessSignature, LockBoxStorageType StorageType, String ContainerName,
            String BlobID, SymmetricAlgorithm ProtectionKeyData)
        {
            if (ProtectionKeyData == null)
            {
                throw new ArgumentNullException("LockBoxManagedBlob->Protection key data cannot be null");
            }
            m_InitStorageObject(StorageEndpointUri, BlobSharedAccessSignature, StorageType, ContainerName, BlobID, ProtectionKeyData);
        }


        //---------------------------------------------------------------------
        /// <summary>
        ///     Destructor
        /// </summary>
        //---------------------------------------------------------------------
        ~LockBoxManagedBlob()
        {
            // Drop the previous file
            m_SecureDropTempFile();
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Gets the last error encountered by this object
        /// </summary>
        //---------------------------------------------------------------------
        public String LastError
        {
            get
            {
                String Result = LibraryResources.NotSet;
                if (m_StorageObject != null)
                {
                    Result = m_StorageObject.LastError;
                }
                return (Result);
            }
        }


        #region BLOB_SUBSTREAM_HELPERS


        //---------------------------------------------------------------------
        /// <summary>
        ///     Loads the configured blob into the given stream
        /// </summary>
        /// <param name="IntoThisStream">Stream to write into</param>
        /// <returns>
        ///     Returns true on success, false otherwise
        /// </returns>
        //---------------------------------------------------------------------
        private bool m_LoadBlobSubStreamData(Stream IntoThisStream)
        {
            Stream StreamToUse = IntoThisStream;
            return (m_StorageObject.GetBlobAsStream(ContainerName, LockBoxBlobHelper.FormBlobStreamName(BlobID, LockBoxBlobSubStream.data),
                StreamToUse, new NameValueCollection()));
        }


        

        #endregion

        //---------------------------------------------------------------------
        /// <summary>
        ///     Securely deletes the tempoary file
        /// </summary>
        //---------------------------------------------------------------------
        private void m_SecureDropTempFile()
        {
            // Close the stream
            if (BlobDataStream != null)
            {
                BlobDataStream.Close();
            }

            // Drop the temporary file
            if (!String.IsNullOrEmpty(TemporaryFileName) && File.Exists(TemporaryFileName))
            {
                LockBoxProtectData.SecureDeleteFile(TemporaryFileName);
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Creates the temp file name
        /// </summary>
        /// <returns></returns>
        //---------------------------------------------------------------------
        private String m_CreateTempFileName()
        {
            String TempFileName = Path.GetTempFileName();
            // Encrypt it if we are set to do so
            if (EncryptDownloadedDataAtRest)
            {
                // This won't work in server scenarios where they may not have
                // had EFS setup
                try
                {
                    File.Encrypt(TempFileName);
                }
                catch (Exception)
                {
                    // Best efforts, move on
                }
            }
            return (TempFileName);
        }

        private void m_InitStorageObject(Uri StorageEndpointUri, String BlobSharedAccessSignature, LockBoxStorageType StorageType, String ContainerName,
            String BlobID, SymmetricAlgorithm ContainerProtectionKeyData)
        {
            // Init our storage object, always do this first
            m_SetStorageObject(StorageEndpointUri, BlobSharedAccessSignature, StorageType);

            // Now do the rest of the object
            this.BlobID = BlobID;
            this.ContainerName = ContainerName;
            m_StorageObject.ContainerSymmetricKeyData = ContainerProtectionKeyData;
            this.EncryptDownloadedDataAtRest = true;

            
        }


        public bool WriteBlobData(Stream BlobData)
        {
            try
            {
                // No error, return true
                return (m_WriteBlobData(BlobData));
            }
            catch (Exception e)
            {
                LockBoxDebugHelper.Debug_Log("LockBoxManagedBlob->WriteBlobData", e.Message);
                return (false);
            }

        }

        private bool m_WriteBlobData(Stream BlobData)
        {           
            Stream StreamToUse = BlobData;

            // Storage object is already initialized so attempt the write
            NameValueCollection NVC = new NameValueCollection();
            return (m_StorageObject.UploadBlobFromStream(ContainerName, LockBoxBlobHelper.FormBlobStreamName(BlobID, LockBoxBlobSubStream.data), StreamToUse, NVC));
        }


        public bool LoadBlobData(Stream IntoThisStream)
        {
            try
            {
                //Debug.WriteLine("TempFile: " + m_LoadedStreamTempFileName);                
                // Now ready to load the blob data
                bool LoadDataStreamResult = m_LoadBlobSubStreamData(IntoThisStream);
                if (!LoadDataStreamResult)
                {
                    throw new Exception("Unable to load data sub stream");
                }
                IntoThisStream.Position = 0;

                // Everything succeeded, so return true
                return (true);
            }
            catch (Exception e)
            {
                LockBoxDebugHelper.Debug_Log("LockBoxManagedBlob->LoadBlobData(IntoThisStream)", e.Message, true);
                return (false);
            }
        }


        /*
        public bool LoadBlobData()
        {
            try
            {
                // Drop the previous file (if any)
                m_SecureDropTempFile();
                // Because the incoming stream can be enormous, we should create a 
                // temporary file to read it and pass it in as a stream and not as memory
                TemporaryFileName = m_CreateTempFileName();
                                
                // Already created, so just open 1/30/2013
                BlobDataStream = (Stream)File.Create(TemporaryFileName);
                
                //Debug.WriteLine("TempFile: " + m_LoadedStreamTempFileName);                
                // Now ready to load the blob data
                bool LoadDataStreamResult = m_LoadBlobSubStreamData(BlobDataStream);
                if (!LoadDataStreamResult)
                {
                    throw new Exception("Unable to load data sub stream");
                }               
                BlobDataStream.Position = 0;

                // Everything succeeded, so return true
                return (true);
            }
            catch (Exception e)
            {
                // Drop the temporary file
                m_SecureDropTempFile();
                LockBoxDebugHelper.Debug_Log("LockBoxManagedBlob->LoadBlobData", e.Message, true);
                //LockBoxDebugHelper.DoLogToFile("LockBoxManagedBlob->LoadBlobData: " + e.Message);
                return (false);
            }
        }
         */

        //---------------------------------------------------------------------
        /// <summary>
        ///     Loads the blob data into the blob data stream of this object
        ///     which uses a temp disk file to store the blob data
        ///     
        ///     2/2/2013
        ///     Note: for some reason for large files this causes the file
        ///     stream to close intermittently, so not stable to use this method
        /// </summary>
        /// <returns></returns>
        //---------------------------------------------------------------------
        public bool LoadBlobDataIntoTempFileStream()
        {
            try
            {
                // Drop the previous file (if any)
                m_SecureDropTempFile();
                // Because the incoming stream can be enormous, we should create a 
                // temporary file to read it and pass it in as a stream and not as memory
                TemporaryFileName = m_CreateTempFileName();

                // Already created, so just open 1/30/2013
                BlobDataStream = (Stream)File.Create(TemporaryFileName);

                return (LoadBlobData(BlobDataStream));
            }
            catch (Exception e)
            {
                // Drop the temporary file
                m_SecureDropTempFile();
                LockBoxDebugHelper.Debug_Log("LockBoxManagedBlob->LoadBlobData", e.Message, true);
                return (false);
            }
        }


        
        
        //---------------------------------------------------------------------
        /// <summary>
        ///     Given a URI endpoint and shared access signature and storage type,
        ///     loads the appropriate storage object
        /// </summary>
        /// <param name="StorageEndpointUri">Storage endpoint URI</param>
        /// <param name="BlobSharedAccessSignature">Shared access signature</param>
        /// <param name="StorageType">Type of storage (Azure, Rackspace, etc.)</param>
        //---------------------------------------------------------------------
        private void m_SetStorageObject(Uri StorageEndpointUri, String BlobSharedAccessSignature, LockBoxStorageType StorageType)
        {
            switch (StorageType)
            {
                case LockBoxStorageType.MicrosoftCloud:
                    m_StorageObject = new MicrosoftAzureStorage();
                    m_StorageObject.StorageEndpoint = StorageEndpointUri;
                    m_StorageObject.SharedAccessSignature = BlobSharedAccessSignature;
                    break;

                default:
                    throw new NotImplementedException("LockBoxBlob:Not implemented for " + StorageType.ToString());
            }
            m_StorageType = StorageType;

            // Set the container protection key data if available
            m_StorageObject.ContainerSymmetricKeyData = ContainerSymmetricKeyData;
        }

        


        
        
    }
}
