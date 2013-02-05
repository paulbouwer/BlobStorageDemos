using System;
using System.IO;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace BlobStorageDemos.Basics
{
  public class _04_DownloadBlob
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

      #region Download Blob

      DownloadBlob(docContainer, @"WhatIsBlobStorage.txt",  @"C:\Temp\WhatIsBlobStorage.txt");
      DownloadBlob(docContainer, @"Readme.txt",             @"C:\Temp\Readme.txt");

      #endregion      
    }

    public static void DownloadBlob(CloudBlobContainer blobContainer, string blobName, string filePath)
    {
      CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(blobName);
      using (var fileStream = File.OpenWrite(filePath))
      {
        blockBlob.DownloadToStream(fileStream);
      }

      var fileInfo = new FileInfo(filePath);
      Console.WriteLine("Name:{0}, File Last Modified(UTC):{1}", fileInfo.FullName, fileInfo.LastWriteTimeUtc);
    }
  }
}
