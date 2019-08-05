using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Xunit;

namespace StorageAccountTests
{
    public class BlobContainerTest
    {
        private CloudBlobContainer container;
        public BlobContainerTest()
        {
            var storageConnectionString =
                "DefaultEndpointsProtocol=https;"
                + "AccountName=<YOUR_ACCOUNT_NAME>"
                + ";AccountKey=<YOUR_ACCOUNT_KEY>"
                + ";EndpointSuffix=core.windows.net";

            var account = CloudStorageAccount.Parse(storageConnectionString);
            var serviceClient = account.CreateCloudBlobClient();

            container = serviceClient.GetContainerReference("mycontainer");
        }
        [Fact]
        public async Task ShouldCreateBlobAsync()
        {
            // Create container. Name must be lower case.
            Console.WriteLine("Creating container...");
            await container.CreateIfNotExistsAsync();

            // write a blob to the container
            CloudBlockBlob blob = container.GetBlockBlobReference("helloworld.txt");
            blob.UploadTextAsync("Hello, World!").Wait();
        }

        [Fact]
        public async Task ShouldListBlobsContainersAsync()
        {
            Assert.True(await container.ExistsAsync());

            container.CreateIfNotExistsAsync().Wait();

            Console.WriteLine("List blobs in container.");
            BlobContinuationToken blobContinuationToken = null;
            do
            {
                var results = await container.ListBlobsSegmentedAsync("", blobContinuationToken);
                // Get the value of the continuation token returned by the listing call.
                blobContinuationToken = results.ContinuationToken;
                foreach (IListBlobItem item in results.Results)
                {
                    Console.WriteLine(item.Uri);
                }
            } while (blobContinuationToken != null); // Loop while the continuation token is not null.
        }

        [Fact]
        public async Task ShouldDeleteBlobContainerAsync()
        {
            Assert.True(await container.ExistsAsync());

            string BlobName = "helloworld.txt";

            CloudBlobDirectory blobDirectory = container.GetDirectoryReference("");
            // get block blob refarence    
            CloudBlockBlob blockBlob = blobDirectory.GetBlockBlobReference(BlobName);

            // delete blob from container        
            await blockBlob.DeleteAsync();
        }

        [Fact]
        public async Task ShouldDeleteContainerAsync()
        {
            Assert.True(await container.ExistsAsync());

            // delete blob from container        
            await container.DeleteAsync();
        }
    }
}
