using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LockBox.Storage.Cloud
{
    public partial class CloudStorageEndpointHelper
    {



        public static bool MirrorPrimaryStorageEndpoint(ILockBoxStorage PrimaryEndpoint, ILockBoxStorage SecondaryEndpoint)
        {
            try
            {
                //-------------------------------------------------------------
                //  Sync containers
                //-------------------------------------------------------------
                // Get all the container names from the primary endpoint
                String[] PrimaryContainerNames = PrimaryEndpoint.GetContainerNames();
                

                // Iterate through each primary container name and make sure it's in the secondary
                foreach (String PContainerName in PrimaryContainerNames)
                {
                    if (!SecondaryEndpoint.ContainerExists(PContainerName))
                    {
                        bool IsPublic = PrimaryEndpoint.IsContainerPublic(PContainerName);
                        if (!SecondaryEndpoint.CreateContainer(PContainerName, IsPublic))
                        {
                            throw new Exception(String.Format("Unable to duplicate primary container {0} on secondary storage", PContainerName));
                        }
                    }
                }

                // Iterate through all the secondary containers, if they don't exist on the primary then
                // delete it off the secondary
                String[] SecondaryContainerNames = SecondaryEndpoint.GetContainerNames();
                foreach (String SContainerName in SecondaryContainerNames)
                {
                    if (!PrimaryEndpoint.ContainerExists(SContainerName))
                    {
                        // Delete off secondary
                        SecondaryEndpoint.RemoveContainer(SContainerName);
                    }
                }


                //-------------------------------------------------------------
                //  Sync blobs in primary containers to the secondary containers
                //-------------------------------------------------------------
                foreach (String PContainerName in PrimaryContainerNames)
                {
                    //----------------------------------------
                    // SYNC Primary blobs to secondary
                    //----------------------------------------
                    // Get the list of blobs in the current primary container
                    String[] PrimaryBlobNames = PrimaryEndpoint.GetBlobNames(PContainerName);
                    foreach (String PBloBName in PrimaryBlobNames)
                    {
                        // If it doesn't exist in the secondary then create it
                        if (!SecondaryEndpoint.BlobExists(PContainerName, PBloBName))
                        {

                        }
                        // If it does exist on secondary, check to make sure MD5s are the same
                        else
                        {
                            String PrimaryBlobMD5 = PrimaryEndpoint.GetBlobContentMD5(PContainerName, PBloBName);
                            String SecondaryBlobMD5 = SecondaryEndpoint.GetBlobContentMD5(PContainerName, PBloBName);
                            if (PrimaryBlobMD5 != SecondaryBlobMD5)
                            {
                                // MD5s don't match, so need to copy from primary to secondary


                            }
                        }
                    }

                    //----------------------------------------
                    //  Remove stale blobs from secondary
                    //----------------------------------------
                    String[] SecondaryBlobNames = SecondaryEndpoint.GetBlobNames(PContainerName);
                    foreach (String SBlobName in SecondaryBlobNames)
                    {
                        // If the blob doesn't exist on the primary then delete it off the secondary
                        if (!PrimaryEndpoint.BlobExists(PContainerName, SBlobName))
                        {
                            SecondaryEndpoint.RemoveBlob(PContainerName, SBlobName, true);
                        }
                    }
                }

                return (true);
            }
            catch (Exception)
            {
                return (false);
            }

        }

    }
}
