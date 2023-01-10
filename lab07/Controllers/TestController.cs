using lab07.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace lab07.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : Controller
    {
        
        private readonly IOptions<DatabaseSettings> _databaseSettings;

        // Istanza di Controller viene creata A OGNI HTTP REQUEST
        public TestController(IOptions<DatabaseSettings> databaseSettings)
        {
            _databaseSettings = databaseSettings;
        }


        [HttpGet("Books",Name = "GetBooks")]
        public IEnumerable<Book> Get()
        {
            MongoClientSettings settings =
                MongoClientSettings.FromConnectionString(
                    _databaseSettings.Value.ConnectionString
            );
            MongoClient client = new MongoClient(settings);
            var db = client.GetDatabase(_databaseSettings.Value.DataBaseName);
            var cls = db.GetCollection<Book>(_databaseSettings.Value.CollectionName);
            var lsl = cls.Find(_ => true).ToList();
            return lsl;
        }

        [HttpGet("Books/By/{Author}", Name = "GetBooksByAuthor")]
        public IEnumerable<Book> GetByAuthor(string? Author)
        {
            MongoClientSettings settings =
                MongoClientSettings.FromConnectionString(
                    _databaseSettings.Value.ConnectionString
            );
            MongoClient client = new MongoClient(settings);
            var db = client.GetDatabase(_databaseSettings.Value.DataBaseName);
            var cls = db.GetCollection<Book>(_databaseSettings.Value.CollectionName);
            var lsl = cls.Find(b => b.Author == Author).ToList();
            return lsl;
        }
        // https://stackoverflow.com/questions/64210010/the-request-matched-multiple-endpoints-on-net-core
        [HttpGet("Books/Top/ByPrice/{Top}", Name = "GetTopNBooksByPrice")]
        public IEnumerable<Book> GetTopNBooksByPrice(int Top)
        {
            MongoClientSettings settings =
                MongoClientSettings.FromConnectionString(
                    _databaseSettings.Value.ConnectionString
            );
            MongoClient client = new MongoClient(settings);
            var db = client.GetDatabase(_databaseSettings.Value.DataBaseName);
            var cls = db.GetCollection<Book>(_databaseSettings.Value.CollectionName);
            var lsl = cls.Find(b => true).ToList().OrderBy(b => b.Price).Take(Top);
            return lsl;
        }

        [HttpGet("Books/Min/Price/{Min}", Name = "GetBooksMinPrice")]
        public IEnumerable<Book> GetBooksMinPrice(decimal Min)
        {
            MongoClientSettings settings =
                MongoClientSettings.FromConnectionString(
                    _databaseSettings.Value.ConnectionString
            );
            MongoClient client = new MongoClient(settings);
            var db = client.GetDatabase(_databaseSettings.Value.DataBaseName);
            var cls = db.GetCollection<Book>(_databaseSettings.Value.CollectionName);
            var lsl = cls.Find(b => b.Price >= Min).ToList();
            return lsl;
        }

        [HttpGet("Books/Max/Price/{Max}", Name = "GetBooksMaxPrice")]
        public IEnumerable<Book> GetBooksMaxPrice(decimal Max)
        {
            MongoClientSettings settings =
                MongoClientSettings.FromConnectionString(
                    _databaseSettings.Value.ConnectionString
            );
            MongoClient client = new MongoClient(settings);
            var db = client.GetDatabase(_databaseSettings.Value.DataBaseName);
            var cls = db.GetCollection<Book>(_databaseSettings.Value.CollectionName);
            var lsl = cls.Find(b => b.Price <= Max).ToList();
            return lsl;
        }

        [HttpGet("Books/Range/Price/{Min}&{Max}", Name = "GetBooksRangePrice")]
        public IEnumerable<Book> GetBooksRangePrice(decimal Min, decimal Max)
        {
            MongoClientSettings settings =
                MongoClientSettings.FromConnectionString(
                    _databaseSettings.Value.ConnectionString
            );
            MongoClient client = new MongoClient(settings);
            var db = client.GetDatabase(_databaseSettings.Value.DataBaseName);
            var cls = db.GetCollection<Book>(_databaseSettings.Value.CollectionName);
            var lsl = cls.Find(b => b.Price >= Min && b.Price <= Max).ToList();
            return lsl;
        }
    }
}
