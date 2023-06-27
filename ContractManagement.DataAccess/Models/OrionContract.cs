using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ContractManagement.DataAccess.Models
{
    [Serializable, BsonIgnoreExtraElements]
    public class OrionContract
    {
        [BsonId, BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public Events[] Events { get; set; }
        public string PolicyNumber { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string ClientId{ get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string QuotationId { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string ActiveQuotationId { get; set; }
        public DateTime ContractDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime EffectDate { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string IntermediaryId { get; set; }
      

    }
   
    public class Events
    {
        public string Who { get; set; }
        public DateTime When { get; set; }
        public string Evt { get; set; }
    }

    
}
