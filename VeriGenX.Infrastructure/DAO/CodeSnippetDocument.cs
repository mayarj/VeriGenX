using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace VeriGenX.Infrastructure.DAO
{
    public class CodeSnippetDocument
    {

        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string  SnippetId { get;  set; }
        public string Title { get;  set; }
        public string Description { get; set; }
        public string VerilogCode { get;  set; }
        public string TestBench { get;  set; }
        public DateTime CreatedDate { get;  set; }
        public DateTime LastModified { get; set; }

        public TestResultDocument? TestResultDocument { get; set; }

        public WaveformDataDocument? WaveformDataDocument { get; set; }

        [BsonRepresentation(BsonType.String)]
        public string  ProjectId { get; set; }  // Foreign key reference to ProjectDocument
    }
}
