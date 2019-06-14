using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace WebApplication.Models
{
    public class GalleryItem : IComparable
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string MongoId { get; set; }
        public int Id { get; set; } //id incrmental 
        public string Description { get; set; }
        public int audioname;//nombre del archivo de audi subido
        public int imagename;//nombre del archivo de imagen subido


        public int CompareTo(object obj)
        {
            GalleryItem item = obj as GalleryItem;
            if(item == null) return 1;
            if (item.Id < Id)
            {
                return 1;
            }
            if (item.Id > Id)
            {
                return -1;
            }
            return 0;
        }
    }
    
}