using Delivery.Models;
using Microsoft.AspNetCore.Mvc;

namespace Delivery.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DeliveryController : Controller
    {
        private readonly ILogger<DeliveryController> _logger;
        // Un solo client per controller
        private readonly IHttpClientFactory _httpClientFactory;

        public DeliveryController(ILogger<DeliveryController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }
        [HttpPut()]
        public IActionResult DeliverRequest([FromBody] Log JSONData)
        {
            // do stuff
            // start delivery
            //var client = _httpClientFactory.CreateClient(name:"PrintServer.api");
            var client = _httpClientFactory.CreateClient(name: "manager.api");
            var response = client.PutAsJsonAsync(client.BaseAddress, JSONData).Result;
            if (!response.IsSuccessStatusCode)
                return BadRequest(response);
            return Ok(JSONData);
        }
    }
}
