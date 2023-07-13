using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ContractManagement.DataAccess.Models
{
    [Serializable, BsonIgnoreExtraElements]
    public class EmailRequestModel
    {

        [BsonId, BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("recipient_email")]
        public string Recipient { get; set; }

        [BsonElement("subject")]
        public string Subject { get; set; }

        [BsonElement("body")]
        public string Body { get; set; }
        //public ICollection<Body> Content { get; set; }

        [BsonElement("scheduled_time")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime ScheduledTime { get; set; }

        [BsonElement("status")]
        [BsonRepresentation(BsonType.String)]
        public string Status { get; set; }
    }

    public class Body
    {
        [BsonId, BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string PolicyNumber { get; set; }
       /* public DateTime Created { get; set; }
        public DateTime DueDate { get; set; }*/
    }
}
