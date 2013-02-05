using System;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace BlobStorageDemos.Basics
{
  public class _05_DeleteBlob
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

      #region Delete blob

      CloudBlockBlob blockBlob = docContainer.GetBlockBlobReference(@"Readme.txt");
      blockBlob.DeleteIfExists();
      Console.WriteLine("Deleted Readme.txt from docs container.");

      try
      {
        blockBlob.Delete();
      }
      catch (Exception exception)
      {
        Console.WriteLine("Caught exception '" + exception.GetType() + "' while trying to delete Readme.txt again from docs container.");
        Console.WriteLine(exception.StackTrace);  
      }

      #endregion
    }
  }
}
