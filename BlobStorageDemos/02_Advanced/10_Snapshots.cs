using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace BlobStorageDemos.Advanced
{
  public class _10_Snapshots
  {
    public static void Run()
    {
      #region Obtain a client from the connection string

      CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
      CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
      Console.WriteLine("Parsed connection string and created blob client.");

      #endregion

      #region Obtain reference to images container

      CloudBlobContainer docContainer = blobClient.GetContainerReference("docs");
      Console.WriteLine("Obtained reference to docs containers.");

      #endregion

      #region Create a snapshot

      CloudBlockBlob blockBlob = docContainer.GetBlockBlobReference(@"WhatIsBlobStorage.txt");
      CloudBlockBlob blobSnapshot1 = blockBlob.CreateSnapshot();
      var snapshotTime = blobSnapshot1.SnapshotTime.Value;
      Console.WriteLine("Created snapshot '{0}' for WhatIsBlobStorage.txt in docs container.", snapshotTime.ToString("o"));

      Thread.Sleep(TimeSpan.FromMilliseconds(100));
      CloudBlockBlob blobSnapshot2 = blockBlob.CreateSnapshot();
      Thread.Sleep(TimeSpan.FromMilliseconds(100));
      CloudBlockBlob blobSnapshot3 = blockBlob.CreateSnapshot();

      #endregion

      #region List snapshots

      Console.WriteLine("\nListing snapshots");
      var snapshots = docContainer
        .ListBlobs(useFlatBlobListing: true, blobListingDetails: BlobListingDetails.Snapshots)
        .Where(blob => ((CloudBlockBlob) blob).SnapshotTime.HasValue);
                      
      foreach (var snapshot in snapshots)
      {
        CloudBlockBlob blob = new CloudBlockBlob(blockBlob.Uri, ((CloudBlockBlob)snapshot).SnapshotTime, blobClient.Credentials);
        blob.FetchAttributes();
        Console.WriteLine(" {0} - {1}", blob.Uri.AbsoluteUri, blob.SnapshotTime);
      }

      #endregion

      #region Modify original blob

      using (var stringStream = new MemoryStream(Encoding.UTF8.GetBytes(@"Replace contents with this random string")))
      {
        blockBlob.UploadFromStream(stringStream);
      }
      Console.WriteLine("\nReplaced contents of WhatIsBlobStorage.txt with random string.");
      Console.WriteLine("Press any key to restore from snapshot ...");
      Console.ReadLine();

      #endregion

      #region Restore from snapshot

      CloudBlockBlob backupSnapshot = new CloudBlockBlob(blockBlob.Uri, snapshotTime, blobClient.Credentials);
      blockBlob.StartCopyFromBlob(backupSnapshot);
      Console.WriteLine("Restored contents of WhatIsBlobStorage.txt from original snapshot.");

      #endregion
    }
  }
}
