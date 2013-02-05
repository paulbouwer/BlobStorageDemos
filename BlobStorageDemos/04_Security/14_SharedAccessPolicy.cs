using System;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace BlobStorageDemos.Security
{
  public class _14_SharedAccessPolicy
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

      #region Create blob shared access policy

      var blobContainerPermissions = new BlobContainerPermissions();
      blobContainerPermissions.SharedAccessPolicies.Add("ReadOnlyPolicy", new SharedAccessBlobPolicy
      {
        Permissions = SharedAccessBlobPermissions.Read,
        SharedAccessStartTime = null,                                                                          
      });
      docContainer.SetPermissions(blobContainerPermissions);

      #endregion

      #region Get blob share access signature

      Console.WriteLine("\nObtaining shared access signature.");
      CloudBlockBlob blockBlob = docContainer.GetBlockBlobReference(@"WhatIsBlobStorage.txt");
      var signature = blockBlob.GetSharedAccessSignature(new SharedAccessBlobPolicy
      {
        SharedAccessExpiryTime = DateTime.UtcNow.AddDays(10)
      }, "ReadOnlyPolicy");

      Console.WriteLine("{0,-24}:{1}", "Shared Access Signature", signature);
      Console.WriteLine("{0,-24}:{1}", "Blob URI", blockBlob.Uri.AbsoluteUri);
      Console.WriteLine("{0,-24}:{1}{2}", "Shared Access URI", blockBlob.Uri.AbsoluteUri, signature);

      #endregion
    }
  }
}
