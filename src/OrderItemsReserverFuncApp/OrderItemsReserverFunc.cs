using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Storage.Blobs;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using System.Threading.Tasks;

namespace OrderItemsReserverFuncApp
{
    public class OrderItemsReserverFunc
    {
        [FunctionName("OrderItemsReserver")]
        public async Task Run([ServiceBusTrigger("sersid-queue", Connection = "ServiceBusConnection", AutoCompleteMessages = true)]string serviceBusMessage, ILogger log)
        {
            log.LogInformation("Order items reservation request received");

            var storageConfigConnectionString = Environment.GetEnvironmentVariable("BlobConnectionString");
            var storageConfigFileContainerName = Environment.GetEnvironmentVariable("BlobFileContainerName");

            try
            {
                var reservationId = JsonConvert.DeserializeObject<Order>(serviceBusMessage)?.Id;
                var blobServiceClient = new BlobServiceClient(storageConfigConnectionString);
                var containerClient = blobServiceClient.GetBlobContainerClient(storageConfigFileContainerName);
                //await containerClient.CreateIfNotExistsAsync();// commented out to provoke an error when container does not exist
                var blobClient = containerClient.GetBlobClient($"Reservation {reservationId}");
                await blobClient.UploadAsync(BinaryData.FromString(serviceBusMessage), overwrite: true);
                var responseMessage = "Order items reservation successfully uploaded to blob storage";
                log.LogInformation(responseMessage);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Order items reservation failed");
                throw;
            }     
        }
    }
}
