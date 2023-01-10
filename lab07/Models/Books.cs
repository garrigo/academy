using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace lab07.Models
{
    public class Book {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? id {get;set;}
        [BsonElement("Name")]
        public string BookName { get; set; } //Ocio

        public decimal Price { get; set; }
        public string Category { get; set; }
        public string Author { get; set; }
    }

}