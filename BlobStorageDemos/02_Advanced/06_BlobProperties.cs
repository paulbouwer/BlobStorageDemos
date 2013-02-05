using System;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace BlobStorageDemos.Advanced
{
  public class _06_BlobProperties
  {
    public static void Run()
    {
      #region Obtain a client from the connection string

      CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
      CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
      Console.WriteLine("Parsed connection string and created blob client.");

      #endregion

      #region Obtain reference to docs and images containers

      CloudBlobContainer docContainer = blobClient.GetContainerReference("docs");
      CloudBlobContainer imageContainer = blobClient.GetContainerReference("images");
      Console.WriteLine("Obtained reference to docs and images containers.");

      #endregion

      #region Get blob properties

      Console.WriteLine();
      GetBlobProperties(docContainer,   @"WhatIsBlobStorage.txt");
      GetBlobProperties(imageContainer, @"water/waterfall/Prometheus.jpg");

      #endregion

      #region Set blob properties

      CloudBlockBlob blockBlob = docContainer.GetBlockBlobReference(@"WhatIsBlobStorage.txt");
      blockBlob.Properties.ContentType = @"text/plain";
      blockBlob.SetProperties();
      Console.WriteLine("Set blob properties for WhatIsBlobStorage.txt.");

      #endregion
    }

    public static void GetBlobProperties(CloudBlobContainer blobContainer, string blobName)
    {
      CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(blobName);
      blockBlob.FetchAttributes();

      Console.WriteLine("Blob properties for: {0}", blockBlob.Name);
      Console.WriteLine("{0,-14}:{1}", "BlobType", blockBlob.Properties.BlobType);
      Console.WriteLine("{0,-14}:{1}", "ETag", blockBlob.Properties.ETag.Replace(@"""",""));
      Console.WriteLine("{0,-14}:{1}", "ContentType", blockBlob.Properties.ContentType);
      Console.WriteLine("{0,-14}:{1}", "Length", blockBlob.Properties.Length);
      Console.WriteLine("{0,-14}:{1}", "LastModified", blockBlob.Properties.LastModified);
      Console.WriteLine();      
    }
  }
}
