using System;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace BlobStorageDemos.Basics
{
  public class _01_CreateContainer
  {
    public static void Run()
    {
      #region Obtain a client from the connection string

      CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
      CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
      Console.WriteLine("Parsed connection string and created blob client.");

      #endregion

      #region Create a private container called mycontainer1

      var container1Name = String.Format("container1-{0:yyyyMMddhhmmss}", DateTime.Now);
      CloudBlobContainer container1 = blobClient.GetContainerReference(container1Name);
      container1.CreateIfNotExists();
      Console.WriteLine("Created mycontainer1 with private access.");

      #endregion

      #region Create a public container access container called mycontainer2

      var container2Name = String.Format("container2-{0:yyyyMMddhhmmss}", DateTime.Now);
      CloudBlobContainer container2 = blobClient.GetContainerReference(container2Name);
      container2.CreateIfNotExists();
      container2.SetPermissions(new BlobContainerPermissions{ PublicAccess = BlobContainerPublicAccessType.Container });
      Console.WriteLine("Created mycontainer2 with public container access.");

      #endregion

      #region Create a public blob access container called mycontainer3

      var container3Name = String.Format("container3-{0:yyyyMMddhhmmss}", DateTime.Now);
      CloudBlobContainer container3 = blobClient.GetContainerReference(container3Name);
      container3.CreateIfNotExists();
      container3.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
      Console.WriteLine("Created mycontainer3 with public blob access.");

      #endregion
    }
  }
}
