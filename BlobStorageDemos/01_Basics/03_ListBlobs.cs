using System;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace BlobStorageDemos.Basics
{
  public class _03_ListBlobs
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

      #region List blobs

      Console.WriteLine("\nListing blobs using flat listing.\n");
      ListBlobs(imageContainer, BlobListing.Flat);

      Console.WriteLine();
      Console.WriteLine("Listing blobs using hierachical listing.\n");
      ListBlobs(imageContainer, BlobListing.Hierarchical);

      #endregion
    }

    public static void ListBlobs(CloudBlobContainer blobContainer, BlobListing blobListing, String prefix = null)
    {
      if (blobListing == BlobListing.Flat)
      {
        foreach (var blockBlob in blobContainer.ListBlobs(prefix: null, useFlatBlobListing: true))
        {
          Console.WriteLine("{0,-10} - {1}", @"Blob", ((CloudBlockBlob)blockBlob).Name);
          Console.WriteLine("{0,-10} - {1}", String.Empty, blockBlob.Uri);
        }
      }
      else if (blobListing == BlobListing.Hierarchical)
      {
        foreach (var blockBlob in blobContainer.ListBlobs(prefix: prefix, useFlatBlobListing: false))
        {
          if (blockBlob is CloudBlockBlob)
          {
            Console.WriteLine("{0,-10} - {1}", @"Blob", ((CloudBlockBlob)blockBlob).Name);
            Console.WriteLine("{0,-10} - {1}", String.Empty, blockBlob.Uri);
          }
          else if (blockBlob is CloudBlobDirectory)
          {
            Console.WriteLine("{0,-10} - {1}", @"Directory", ((CloudBlobDirectory)blockBlob).Prefix);
            Console.WriteLine("{0,-10} - {1}", String.Empty, blockBlob.Uri);

            ListBlobs(blobContainer, BlobListing.Hierarchical, ((CloudBlobDirectory)blockBlob).Prefix);
          }
        }
      }
    }

  }
}
