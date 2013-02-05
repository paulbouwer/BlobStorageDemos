using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace BlobStorageDemos.Advanced
{
  public class _09_ConcurrencyWithLease
  {
    public static void Run()
    {
      #region Obtain a client from the connection string

      CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
      CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
      Console.WriteLine("Parsed connection string and created blob client.");

      #endregion

      #region Obtain reference to images container

      CloudBlobContainer imageContainer = blobClient.GetContainerReference("images");
      Console.WriteLine("Obtained reference to images container.");

      #endregion

      #region Show concurrency management using leases

      Console.WriteLine();
      
      var leaseId = Guid.NewGuid().ToString();
      var metadata1 = new Dictionary<string, string>()
      {
        { "Process",  "1" }
      };
      var metadata2 = new Dictionary<string, string>()
      {
        { "Process",  "2" }
      };

      CloudBlockBlob blockBlob1 = imageContainer.GetBlockBlobReference(@"rocks/WhitePocketFocus.jpg");
      CloudBlockBlob blockBlob2 = imageContainer.GetBlockBlobReference(@"rocks/WhitePocketFocus.jpg");
      
      // Leases must be 15 - 60s or infinite
      blockBlob1.AcquireLease(TimeSpan.FromSeconds(15), leaseId);
      Console.WriteLine("[1] Obtained LeaseId {0} for rocks/WhitePocketFocus.jpg in images container.", leaseId);

      Console.WriteLine("[1] Setting metadata on rocks/WhitePocketFocus.jpg in images container.");
      foreach (KeyValuePair<string, string> pair in metadata1) { blockBlob1.Metadata.Add(pair); }
      blockBlob1.SetMetadata(new AccessCondition { LeaseId = leaseId });

      Console.WriteLine("Press any key to trigger concurrency ...");
      Console.ReadLine();

      try
      {
        Console.WriteLine("[2] Setting metadata on rocks/WhitePocketFocus.jpg in images container.");
        foreach (KeyValuePair<string, string> pair in metadata2) { blockBlob2.Metadata.Add(pair); } 
        blockBlob2.SetMetadata();        
      }
      catch (Exception exception)
      {
        Console.WriteLine("[2] Caught exception '" + exception.GetType() + "' while trying to set metadata on rocks/WhitePocketFocus.jpg in images container.");
        Console.WriteLine(exception.StackTrace);
      }

      blockBlob1.ReleaseLease(new AccessCondition{ LeaseId = leaseId });
      Console.WriteLine("[1] Released LeaseId {0} for rocks/WhitePocketFocus.jpg in images container.", leaseId);

      Console.WriteLine("[2] Setting metadata on rocks/WhitePocketFocus.jpg in images container.");
      blockBlob2.SetMetadata();

      #endregion
    }
  }
}
