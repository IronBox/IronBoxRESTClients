using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.WindowsAzure.StorageClient;
using System.Collections.Specialized;
using System.IO;
using System.IO.Compression;
using System.IO.MemoryMappedFiles;
using LockBox.Common.Security.Cryptography;
using LockBox.Common.IO.Compression;
using System.Security.Cryptography;
using LockBox.Common.IO;
using LockBox.Common;
using System.Diagnostics;
using LockBox;

namespace LockBox
{
    public partial class MicrosoftAzureStorage 
    {

        //---------------------------------------------------------------------
        /// <summary>
        ///     Gets the MD5 string value of the blob contents
        /// </summary>
        /// <param name="ContainerName">Container name</param>
        /// <param name="BlobName">Blob name</param>
        /// <returns>
        ///     Returns the MD5 string, otherwise null on error
        /// </returns>
        //---------------------------------------------------------------------
        public String GetBlobContentMD5(String ContainerName, String BlobName)
        {
            ClearLastError();

            try
            {
                // Validate the blob exists
                if (!BlobExists(ContainerName, BlobName))
                {
                    throw new Exception(LibraryResources.BlobDoesNotExist);
                }

                // Get reference to the blob
                CloudBlob Blob = GetBlobClient().GetBlobReference(String.Format("{0}/{1}", ContainerName, BlobName));
                Blob.FetchAttributes();
                return (Blob.Properties.ContentMD5);
            }
            catch (Exception e)
            {
                LastError = e.Message;
                return (null);
            }

        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Gets a blob as a stream
        /// </summary>
        /// <param name="ContainerName">
        ///     Container name
        /// </param>
        /// <param name="BlobName">Blob name</param>
        /// <param name="BlobData">Receiving blob data stream</param>
        /// <param name="BlobMetaData">Receiving blob meta data</param>
        /// <returns>
        ///     Returns true on success, false otherwise.
        /// </returns>
        //---------------------------------------------------------------------
        public bool GetBlobAsStream(String ContainerName, String BlobName, Stream BlobData, 
            NameValueCollection BlobMetaData)
        {
            ClearLastError();

            // Check if the blob exists
            try
            {
                // Input validation
                if ((BlobData == null) || (BlobMetaData == null))
                {
                    throw new Exception(LibraryResources.InvalidInputs);
                }

                
                // Validate the blob exists
                if (!BlobExists(ContainerName, BlobName))
                {
                    Debug.WriteLine("{0}/{1}", ContainerName, BlobName);
                    throw new Exception(LibraryResources.BlobDoesNotExist);
                }
                

                // Grab the blob and metadata
                /* This won't work with shared access signatures
                CloudBlobContainer Container = GetBlobClient().GetContainerReference(ContainerName);
                CloudBlob Blob = Container.GetBlobReference(BlobName);
                 */
                CloudBlob Blob = GetBlobClient().GetBlobReference(String.Format("{0}/{1}", ContainerName, BlobName));

                Blob.FetchAttributes();

                // Check if the blob is empty
                String EmptyValueStr;
                bool IsEmpty = false;
                if (LockBoxBlobHelper.BlobMetaDataContainsKeyWithValue(Blob.Metadata, LockBoxBlobMetaDataKey.IsEmptyFile.ToString(), out EmptyValueStr))
                {
                    bool ParsedBool;
                    if (Boolean.TryParse(EmptyValueStr, out ParsedBool))
                    {
                        IsEmpty = ParsedBool;
                    }
                }

                // Return the blob meta data
                BlobMetaData.Clear();
                BlobMetaData.Add(Blob.Metadata);
                if (IsEmpty)
                {
                    //Debug.WriteLine("Returning empty blob");
                    // Blob is empty, remove our empty key
                    BlobData = new MemoryStream();
                    BlobMetaData.Remove(LockBoxBlobMetaDataKey.IsEmptyFile.ToString());
                }
                else
                {
                    Stream StreamToUse = BlobData;

                    // Decrypt the blob stream if symmetric protection data is set
                    bool UseEncryption = ContainerSymmetricKeyData != null;
                    if (UseEncryption)
                    {
                        LockBoxDebugHelper.Debug_Log("UploadBlobFromStream", "Automatically decrypting blob", false);
                        StreamToUse = SymmetricEncryptionHelper.CreateDecryptedWriteStream(BlobData, ContainerSymmetricKeyData);
                    }

                    // Blob is not empty
                    //Blob.DownloadToStream(BlobData);
                    Blob.DownloadToStream(StreamToUse);

                    // If we are using encryption, then we need to remove any extra padding 
                    // set by the encryption method, we can accomplish this with flushign
                    // any final blocks
                    if (UseEncryption)
                    {
                        CryptoStream CS = (CryptoStream)StreamToUse;
                        CS.FlushFinalBlock();
                    }
                }

                // Done
                return (true);
            }
            catch (Exception e)
            {
                LockBoxDebugHelper.Debug_Log("GetBlobAsStream", e.ToString(), true);
                LastError = e.Message;
                return (false);
            }
        }

         
        //---------------------------------------------------------------------
        /// <summary>
        ///     Creates an Azure blob with the given container name, blob name, 
        ///     blob data and meta data.
        /// </summary>
        /// <param name="ContainerName">Container name</param>
        /// <param name="BlobName">Blob name</param>
        /// <param name="BlobData">Blob data</param>
        /// <param name="BlobMetaData">Blob meta data</param>
        /// <returns>
        ///     Returns true on success, false otherwise
        /// </returns>
        /// <remarks>
        ///     Blob must not already exist before this call.
        /// </remarks>
        //---------------------------------------------------------------------
        public bool UploadBlobFromStream(string ContainerName, string BlobName, Stream BlobData, 
            NameValueCollection BlobMetaData)
        {
            ClearLastError();
            FileStream UnderlyingFileStream = null;
            try
            {
                // Input validation
                if (BlobData == null)
                {
                    throw new Exception(LibraryResources.InvalidBlobData);
                }

                /* This won't work if we are using SAS, container existence is verified implicitly, 
                 * operation won't work unless it exists
                // Validate that the container exists
                if (!ContainerExists(ContainerName))
                {
                    throw new Exception(LibraryResources.ContainerDoesNotExist);
                }
                 */
                
                // If blob exists, then we fail this operation, caller must 
                // delete first
                // This was the case when it was CreateBlob, but now we are straight up
                // uploading directly so we don't care if it exists or not
                /*
                if (BlobExists(ContainerName, BlobName))
                {
                    
                    throw new Exception("Blob exists, must remove this blob first before this operation");
                }
                 */
                
                /* This won't work if we are using SAS, get the blob reference directly
                // Upload the blob to cloud storage
                Debug.WriteLine("here1");
                CloudBlobContainer Container = GetBlobClient().GetContainerReference(ContainerName);
                Debug.WriteLine("here2");
                CloudBlob Blob = Container.GetBlobReference(BlobName);
                */
                CloudBlob Blob = GetBlobClient().GetBlobReference(String.Format("{0}/{1}", ContainerName, BlobName));
                
                // Check if the given data is empty
                Stream StreamToUse = BlobData;
                bool IsEmpty = false;
                if (BlobData.Length == 0)
                {                   
                    IsEmpty = true;
                    byte[] EmptyData = new byte[] { 0x1, 0x2, 0x3 };
                    StreamToUse = new MemoryStream(EmptyData);
                }

                // Encrypt the blob stream if symmetric protection data is set
                if (ContainerSymmetricKeyData != null)
                {
                    LockBoxDebugHelper.Debug_Log("UploadBlobFromStream", "Automatically encrypting blob", false);
                    StreamToUse = SymmetricEncryptionHelper.CreateEncryptedReadStream(StreamToUse, ContainerSymmetricKeyData);
                }

                // Upload the blob as a block blob
                Blob.UploadFromStream(StreamToUse);

                // Set meta data
                if (BlobMetaData != null)
                {
                    Blob.Metadata.Add(BlobMetaData);
                }
                Blob.Metadata[LockBoxBlobMetaDataKey.IsEmptyFile.ToString()] = IsEmpty ? Boolean.TrueString : Boolean.FalseString;
                Blob.SetMetadata();

                // No errors, done
                return (true);
            }
            catch (Exception e)
            {
                LockBoxDebugHelper.Debug_Log("UploadBlobFromStream", "Exception: " + e.ToString(), true);
                LastError = e.Message;
                return (false);
            }
            finally
            {
                // Close the underlying file stream if it's open
                if (UnderlyingFileStream != null)
                {
                    UnderlyingFileStream.Close();
                }
            }
        }



        //---------------------------------------------------------------------
        /// <summary>
        ///     Creates a new empty blob if it does not already exist, with 
        ///     empty meta data, no encryption or compression
        /// </summary>
        /// <param name="ContainerName">Container name</param>
        /// <param name="BlobName">Blob name</param>
        /// <returns>
        ///     Returns true on success, false otherwise
        /// </returns>
        //---------------------------------------------------------------------
        public bool TouchBlob(string ContainerName, string BlobName)
        {
            ClearLastError();
            try
            {
                bool Result = true;
                if (!BlobExists(ContainerName, BlobName))
                {
                    // Blob doesn't exist, create it with no meta data
                    MemoryStream ms = new MemoryStream();
                    NameValueCollection metadata = new NameValueCollection();
                    Result = UploadBlobFromStream(ContainerName, BlobName, ms, metadata);
                }
                return (Result);
            }
            catch (Exception e)
            {
                LastError = e.Message;
                return (false);
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Removes a given blob 
        /// </summary>
        /// <param name="ContainerName">Container name</param>
        /// <param name="BlobName">Name of blob to remove</param>
        /// <param name="RemoveSnapShots">Indicates </param>
        /// <returns>
        ///     Returns true if the blob exists, false otherwise.
        /// </returns>
        /// <remarks>
        ///     If the blob does not exist, then this method will return false
        ///     to indicate that the remove method did not succeed.
        /// </remarks>
        //---------------------------------------------------------------------
        public bool RemoveBlob(string ContainerName, string BlobName, bool RemoveSnapShots)
        {
            ClearLastError();
            try
            {
                if (!BlobExists(ContainerName, BlobName))
                {
                    throw new Exception("Blob does not exist");
                }

                CloudBlobContainer container = GetBlobClient().GetContainerReference(ContainerName);
                CloudBlob blob = container.GetBlobReference(BlobName);
                BlobRequestOptions options = new BlobRequestOptions();
                if (SnapShotsSupported && RemoveSnapShots)
                {
                    options.DeleteSnapshotsOption = DeleteSnapshotsOption.IncludeSnapshots;
                }
                blob.Delete(options);
                LockBoxDebugHelper.Debug_Log("RemoveBlob", String.Format("Dropped blob {0}/{1}", ContainerName, BlobName), false);
                return (true);
            }
            catch (Exception e)
            {
                LastError = e.Message;
                LockBoxDebugHelper.Debug_Log("RemoveBlob", String.Format("Unable to remove blob {0}/{1}: {2}", ContainerName, BlobName, e.Message), true);
                return (false);
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Indicates if a given blob exists or not
        /// </summary>
        /// <param name="ContainerName"></param>
        /// <param name="BlobName"></param>
        /// <returns></returns>
        //---------------------------------------------------------------------
        public bool BlobExists(string ContainerName, string BlobName)
        {
            ClearLastError();
            bool Result = GetBlobMetaData(ContainerName, BlobName) != null;
            return (Result);
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Retrieves the meta data for a given blob in a given container
        /// </summary>
        /// <param name="ContainerName"></param>
        /// <param name="BlobName"></param>
        /// <returns>
        ///     Returns a namevalue collection on success, null otherwise
        /// </returns>
        //---------------------------------------------------------------------
        public NameValueCollection GetBlobMetaData(string ContainerName, string BlobName)
        {
            ClearLastError();
            try
            {
                // Must call fetch attributes, otherwise this call won't work
                // you'll get an empty collection
                CloudBlobContainer container = GetBlobClient().GetContainerReference(ContainerName);
                CloudBlob blob = container.GetBlobReference(BlobName);
                blob.FetchAttributes();
                return (blob.Metadata);
            }
            catch (Exception e)
            {
                LastError = e.Message;
                LockBoxDebugHelper.Debug_Log("GetBlobMetaData", e.Message, true);
                return (null);
            }
        }

        public bool SetBlobMetaData(string ContainerName, string BlobName, NameValueCollection MetaData, bool DoAppend)
        {
            ClearLastError();
            try
            {
                // Get reference to the blob, must fetch attributes otherwise update
                // won't succeed
                CloudBlobContainer container = GetBlobClient().GetContainerReference(ContainerName);
                CloudBlob blob = container.GetBlobReference(BlobName);
                blob.FetchAttributes();

                // Wipe the current meta data if we're not appending
                if (!DoAppend)
                {
                    blob.Metadata.Clear();
                }

                // Add the given meta to the blobs meta data, then set it
                foreach (String k in MetaData)
                {
                    blob.Metadata[k] = MetaData[k];
                }
                blob.SetMetadata();

                // Done return true
                return (true);
            }
            catch (Exception e)
            {
                LastError = e.Message;
                LockBoxDebugHelper.Debug_Log("SetBlobMetaData", "Unable to set blob metadata: " + e.Message, true);
                return (false);
            }
        }


        //---------------------------------------------------------------------
        /// <summary>
        ///     Gets a shared access signature information for a given blob
        /// </summary>
        /// <param name="Permissions">Permissions to apply</param>
        /// <param name="ContainerName">Container name</param>
        /// <param name="BlobName">Blob name</param>
        /// <param name="ExpirationMinutes">Expiration minutes</param>
        /// <param name="SharedAccessSignature">Shared access signature</param>
        /// <param name="AccessUri">Access URI</param>
        /// <returns>
        ///     Returns true on success, false other wise
        /// </returns>
        //---------------------------------------------------------------------
        public bool GetSharedAccessSignatureForBlob(SharedAccessPermissions Permissions, string ContainerName, string BlobName, double ExpirationMinutes, 
            out string SharedAccessSignature, out string AccessUri)
        {
            ClearLastError();
            try
            {
                // Check if shared access signatures are supported, kind of redundant since 
                // we know, but more robus in case any code changes happen
                if (!SharedAccessSupported)
                {
                    throw new NotSupportedException(LibraryResources.SharedAccessNotSupported);
                }

                // Check if the container exists
                if (!BlobExists(ContainerName, BlobName))
                {
                    throw new Exception("Blob does not exist");
                }

                // According to http://msdn.microsoft.com/en-us/library/windowsazure/ee772877.aspx, 
                // SAS can only live less than 60 minutes so make a check for this
                if (ExpirationMinutes > SharedAccessSignatureHelper.SharedAccessSignatureMaxTtlMinutes)
                {
                    throw new Exception("Azure SAS may only be valid for be less than 60 minutes");
                }

                // Get reference to the conatiner
                CloudBlobContainer c = GetBlobClient().GetContainerReference(ContainerName);
                CloudBlob blob = c.GetBlobReference(BlobName);
                SharedAccessSignature = blob.GetSharedAccessSignature(new SharedAccessPolicy()
                {
                    Permissions = Permissions,
                    SharedAccessExpiryTime = DateTime.UtcNow + TimeSpan.FromMinutes(ExpirationMinutes)
                }
                );
                AccessUri = blob.Uri.AbsoluteUri + SharedAccessSignature;
                // Success, return the 
                return (true);
            }
            catch (Exception e)
            {
                SharedAccessSignature = null;
                AccessUri = null;
                LastError = e.Message;
                //LockBoxDebugHelper.DoLogToFile(ExpirationMinutes.ToString(), @"c:\temp\azure.txt");
                return (false);
            }
        }


        // Read blobs http://gurucoders.blogspot.com/2012/01/how-to-read-all-blob-list-from-windows.html


        public string[] GetBlobNames(string ContainerName)
        {
            return (GetBlobNames(ContainerName, null, false));
        }

        public String[] GetBlobNames(String ContainerName, String Prefix, bool CaseSensitive)
        {

            try
            {
                List<String> Results = new List<String>();
                CloudBlobContainer BlobContainer = GetBlobClient().GetContainerReference(ContainerName);

                BlobRequestOptions options = new BlobRequestOptions();
                options.UseFlatBlobListing = true;


                foreach (var blobItem in BlobContainer.ListBlobs())
                {
                    String BlobName = blobItem.Uri.Segments[2];
                    if (!String.IsNullOrEmpty(BlobName))
                    {
                        // Check if we have a prefix to use, if so then we need to check
                        if (!String.IsNullOrEmpty(Prefix))
                        {
                            String BlobNameCheck = CaseSensitive ? BlobName : BlobName.ToLower();
                            String PrefixToUse = CaseSensitive ? Prefix : Prefix.ToLower();
                            if (!BlobNameCheck.StartsWith(PrefixToUse))
                            {
                                // Skip the current blob name, doesn't matchprefix
                                continue;
                            }
                        }
                        Results.Add(BlobName);
                    }
                }

                // Done
                return (Results.ToArray());
            }
            catch (Exception e)
            {
                LockBoxDebugHelper.Debug_Log("GetBlobNames", e.Message, true);
                return (null);
            }
        }

        
        

    }
}
