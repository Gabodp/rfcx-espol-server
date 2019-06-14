using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace WebApplication.Models
{

    public class AudioUnity: IComparable{

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string AudioId { get; set; }
        public int Id { get; set; }
        public string Description { get; set; }
    

        public int CompareTo(object obj)
        {
            AudioUnity audio = obj as AudioUnity;
            if(audio == null) return 1;
            
            if (audio.Id < Id)
            {
            return 1;
            }
            if (audio.Id > Id)
            {
                return -1;
            }
            return 0;
        }
    }
}
