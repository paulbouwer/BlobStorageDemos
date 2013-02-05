using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace BlobStorageDemos.Advanced
{
  public class _08_ConcurrencyWithETag
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

      #region Show concurrency management using ETag property

      Console.WriteLine();

      CloudBlockBlob blockBlob1 = imageContainer.GetBlockBlobReference(@"rocks/WhitePocketFocus.jpg");
      CloudBlockBlob blockBlob2 = imageContainer.GetBlockBlobReference(@"rocks/WhitePocketFocus.jpg");
      blockBlob1.FetchAttributes();
      blockBlob2.FetchAttributes();
      Console.WriteLine("[1] Obtained ETag {0} for rocks/WhitePocketFocus.jpg in images container.", blockBlob1.Properties.ETag);
      Console.WriteLine("[2] Obtained ETag {0} for rocks/WhitePocketFocus.jpg in images container.", blockBlob2.Properties.ETag);

      var etag = blockBlob1.Properties.ETag;
      var accessCondition = new AccessCondition {IfMatchETag = etag};

      var metadata = new Dictionary<string, string>()
      {
        { "License",  "snowpeak" },
        { "URL",      "http://www.flickr.com/photos/snowpeak/7394540572/sizes/l/in/pool-809956@N25/"}
      };

      Console.WriteLine("[1] Setting metadata on rocks/WhitePocketFocus.jpg in images container.");
      foreach (KeyValuePair<string, string> pair in metadata) { blockBlob1.Metadata.Add(pair); } 
      blockBlob1.SetMetadata(accessCondition);      

      blockBlob1.FetchAttributes();
      Console.WriteLine("[1] New ETag {0} for rocks/WhitePocketFocus.jpg in images container.", blockBlob1.Properties.ETag);

      try
      {
        Console.WriteLine("[2] Setting metadata on rocks/WhitePocketFocus.jpg in images container.");
        foreach (KeyValuePair<string, string> pair in metadata) { blockBlob2.Metadata.Add(pair); }
        blockBlob2.SetMetadata(accessCondition);  
      }
      catch (Exception exception)
      {
        Console.WriteLine("[2] Caught exception '" + exception.GetType() + "' while trying to set metadata on rocks/WhitePocketFocus.jpg in images container.");
        Console.WriteLine(exception.StackTrace);
      }
      
      #endregion
    }
  }
}
