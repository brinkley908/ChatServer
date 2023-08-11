using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;

namespace ChatServer
{
    public class Talk
    {

        [FunctionName("talk")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "talk")] HttpRequest req,
            [SignalR(HubName = "simplechat")] IAsyncCollector<SignalRMessage> questionR,
            ILogger log)
        {
            try
            {

                var json = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic obj = JsonConvert.DeserializeObject(json);

                var jObject = new JObject(obj);

                await questionR.AddAsync(new SignalRMessage
                {
                    Target = "newMessage",
                    Arguments = new[] { jObject }
                });


                return new OkObjectResult(obj);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}
