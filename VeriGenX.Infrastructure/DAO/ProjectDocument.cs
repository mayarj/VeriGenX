using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace VeriGenX.Infrastructure.DAO
{
    public class ProjectDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string   ProjectId { get;set; }
        [BsonRepresentation(BsonType.String)]
        public string  UserId { get;set; }
        public string Name { get; set; }
        public string Description { get;  set; }
        public DateTime CreatedDate { get;set; }
        public DateTime LastModified { get; set; }
      

    }
}
