using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Financial.Bot.Domain
{
    public class Coin
    {
        public ObjectId Id { get;set;}

        [BsonElement("symbol")]
        public string Symbol { get; set;}

        [BsonElement("name_id")]
        public string NameId { get; set; }

        [BsonElement("name")]
        public string Name { get; set;}

        [BsonElement("source")]
        public string Source { get; set;}

        [BsonElement("info")]
        public string Info { get; set; }

        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set;}

        [BsonElement("updated_at")]
        public DateTime UpdatedAt { get; set;}
    }
}
