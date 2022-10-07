using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using System;
using Microsoft.Azure.Cosmos;

namespace FunctionApp
{
    public static class OrderItemsReserver
    {
        [FunctionName("OrderItemsReserver")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("Create order request received");
            string endpoint = System.Environment.GetEnvironmentVariable("CosmosEndPoint");
            string key = System.Environment.GetEnvironmentVariable("CosmosKey");
            var responseMessage = "";
            try
            {
                var payload = await new StreamReader(req.Body).ReadToEndAsync();             
                var cosmosOrder = CosmosOrder.FromOrder(JsonConvert.DeserializeObject<Order>(payload));
                using CosmosClient client = new CosmosClient(endpoint, key);
                var databaseResponse = await client.CreateDatabaseIfNotExistsAsync("ordersdb");
                ContainerResponse container = await databaseResponse.Database.CreateContainerIfNotExistsAsync(id: "orders", partitionKeyPath: "/id");
                ItemResponse<CosmosOrder> response = await container.Container.CreateItemAsync(cosmosOrder, new PartitionKey(cosmosOrder.id));
                responseMessage = "Order successfully uploaded to cosmos db";
                log.LogInformation(responseMessage);
            } catch (Exception ex) {
                responseMessage = "Order creation failed";
                log.LogError(ex, responseMessage);
                throw new Exception(responseMessage);
            }
         
            return new OkObjectResult(responseMessage);
        }
    }
}
