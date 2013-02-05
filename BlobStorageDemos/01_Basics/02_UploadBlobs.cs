using System;
using System.IO;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace BlobStorageDemos.Basics
{
  public class _02_UploadBlobs
  {
    public static void Run()
    {
      #region Obtain a client from the connection string

      CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
      CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
      Console.WriteLine("Parsed connection string and created blob client.");

      #endregion

      #region Create a private container called docs

      CloudBlobContainer docContainer = blobClient.GetContainerReference("docs");
      docContainer.CreateIfNotExists();
      Console.WriteLine("Created docs container with private access.");

      #endregion

      #region Upload blobs

      UploadFile(docContainer, @"Readme.txt",             @"..\..\Resources\Docs\Readme.txt",             @"text/plain");
      UploadFile(docContainer, @"WhatIsBlobStorage.txt",  @"..\..\Resources\Docs\WhatIsBlobStorage.txt");

      #endregion

      #region Create a private container called images

      CloudBlobContainer imageContainer = blobClient.GetContainerReference("images");
      imageContainer.CreateIfNotExists();
      Console.WriteLine("Created images container with private access.");

      #endregion

      #region Upload blobs - advanced

      UploadFile(imageContainer, @"trees/AutumnInTheNewForest.jpg", @"..\..\Resources\Images\AutumnInTheNewForest.jpg", @"image/jpeg");
      UploadFile(imageContainer, @"trees/PaperbarkTrees.jpg",       @"..\..\Resources\Images\PaperbarkTrees.jpg",       @"image/jpeg");
      UploadFile(imageContainer, @"water/beach/Denmark.jpg",        @"..\..\Resources\Images\Denmark.jpg",              @"image/jpeg");      
      UploadFile(imageContainer, @"water/waterfall/Prometheus.jpg", @"..\..\Resources\Images\Prometheus.jpg",           @"image/jpeg");
      UploadFile(imageContainer, @"rocks/WhitePocketFocus.jpg",     @"..\..\Resources\Images\WhitePocketFocus.jpg");

      #endregion
    }

    public static void UploadFile(CloudBlobContainer blobContainer, string blobName, string filePath, string contentType = null)
    {
      CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(blobName);
      using (var fileStream = File.OpenRead(filePath))
      {
        if (!String.IsNullOrEmpty(contentType)) { blockBlob.Properties.ContentType = contentType; }
        blockBlob.UploadFromStream(fileStream);
      }
      Console.WriteLine("URL:{0}, ETag:{1}", blockBlob.Uri, blockBlob.Properties.ETag);
    }
  }
}
