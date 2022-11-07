using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;

namespace DeliveryOrderProcessorFunctionApp
{
    public static class DeliveryOrderProcessorFunc
    {
        [FunctionName("DeliveryOrderProcessor")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("Order details received");
            string endpoint = Environment.GetEnvironmentVariable("CosmosEndPoint");
            string key = Environment.GetEnvironmentVariable("CosmosKey");
            var responseMessage = "";
            try
            {
                var payload = await new StreamReader(req.Body).ReadToEndAsync();             
                var cosmosOrder = OrderDetailInfo.FromOrder(JsonConvert.DeserializeObject<Order>(payload));
                using CosmosClient client = new CosmosClient(endpoint, key);
                var databaseResponse = await client.CreateDatabaseIfNotExistsAsync("OrdersDb");
                ContainerResponse container = await databaseResponse.Database.CreateContainerIfNotExistsAsync(id: "Orders", partitionKeyPath: "/id");
                await container.Container.UpsertItemAsync(cosmosOrder, new PartitionKey(cosmosOrder.id));
                responseMessage = "Order details successfully uploaded to database";
                log.LogInformation(responseMessage);
            } catch (Exception ex) {
                responseMessage = "Order details save failed";
                log.LogError(ex, responseMessage);
                throw;
            }
         
            return new OkObjectResult(responseMessage);
        }
    }
}
