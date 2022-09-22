using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;
using Newtonsoft.Json;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using System;

namespace FunctionApp
{
    public static class OrderItemsReserver
    {
        [FunctionName("OrderItemsReserver")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("Order items request received");

            string storageConfigConnectionString = System.Environment.GetEnvironmentVariable("BlobConnectionString");
            string storageConfigFileContainerName = System.Environment.GetEnvironmentVariable("BlobFileContainerName");

            var order = await new StreamReader(req.Body).ReadToEndAsync();        
            var orderId = JsonConvert.DeserializeObject<Order>(order).Id;

            var blobServiceClient = new BlobServiceClient(storageConfigConnectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(storageConfigFileContainerName);
            await containerClient.CreateIfNotExistsAsync();
            var blobClient = containerClient.GetBlobClient($"Order {orderId}");
            await blobClient.UploadAsync(BinaryData.FromString(order), overwrite: true);

            var responseMessage = "Order items request successfully uploaded to blob storage";
            log.LogInformation(responseMessage);
            return new OkObjectResult(responseMessage);
        }
    }
}
