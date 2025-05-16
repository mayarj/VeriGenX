using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace VeriGenX.Infrastructure.DAO
{
    public class UserDocument
    {

        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; }


        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        //protected UserDocument() { }
    }
}
