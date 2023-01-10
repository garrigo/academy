using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using lab10.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace lab10.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    // Un solo client per controller
    private readonly IHttpClientFactory _httpClientFactory;

    public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    public IActionResult Index(int Page = 1)
    {
        var client = _httpClientFactory.CreateClient(name: "lab10.api");
        HttpRequestMessage req = new HttpRequestMessage()
        {
            Method = HttpMethod.Get
            
            //,RequestUri = new Uri("http://localhost:5134")
        };
        client.BaseAddress = new Uri($"{client.BaseAddress}/Get/Page/{Page-1}");
        int PageNumber = GetPageNumber();
        var res = client.SendAsync(req).Result;
        var dati = res.Content.ReadFromJsonAsync<IEnumerable<Log>>().Result;
        if (dati.Count() == 0)
            return RedirectToAction("Index");
        TempData["PageNumber"] = PageNumber;
        TempData["Page"] = Page;
        return View(dati);
    }

    public int GetPageNumber()
    {
        var client = _httpClientFactory.CreateClient(name: "lab10.api");
        HttpRequestMessage req = new HttpRequestMessage()
        {
            Method = HttpMethod.Get

            //,RequestUri = new Uri("http://localhost:5134")
        };
        client.BaseAddress = new Uri($"{client.BaseAddress}/Get/Page/Number");
        var res = client.SendAsync(req).Result;
        return  res.Content.ReadFromJsonAsync<int>().Result;
    }
    public IActionResult Details(string Id)
    {
        var client = _httpClientFactory.CreateClient(name: "lab10.api");
        HttpRequestMessage req = new HttpRequestMessage()
        {
            Method = HttpMethod.Get
            //,RequestUri = new Uri("")
        };
        client.BaseAddress = new Uri($"{client.BaseAddress}/Get/{Id}");
        var res = client.SendAsync(req).Result;
        try
        {
            res.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e.Message);
            return View(null);
        }
        var log = res.Content.ReadFromJsonAsync<Log>().Result;

        return View(log);
    }

    public IActionResult FromRX(string Rx)
    {
        var client = _httpClientFactory.CreateClient(name: "lab10.api");
        HttpRequestMessage req = new HttpRequestMessage()
        {
            Method = HttpMethod.Get
            //,RequestUri = new Uri("")
        };
        client.BaseAddress = new Uri($"{client.BaseAddress}/Get/RX/{Rx}");
        var res = client.SendAsync(req).Result;
        try
        {
            res.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e.Message);
            return View(new List<Log>());
        }
        var logs = res.Content.ReadFromJsonAsync<IEnumerable<Log>>().Result;
        return View(logs);
    }

    public IActionResult AddNew(Log log)
    {
        var client = _httpClientFactory.CreateClient(name: "lab10.api");
        client.BaseAddress = new Uri($"{client.BaseAddress}/Put");
        //Log log = new()
        //{
        //    Data = DateTime.Today,
        //    NominativoRX = "RX Proxy",
        //    NominativoTX = "TX Proxy",
        //    Status = 1,
        //    ProgressivoSessione = 14
        //};
        var res = client.PutAsJsonAsync(client.BaseAddress, log).Result;
        try
        {
            res.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e.Message);
            return View("Details", null);
        }
        //return View("Details", log);
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> AddMany()
    {
        var client = _httpClientFactory.CreateClient(name: "lab10.api");
        client.BaseAddress = new Uri($"{client.BaseAddress}/Put");
        for (int i = 101; i<10000; i++)
        {
            Log log = new()
            {
                Data = DateTime.Today,
                NominativoRX = $"RX {i}",
                NominativoTX = $"TX {i}",
                Status = i % 2,
                ProgressivoSessione = i
            };
            var res = await client.PutAsJsonAsync(client.BaseAddress, log);
            try
            {
                res.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(e.Message);
                return RedirectToAction("Index");
            }
        }
        //return View("Details", log);
        return RedirectToAction("Index");
    }

    public IActionResult Modify(Log log)
    {
        var client = _httpClientFactory.CreateClient(name: "lab10.api");
        client.BaseAddress = new Uri($"{client.BaseAddress}/Update/{log.id}");
        //Log log = new()
        //{
        //    id = Id,
        //    Data = DateTime.Today,
        //    NominativoRX = "RX Modified",
        //    NominativoTX = "TX Modified",
        //    Status = 0,
        //    ProgressivoSessione = 10
        //};
        var res = client.PostAsJsonAsync(client.BaseAddress, log).Result;
        try
        {
            res.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e.Message);
            return View("Details", null);
        }
        //return View("Details", log);
        return RedirectToAction("Index");
    }

    public IActionResult Delete(string Id)
    {
        var client = _httpClientFactory.CreateClient(name: "lab10.api");
        HttpRequestMessage req = new HttpRequestMessage()
        {
            Method = HttpMethod.Delete
            //,RequestUri = new Uri("")
        };
        client.BaseAddress = new Uri($"{client.BaseAddress}/Delete/{Id}");
        var res = client.SendAsync(req).Result;
        try
        {
            res.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e.Message);
            return RedirectToAction("Index");
        }
        //return View("Index", null);
        return RedirectToAction("Index");
    }

    public IActionResult Edit(string? Id, int Flag = 0)
    {
        var client = _httpClientFactory.CreateClient(name: "lab10.api");
        HttpRequestMessage req = new HttpRequestMessage()
        {
            Method = HttpMethod.Get
            //,RequestUri = new Uri("")
        };
        if (Flag == 0)
        {
            client.BaseAddress = new Uri($"{client.BaseAddress}/Get/{Id}");
            var res = client.SendAsync(req).Result;
            try
            {
                res.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(e.Message);
                return RedirectToAction("Index");
            }
            var log = res.Content.ReadFromJsonAsync<Log>().Result;
            return View(log);
        }
        return View(null);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
