using ContractManagement.BusinessLogic.Services.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;
using IMailService = ContractManagement.BusinessLogic.Services.Interfaces.IMailService;

namespace ContractManagement.BusinessLogic.Services
{
    public class SendPendingEmails : ISendPendingEmails
    {
        private readonly IMongoDatabase _database;
        private readonly IMailService _mailService;
        public SendPendingEmails(IMongoDatabase database , IMailService mailService)
        {
          _database = database;
            _mailService = mailService;
        }
        public async Task SendPendingEmail(CancellationToken cancellationToken)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("status", "En attente d'envoi");
            var jobs = await _database.GetCollection<BsonDocument>("Jobs").Find(filter).ToListAsync();
            foreach (var job in jobs)
            {
                var recipientEmail = job["recipient_email"].AsString;
                var subject = job["subject"].AsString;
                var body = job["body"].AsString;
                // Envoi de l'e-mail avec la liste des contrats à échéance
                await _mailService.SendEmailAsync(recipientEmail, subject, body, cancellationToken);
                var jobId = job["_id"].AsObjectId;
                var updateFilter = Builders<BsonDocument>.Filter.Eq("_id", jobId);
                var update = Builders<BsonDocument>.Update.Set("status", "Envoyé");
                await _database.GetCollection<BsonDocument>("Jobs").UpdateOneAsync(updateFilter, update);
            }
        }
    }
}
