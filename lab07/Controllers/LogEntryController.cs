using lab07.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace lab07.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LogEntryController : Controller
    {
        private readonly IOptions<DatabaseSettings> _databaseSettings;
        private readonly int PageLen;

        // Istanza di Controller viene creata A OGNI HTTP REQUEST
        public LogEntryController(IOptions<DatabaseSettings> databaseSettings)
        {
            _databaseSettings = databaseSettings;
            PageLen = 20;
        }


        [HttpGet("Get/All", Name = nameof(GetAllLogs))]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Log>))]
        public IActionResult GetAllLogs()
        {
            MongoClientSettings settings =
                MongoClientSettings.FromConnectionString(
                    _databaseSettings.Value.ConnectionString
            );
            MongoClient client = new MongoClient(settings);
            var db = client.GetDatabase(_databaseSettings.Value.DataBaseName);
            var cls = db.GetCollection<Log>(_databaseSettings.Value.CollectionName);
            var lsl = cls.Find(_ => true).ToList();
            return Ok(lsl);
        }

        [HttpGet("Get/Page/{Page}", Name = nameof(GetLogsPage))]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Log>))]
        public IActionResult GetLogsPage(int? Page)
        {
            MongoClientSettings settings =
                MongoClientSettings.FromConnectionString(
                    _databaseSettings.Value.ConnectionString
            );
            MongoClient client = new MongoClient(settings);
            var db = client.GetDatabase(_databaseSettings.Value.DataBaseName);
            var cls = db.GetCollection<Log>(_databaseSettings.Value.CollectionName);
            var lsl = cls.Find(_ => true).Skip(Page * PageLen).Limit(PageLen).ToList();
            return Ok(lsl);
        }

        [HttpGet("Get/Page/Number", Name = nameof(GetPageNumber))]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public IActionResult GetPageNumber()
        {
            MongoClientSettings settings =
                MongoClientSettings.FromConnectionString(
                    _databaseSettings.Value.ConnectionString
            );
            MongoClient client = new MongoClient(settings);
            var db = client.GetDatabase(_databaseSettings.Value.DataBaseName);
            var cls = db.GetCollection<Log>(_databaseSettings.Value.CollectionName);
            long PageNumber;
            try
            {
                PageNumber = cls.CountDocuments(_ => true)/PageLen;
            }
            catch
            {
                return BadRequest(); // 400
            }

            return Ok(PageNumber); // 200
        }

        [HttpGet("Get/{Id}", Name = nameof(GetLog))]
        [ProducesResponseType(200, Type=typeof(Log))]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public IActionResult GetLog(string? Id)
        {
            MongoClientSettings settings =
                MongoClientSettings.FromConnectionString(
                    _databaseSettings.Value.ConnectionString
            );
            MongoClient client = new MongoClient(settings);
            var db = client.GetDatabase(_databaseSettings.Value.DataBaseName);
            var cls = db.GetCollection<Log>(_databaseSettings.Value.CollectionName);
            Log log;
            try
            {
                log = cls.Find(x => x.id == Id).FirstOrDefault();
            }
            catch
            {
                return BadRequest($"Invalid input: {Id}"); // 400
            }
            if (log == null)
                return NotFound($"{Id} not found."); // 404 todo: move to async

            return Ok(log); // 200
        }

        [HttpGet("Get/RX/{RX}", Name = nameof(GetLogRX))]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Log>))]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public IActionResult GetLogRX(string? RX)
        {
            MongoClientSettings settings =
                MongoClientSettings.FromConnectionString(
                    _databaseSettings.Value.ConnectionString
            );
            MongoClient client = new MongoClient(settings);
            var db = client.GetDatabase(_databaseSettings.Value.DataBaseName);
            var cls = db.GetCollection<Log>(_databaseSettings.Value.CollectionName);
            List<Log> logs;
            try
            {
                logs = cls.Find(x => x.NominativoRX == RX).ToList();
            }
            catch
            {
                return BadRequest($"Invalid input: {RX}"); // 400
            }
            if (logs == null)
                return NotFound($"{RX} not found."); // 404 todo: move to async

            return Ok(logs); // 200
        }


        [HttpPut("Put/Random", Name = nameof(PutRandomLogs))]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Log>))]
        public IActionResult PutRandomLogs()
        {
            MongoClientSettings settings =
                MongoClientSettings.FromConnectionString(
                    _databaseSettings.Value.ConnectionString
            );
            MongoClient client = new MongoClient(settings);
            var db = client.GetDatabase(_databaseSettings.Value.DataBaseName);
            var cls = db.GetCollection<Log>(_databaseSettings.Value.CollectionName);
            
            List<Log> addenda = new List<Log>();
            if (cls.CountDocuments(_ => true) == 0)
            {
                for (int i = 1; i < 101; i++)
                    addenda.Add(new Log
                    {
                        Data = DateTime.Today,
                        NominativoRX = $"RX {i}",
                        NominativoTX = $"TX {i}",
                        Status = i % 3,
                        ProgressivoSessione = i % 3
                    });

                cls.InsertMany(addenda);
            }
            return Ok(addenda);
        }

        [HttpPut("Put", Name = nameof(PutLog))]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult PutLog([FromBody] Log log)
        {
            MongoClientSettings settings =
                MongoClientSettings.FromConnectionString(
                    _databaseSettings.Value.ConnectionString
            );
            settings.LinqProvider = MongoDB.Driver.Linq.LinqProvider.V3;
            MongoClient client = new MongoClient(settings);
            var db = client.GetDatabase(_databaseSettings.Value.DataBaseName);
            var cls = db.GetCollection<Log>(_databaseSettings.Value.CollectionName);
            try { cls.InsertOne(log); }
            catch { return BadRequest($"Invalid input"); } //400
            return Ok();
        }

        [HttpPost("Update/{Id}", Name = nameof(UpdateLog))]
        [ProducesResponseType(200, Type = typeof(Log))]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public IActionResult UpdateLog(string? Id, [FromBody] Log log)
        {
            MongoClientSettings settings =
                MongoClientSettings.FromConnectionString(
                    _databaseSettings.Value.ConnectionString
            );
            MongoClient client = new MongoClient(settings);
            var db = client.GetDatabase(_databaseSettings.Value.DataBaseName);
            var cls = db.GetCollection<Log>(_databaseSettings.Value.CollectionName);
            ReplaceOneResult updated;
            try
            {
                updated = cls.ReplaceOne(x => x.id == Id, log);
            }
            catch
            {
                return BadRequest($"Invalid input: {Id}"); // 400
            }
            if (updated.MatchedCount == 0)
                return NotFound($"{Id} not found."); // 404 

            return Ok(cls.Find(x => x.id == Id) /* log */); // 200
        }

        [HttpDelete("Delete/All", Name = nameof(DeleteAll))]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Log>))]
        public IActionResult DeleteAll()
        {
            MongoClientSettings settings =
                MongoClientSettings.FromConnectionString(
                    _databaseSettings.Value.ConnectionString
            );
            MongoClient client = new MongoClient(settings);
            var db = client.GetDatabase(_databaseSettings.Value.DataBaseName);
            var cls = db.GetCollection<Log>(_databaseSettings.Value.CollectionName);

            cls.DeleteMany(_ => true);
            return Ok(cls.Find(_ => true).ToList());
        }

        [HttpDelete("Delete/{Id}", Name = nameof(DeleteLog))]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public IActionResult DeleteLog(string? Id)
        {
            MongoClientSettings settings =
                MongoClientSettings.FromConnectionString(
                    _databaseSettings.Value.ConnectionString
            );
            MongoClient client = new MongoClient(settings);
            var db = client.GetDatabase(_databaseSettings.Value.DataBaseName);
            var cls = db.GetCollection<Log>(_databaseSettings.Value.CollectionName);
            long deleted;
            try
            {
                deleted = cls.DeleteOne(x => x.id == Id).DeletedCount;
            }
            catch
            {
                return BadRequest($"Invalid input: {Id}"); // 400
            }
            if (deleted == 0)
                return NotFound($"{Id} not found."); // 404 todo: move to async

            return Ok(Id); // 200
        }
    }
}
