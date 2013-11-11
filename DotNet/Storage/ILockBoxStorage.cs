using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using LockBox.Common.IO.Compression;
using LockBox.Common.Security.Cryptography;
using System.Collections.Specialized;
using System.Security.Cryptography;
using Microsoft.WindowsAzure.StorageClient;


namespace LockBox
{
    public interface ILockBoxStorage
    {

        LockBoxStorageType StorageType { set; get; }
        Uri StorageEndpoint { set; get; }
        String AccountName { set; get; }
        String AccountKey { set; get; }
        bool SharedAccessSupported { set; get; }
        bool SnapShotsSupported { set; get; }
        String SharedAccessSignature { set; get; }
        String LastError { set; get; }
        bool EndPointIsReady();
        SymmetricAlgorithm ContainerSymmetricKeyData { set; get; }
        
        
        //---------------------------------------------------------------------
        //  Container methods
        //---------------------------------------------------------------------
        bool CreateContainer(String ContainerName, NameValueCollection MetaData, bool IsPublic);
        bool CreateContainer(String ContainerName, bool IsPublic);
        bool RemoveContainer(String ContainerName);
        bool ContainerExists(String ContainerName);
        NameValueCollection GetContainerMetaData(String ContainerName);
        bool SetContainerMetaData(String ContainerName, NameValueCollection MetaData, bool DoAppend);
        String[] GetContainerNames();
        String[] GetContainerNames(String Prefix);

        // SAS must time to live must be < 60 minutes
        bool GetSharedAccessSignatureForContainer(SharedAccessPermissions Permissions, string ContainerName, 
            double ExpirationMinutes, out String SharedAccessSignature, out String AccessUri);


        bool IsContainerPublic(String ContainerName);

        //---------------------------------------------------------------------
        //  Blob methods
        //---------------------------------------------------------------------
        bool UploadBlobFromStream(string ContainerName, string BlobName, Stream BlobData, NameValueCollection MetaData);
        bool GetBlobAsStream(String ContainerName, String BlobName, Stream BlobData, NameValueCollection BlobMetaData);
        bool TouchBlob(String ContainerName, String BlobName);
        bool RemoveBlob(String ContainerName, String BlobName, bool RemoveSnapShots);
        bool BlobExists(String ContainerName, String BlobName);
        NameValueCollection GetBlobMetaData(String ContainerName, String BlobName);
        bool SetBlobMetaData(String ContainerName, String BlobName, NameValueCollection MetaData, bool DoAppend);

        // SAS must time to live must be < 60 minutes
        bool GetSharedAccessSignatureForBlob(SharedAccessPermissions Permissions, String ContainerName, String BlobName,
            double ExpirationMinutes, out String SharedAccessSignature, out String AccessUri);
        String[] GetBlobNames(String ContainerName);
        String[] GetBlobNames(String ContainerName, String Prefix, bool CaseSensitive);
        // Directory folder for blobs



        String GetBlobContentMD5(String ContainerName, String BlobName);

        
    }
}
