using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.WindowsAzure.StorageClient;
using System.Collections.Specialized;
using LockBox.Common;

namespace LockBox
{
    public partial class MicrosoftAzureStorage 
    {

        public bool IsContainerPublic(String ContainerName)
        {
            ClearLastError();
            try
            {
                // If the container already exists, then we can't create
                if (ContainerExists(ContainerName))
                {
                    throw new Exception("Container already exists, can't create");
                }

                CloudBlobContainer c = GetBlobClient().GetContainerReference(ContainerName);
                c.FetchAttributes();

                // Return only if the container public marker is on the container
                return (c.GetPermissions().PublicAccess == BlobContainerPublicAccessType.Container);
            }
            catch (Exception e)
            {
                // We failed the create operation
                LastError = e.Message;
                return (false);
            }
        }


        public bool CreateContainer(string ContainerName, NameValueCollection MetaData, bool IsPublic)
        {
            ClearLastError();
            bool ContainerCreatedByThisCall = false;
            try
            {
                // If the container already exists, then we can't create
                if (ContainerExists(ContainerName))
                {
                    throw new Exception("Container already exists, can't create");
                }

                CloudBlobContainer c = GetBlobClient().GetContainerReference(ContainerName);
                c.Create();
                ContainerCreatedByThisCall = true;

                // Set permissions
                BlobContainerPermissions p = new BlobContainerPermissions();
                p.PublicAccess = IsPublic ? BlobContainerPublicAccessType.Container : BlobContainerPublicAccessType.Off;
                c.SetPermissions(p);

                // Set the meta data if any
                if (MetaData != null)
                {
                    foreach (String key in MetaData)
                    {
                        c.Metadata[key] = MetaData[key];
                    }
                    c.SetMetadata();
                }

                // Done
                return (true);
            }
            catch (Exception e)
            {
                // We need to clean up if the container was created by us
                if (ContainerCreatedByThisCall)
                {
                    RemoveContainer(ContainerName);
                }

                // We failed the create operation
                LastError = e.Message;
                return (false);
            }
        }


        public bool CreateContainer(string ContainerName, bool IsPublic)
        {
            return (CreateContainer(ContainerName, null, IsPublic));
        }


        public bool RemoveContainer(string ContainerName)
        {
            ClearLastError();
            try
            {
                CloudBlobContainer c = GetBlobClient().GetContainerReference(ContainerName);
                if (c == null)
                {
                    throw new Exception("Unable to get reference to container");
                }
                c.Delete();
                return (!ContainerExists(ContainerName));
            }
            catch (Exception e)
            {
                LastError = e.Message;
                return (false);
            }
        }


        public bool ContainerExists(string ContainerName)
        {
            try
            {
                GetBlobClient().GetContainerReference(ContainerName).FetchAttributes();
                return (true);
            }
            catch (StorageClientException e)
            {
                LockBoxDebugHelper.Debug_Log("ContainerExists", e.Message, true);
                return (false);
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Returns all the container names
        /// </summary>
        /// <returns>
        ///     Returns a list of the container names, null on error
        /// </returns>
        //---------------------------------------------------------------------
        public string[] GetContainerNames()
        {
            return (GetContainerNames(String.Empty));
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///     Returns all the container names starting with the given
        ///     prefix.
        /// </summary>
        /// <param name="Prefix">Prefix to check</param>
        /// <returns>
        ///     Returns a list of the container names, null on error
        /// </returns>
        //---------------------------------------------------------------------
        public string[] GetContainerNames(string Prefix)
        {
            ClearLastError();
            String PrefixToUse = String.IsNullOrEmpty(Prefix) ? String.Empty : Prefix;
            try
            {
                List<String> Results = new List<string>();
                foreach (CloudBlobContainer s in GetBlobClient().ListContainers(PrefixToUse))
                {
                    Results.Add(s.Name);
                }
                return (Results.ToArray());
            }
            catch (Exception e)
            {
                LastError = e.Message;
                return (null);
            }
        }


        


        public NameValueCollection GetContainerMetaData(string ContainerName)
        {
            ClearLastError();
            try
            {
                
                if (!ContainerExists(ContainerName))
                {
                    throw new Exception("Container does not exist");
                }
                
                CloudBlobContainer c = GetBlobClient().GetContainerReference(ContainerName);
                c.FetchAttributes();    // This has to be called, else the meta data will be empty

                return (c.Metadata);
            }
            catch (Exception e)
            {
                LastError = e.Message;
                return (null);
            }
        }

        public bool SetContainerMetaData(String ContainerName, NameValueCollection MetaData, bool DoAppend)
        {
            ClearLastError();
            try
            {
                /* Redundant, since we're call fetch attributes anyways
                if (!ContainerExists(ContainerName))
                {
                    throw new Exception("Container does not exist");
                }
                 */
                CloudBlobContainer c = GetBlobClient().GetContainerReference(ContainerName);
                c.FetchAttributes();

                if (!DoAppend)
                {
                    c.Metadata.Clear();
                }

                // Add our new meta data
                foreach (String s in MetaData)
                {
                    c.Metadata[s] = MetaData[s];
                }

                // Push the updates
                c.SetMetadata();

                return (true);
            }
            catch (Exception e)
            {
                LastError = e.Message;
                return (false);
            }
        }

        
        //---------------------------------------------------------------------
        /// <summary>
        ///     Gets a shared access signature for the given container
        /// </summary>
        /// <param name="Permissions">Permission to set</param>
        /// <param name="ContainerName">Container name</param>
        /// <param name="ExpirationMinutes">
        ///     Duration in minutes for the signature to be valid.
        /// </param>
        /// <param name="SharedAccessSignature">Shared access signature</param>
        /// <param name="AccessUri">
        ///     Access URI
        /// </param>
        /// <returns>
        ///     Returns true on success, false otherwise
        /// </returns>
        //---------------------------------------------------------------------
        public bool GetSharedAccessSignatureForContainer(SharedAccessPermissions Permissions, 
            string ContainerName, double ExpirationMinutes, 
            out String SharedAccessSignature, out String AccessUri)
        {
            ClearLastError();
            try
            {
                // Check if shared access signatures are supported, kind of redundant since 
                // we know, but more robus in case any code changes happen
                if (!SharedAccessSupported)
                {
                    throw new NotSupportedException("Shared access signatures not supported");
                }

                // Check if the container exists
                if (!ContainerExists(ContainerName))
                {
                    throw new Exception("Container does not exist");
                }

                // According to http://msdn.microsoft.com/en-us/library/windowsazure/ee772877.aspx, 
                // SAS can only live less than 60 minutes so make a check for this
                if (ExpirationMinutes >= 60)
                {
                    throw new Exception("Azure SAS may only be valid for be less than 60 minutes");
                }

                // Get reference to the conatiner
                CloudBlobContainer c = GetBlobClient().GetContainerReference(ContainerName);
                SharedAccessSignature = c.GetSharedAccessSignature(new SharedAccessPolicy()
                    {
                        Permissions = Permissions,
                        SharedAccessExpiryTime = DateTime.UtcNow + TimeSpan.FromMinutes(ExpirationMinutes)
                        
                    }
                );
                AccessUri = c.Uri.AbsoluteUri + SharedAccessSignature;
                // Success, return the 
                return (true);
            }
            catch (Exception e)
            {
                SharedAccessSignature = null;
                AccessUri = null;
                LastError = e.Message;
                return (false);
            }
        }
    }
}
