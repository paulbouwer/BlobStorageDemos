using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace BlobStorageDemos.Advanced
{
  public class _07_BlobMetadata
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

      #region Set metadata

      Console.WriteLine();
      var metadata = new Dictionary<string, string>()
      {
        { "License",  "MarilynJane" },
        { "URL",      "http://www.flickr.com/photos/marilynjane/4069106759/sizes/o/in/pool-809956@N25/"}
      };
      SetBlobMetadata(imageContainer, @"trees/AutumnInTheNewForest.jpg", metadata);
      Console.WriteLine("Metadata set for trees/AutumnInTheNewForest.jpg in images container.");

      #endregion

      #region Get metadata

      Console.WriteLine();
      GetBlobMetadata(imageContainer, @"trees/AutumnInTheNewForest.jpg");
      GetBlobMetadata(imageContainer, @"trees/PaperbarkTrees.jpg");

      #endregion
    }

    public static void SetBlobMetadata(CloudBlobContainer blobContainer, string blobName, IDictionary<string, string> metadata)
    {
      CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(blobName);
      foreach (KeyValuePair<string, string> pair in metadata) { blockBlob.Metadata.Add(pair); }
      blockBlob.SetMetadata();
    }

    public static void GetBlobMetadata(CloudBlobContainer blobContainer, string blobName)
    {
      CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(blobName);
      blockBlob.FetchAttributes();

      Console.WriteLine("Blob metadata for: {0}", blockBlob.Name);
      Console.WriteLine("{0,-14}:{1}", "License", blockBlob.Metadata.ContainsKey("License") ? blockBlob.Metadata["License"] : "-");
      Console.WriteLine("{0,-14}:{1}", "URL", blockBlob.Metadata.ContainsKey("URL") ? blockBlob.Metadata["URL"] : "-");
      Console.WriteLine();   
    }
  }
}
