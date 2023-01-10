using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing.Models
{
    public class Log
    {
        public string? id { get; set; }
        public DateTime data { get; set; }

        public string? nominativoRX { get; set; }
        public string? nominativoTX { get; set; }
        public int status { get; set; }
        public int progressivoSessione { get; set; }
    }
}
