using System;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace BlobStorageDemos.Security
{
  public class _13_SharedAccessSignature
  {
    public static void Run()
    {
      #region Obtain a client from the connection string

      CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
      CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
      Console.WriteLine("Parsed connection string and created blob client.");

      #endregion

      #region Obtain reference to docs container

      CloudBlobContainer docContainer = blobClient.GetContainerReference("docs");
      Console.WriteLine("Obtained reference to docs container.");

      #endregion

      #region Get blob shared access signature

      Console.WriteLine("\nObtaining shared access signature.");
      CloudBlockBlob blockBlob = docContainer.GetBlockBlobReference(@"WhatIsBlobStorage.txt");
      var signature = blockBlob.GetSharedAccessSignature(new SharedAccessBlobPolicy
      {
        Permissions            = SharedAccessBlobPermissions.Read,
        SharedAccessStartTime  = null,
        SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(1)
      });

      Console.WriteLine("{0,-24}:{1}", "Shared Access Signature", signature);
      Console.WriteLine("{0,-24}:{1}", "Blob URI", blockBlob.Uri.AbsoluteUri);
      Console.WriteLine("{0,-24}:{1}{2}", "Shared Access URI", blockBlob.Uri.AbsoluteUri, signature);

      #endregion
    }
  }
}
