using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Printer.Models;

namespace Printer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PrinterController : Controller
    {

        private readonly ILogger<PrinterController> _logger;
        // Un solo client per controller
        private readonly IHttpClientFactory _httpClientFactory;

        public PrinterController(ILogger<PrinterController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        [HttpPut()]
        public IActionResult Print([FromBody] Log JSONData)
        {
            // do stuff
            // start delivery
            //var client = _httpClientFactory.CreateClient(name:"PrintServer.api");
            var client = _httpClientFactory.CreateClient(name: "manager.api");
            var response = client.PutAsJsonAsync(client.BaseAddress, JSONData).Result;


            var client2 = _httpClientFactory.CreateClient(name: "delivery.api");
            var response2 = client2.PutAsJsonAsync(client2.BaseAddress, JSONData).Result;

            if (!response.IsSuccessStatusCode)
                return BadRequest(response);
            if (!response2.IsSuccessStatusCode)
                return BadRequest(response);
            return Ok(JSONData);
        }
    }
}
