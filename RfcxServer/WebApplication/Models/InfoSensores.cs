using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;


namespace WebApplication.Models
{
    public class InfoSensores
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string InfoSensoresId { get; set; }
        public string Timestamp { get; set; }
        public List<Data> Data { get; set; }        
    }
}