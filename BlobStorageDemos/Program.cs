using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using BlobStorageDemos.Advanced;
using BlobStorageDemos.Basics;
using BlobStorageDemos.Performance;
using BlobStorageDemos.Security;

namespace BlobStorageDemos
{
  public class Program
  {
    #region Main

    //
    // Main loop is modified from example in: StreamInsight Product Team Samples V2.1
    // http://streaminsight.codeplex.com/releases/view/90143
    //
    public static void Main(string[] args)
    {
      try
      {
        var demos = (from mi in typeof (Program).GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
                     let nameAttr = mi.GetCustomAttributes(typeof (DisplayNameAttribute), false)
                                      .OfType<DisplayNameAttribute>()
                                      .SingleOrDefault()
                     let descriptionAttr = mi.GetCustomAttributes(typeof (DescriptionAttribute), false)
                                             .OfType<DescriptionAttribute>()
                                             .SingleOrDefault()
                     where null != nameAttr
                     select new {Action = mi, Name = nameAttr.DisplayName}).ToArray();

        while (true)
        {
          var lastSectionName = "";

          Console.Clear();
          Console.WriteLine();
          Console.WriteLine("Pick a demo to run:");
          for (int i = 0; i < demos.Length; i++)
          {
            var sectionName = demos[i].Name.Split(':').First();
            if (!lastSectionName.Equals(sectionName))
            {
              Console.WriteLine("\n---------------------------------------------------------------\n");
              lastSectionName = sectionName;
            }

            Console.WriteLine("{0,4} - {1}", i + 1, demos[i].Name);
          }

          Console.WriteLine("\n---------------------------------------------------------------\n");
          Console.WriteLine("{0,4} - {1}\n", "Q", "Quit");
          var response = Console.ReadLine().Trim();
          if (string.Equals(response, "q", StringComparison.OrdinalIgnoreCase))
          {
            break;
          }

          int demoToRun;
          demoToRun = Int32.TryParse(response, NumberStyles.Integer, CultureInfo.InvariantCulture, out demoToRun)
                        ? demoToRun - 1
                        : -1;

          if (0 <= demoToRun && demoToRun < demos.Length)
          {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine(demos[demoToRun].Name);
            Console.WriteLine(@"------------------------------------------------------------");
            Console.WriteLine();

            demos[demoToRun].Action.Invoke(null, null);

            Console.WriteLine();
            Console.WriteLine("Press any key to return to menu ...");
            Console.ReadLine();
          }
          else
          {
            Console.WriteLine("Unknown Demo");
          }
        }
      }
      catch (Exception exception)
      {
        if (exception.InnerException != null)
        {
          if (exception.InnerException.Source.Equals("Microsoft.WindowsAzure.Storage") && exception.InnerException.GetType() == typeof (System.FormatException))
          {
            Console.WriteLine("Invalid Blob Storage connection string\n");
            Console.WriteLine("Ensure that you have replaced the ACCOUNT_NAME and ACCOUNT_KEY values in the StorageConnectionString appSetting in App.config.\n");
          }
        }
        else
        {
          throw;
        }
      }
    }

    #endregion Main

    #region Basics

    [DisplayName("Basics: Create Container")]
    private static void Execute_01() { _01_CreateContainer.Run(); }

    [DisplayName("Basics: Upload Blobs")]
    private static void Execute_02() { _02_UploadBlobs.Run(); }

    [DisplayName("Basics: List Blobs")]
    private static void Execute_03() { _03_ListBlobs.Run(); }

    [DisplayName("Basics: Download Blob")]
    private static void Execute_04() { _04_DownloadBlob.Run(); }

    [DisplayName("Basics: Delete Blob")]
    private static void Execute_05() { _05_DeleteBlob.Run(); }

    #endregion Basics

    #region Advanced

    [DisplayName("Advanced: Blob Properties")]
    private static void Execute_06() { _06_BlobProperties.Run(); }

    [DisplayName("Advanced: Blob Metadata")]
    private static void Execute_07() { _07_BlobMetadata.Run(); }

    [DisplayName("Advanced: Manage Concurrency with ETags")]
    private static void Execute_08() { _08_ConcurrencyWithETag.Run(); }

    [DisplayName("Advanced: Manage Concurrency with Lease")]
    private static void Execute_09() { _09_ConcurrencyWithLease.Run(); }

    [DisplayName("Advanced: Snapshots")]
    private static void Execute_10() { _10_Snapshots.Run(); }

    #endregion Advanced

    #region Performance

    [DisplayName("Performance: Parallel Blob Upload")]
    private static void Execute_11() { _11_ParallelBlobUpload.Run(); }

    #endregion Performance

    #region Security

    [DisplayName("Security: Shared Key")]
    private static void Execute_12() { _12_SharedKey.Run(); }

    [DisplayName("Security: Shared Access Signature")]
    private static void Execute_13() { _13_SharedAccessSignature.Run(); }

    [DisplayName("Security: Shared Access Policy")]
    private static void Execute_14() { _14_SharedAccessPolicy.Run(); }

    #endregion Security

  }
}
