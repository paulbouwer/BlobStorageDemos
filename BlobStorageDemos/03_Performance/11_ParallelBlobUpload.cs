using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace BlobStorageDemos.Performance
{
  public class _11_ParallelBlobUpload
  {
    public static void Run()
    {
      #region Obtain a client from the connection string

      CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
      CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
      Console.WriteLine("Parsed connection string and created blob client.");

      #endregion

      #region Create a private container called docs

      CloudBlobContainer parallelContainer = blobClient.GetContainerReference("parallel");
      parallelContainer.CreateIfNotExists();
      Console.WriteLine("Created big container with private access.");

      #endregion

      #region Upload blobs

      UploadFile(parallelContainer,         @"PtJudithLight1.jpg", @"..\..\Resources\Images\PtJudithLight.jpg", @"image/jpeg");
      ParallelUploadFile(parallelContainer, @"PtJudithLight2.jpg", @"..\..\Resources\Images\PtJudithLight.jpg", @"image/jpeg");

      #endregion
    }

    public static void UploadFile(CloudBlobContainer blobContainer, string blobName, string filePath, string contentType = null)
    {
      var start = DateTime.Now;

      Console.WriteLine("\nUpload file as single process.");
      CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(blobName);
      using (var fileStream = File.OpenRead(filePath))
      {
        if (!String.IsNullOrEmpty(contentType)) { blockBlob.Properties.ContentType = contentType; }
        blockBlob.UploadFromStream(fileStream);
      }
      Console.WriteLine("URL:{0}, ETag:{1}", blockBlob.Uri, blockBlob.Properties.ETag);

      var timespan = DateTime.Now - start;
      Console.WriteLine("{0} seconds.", timespan.Seconds);
    }

    public static void ParallelUploadFile(CloudBlobContainer blobContainer, string blobName, string filePath, string contentType = null)
    {
      var start = DateTime.Now;

      Console.WriteLine("\nUpload file in parallel.");
      CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(blobName);

      // 2M blocks for demo
      int blockLength = 2 * 1000 * 1024;
      byte[] dataToUpload = File.ReadAllBytes(filePath);
      int numberOfBlocks = (dataToUpload.Length / blockLength) + 1;
      string[] blockIds = new string[numberOfBlocks];
      
      Parallel.For(0, numberOfBlocks, x =>
      {
        var blockId = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        var currentLength = Math.Min(blockLength, dataToUpload.Length - (x * blockLength));

        using (var memStream = new MemoryStream(dataToUpload, x * blockLength, currentLength))
        {
          blockBlob.PutBlock(blockId, memStream, null);
        }
        blockIds[x] = blockId;
        Console.WriteLine("BlockId:{0}", blockId);
      });

      if (!String.IsNullOrEmpty(contentType)) { blockBlob.Properties.ContentType = contentType; }
      blockBlob.PutBlockList(blockIds);
      Console.WriteLine("URL:{0}, ETag:{1}", blockBlob.Uri, blockBlob.Properties.ETag);

      var timespan = DateTime.Now - start;
      Console.WriteLine("{0} seconds.", timespan.Seconds);
    }
  }
}


