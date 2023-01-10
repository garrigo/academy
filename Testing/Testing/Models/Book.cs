using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


// Install MongoDB.Driver
// Set up MongoDB at the site
// https://zetcode.com/csharp/mongodb/
namespace Testing.Models
{
    public class Book
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("Name")]
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public string Author { get; set; }

    }
}