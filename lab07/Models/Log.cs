using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace lab07.Models
{
    public class Log
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? id { get; set; }
        public DateTime Data { get; set; }

        public string NominativoRX { get; set; }
        public string NominativoTX { get; set; }
        public int Status { get; set; }
        public int ProgressivoSessione { get; set; }
    }

}