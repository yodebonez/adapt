using Adapt.Custodian.Lib.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System.Text;

namespace Adapt.Custodian.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public OrdersController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrderAsync([FromBody] Order order)
        {
            IQueueClient queueClient = new QueueClient(_configuration["QueueConnectionString"], _configuration["QueueName"]);
            var orderJSON = JsonConvert.SerializeObject(order);
            var orderMessage = new Message(Encoding.UTF8.GetBytes(orderJSON))
            {
                MessageId = Guid.NewGuid().ToString(),
                ContentType = "application/json"
            };
            await queueClient.SendAsync(orderMessage).ConfigureAwait(false);

            return Ok("Create order message has been successfully pushed to queue");
        }
    }
}
