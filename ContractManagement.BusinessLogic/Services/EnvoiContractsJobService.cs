using ContractManagement.BusinessLogic.Services.Interfaces;
using ContractManagement.DataAccess.Models;
using MongoDB.Driver;
using Quartz;

namespace ContractManagement.BusinessLogic.Services
{
    public class EnvoiContractsJobService : IJob
    {
        private readonly IContractService _contraService;
        private readonly IMailService _mailService;
        private readonly IMongoDatabase _database;

        public EnvoiContractsJobService(IContractService contraService, IMailService mailService, IMongoDatabase database)
        {
            _contraService = contraService;
            _mailService = mailService;
            _database = database;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            // Récupération de la liste des contrats à échéance sur 7 jours
            var contracts = await _contraService.GetContractsByDueDateAsync(30, context.CancellationToken);

            // Création du contenu de l'e-mail avec un tableau HTML
            var emailContent = "<h2>Liste des contrats à échéance :</h2>";
            emailContent += "<table style='border-collapse: collapse;'>";
            emailContent += "<tr><th style='border: 1px solid black; padding: 8px;'>ID Contrat</th> <th style='border: 1px solid black; padding: 8px;'>Numéro de police</th> <th style='border: 1px solid black; padding: 8px;'>Date de création</th><th style='border: 1px solid black; padding: 8px;'>Échéance</th></tr>";

            foreach (var contract in contracts)
            {
                emailContent += $"<tr><td style='border: 1px solid black; padding: 8px;'>{contract.Id}</td><td style='border: 1px solid black; padding: 8px;'>{contract.PolicyNumber}</td><td style='border: 1px solid black; padding: 8px;'>{contract.Created}</td><td style='border: 1px solid black; padding: 8px;'>{contract.DueDate}</td></tr>";
            }

            emailContent += "</table>";

            // Enregistrement de l'e-mail dans MongoDB
            var email = new EmailRequestModel
            {
                Recipient = "maud.kirlin@ethereal.email",
                Subject = "Liste des contrats à échéance",
                Body = emailContent,
                ScheduledTime = DateTime.Now.AddMinutes(1),
                Status = "En attente d'envoi"
            };
            await _database.GetCollection<EmailRequestModel>("Jobs").InsertOneAsync(email);



            // Envoi de l'e-mail avec la liste des contrats à échéance
            await _mailService.SendEmailAsync("maud.kirlin@ethereal.email", "Liste des contrats à échéance", emailContent, context.CancellationToken);
        }
    }

}
