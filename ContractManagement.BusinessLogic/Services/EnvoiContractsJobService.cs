using ContractManagement.BusinessLogic.Services.Interfaces;
using Quartz;

namespace ContractManagement.BusinessLogic.Services
{
    public  class EnvoiContractsJobService :IJob
    {
        private readonly IContractService _contraService;
        private readonly IMailService _mailService;
        public EnvoiContractsJobService(IContractService contraService, IMailService mailService)
        {
            _contraService = contraService;
            _mailService = mailService;
        }

        public async Task Execute(IJobExecutionContext context )
        {
            // Récupération de la liste des contrats à échéance sur 7 jours
            var contracts = await _contraService.GetContractsByDueDateAsync(7, context.CancellationToken);

            // Création du contenu de l'e-mail avec un tableau HTML
            var emailContent = "<h2>Liste des contrats à échéance :</h2>";
            emailContent += "<table style='border-collapse: collapse;'>";
            emailContent += "<tr><th style='border: 1px solid black; padding: 8px;'>ID Contrat</th> <th style='border: 1px solid black; padding: 8px;'>Numéro de police</th> <th style='border: 1px solid black; padding: 8px;'>Date de créaition</th><th style='border: 1px solid black; padding: 8px;'>Echéance</th></tr>";
            foreach (var contract in contracts)
            {
                emailContent += $"<tr><td style='border: 1px solid black; padding: 8px;'>{contract.Id}</td><td style='border: 1px solid black; padding: 8px;'>{contract.PolicyNumber}</td><td style='border: 1px solid black; padding: 8px;'>{contract.Created}</td><td style='border: 1px solid black; padding: 8px;'>{contract.DueDate}</td></tr>";
            }
            emailContent += "</table>";

            // Envoi de l'e-mail avec la liste des contrats à échéance
            await _mailService.SendEmailAsync("loulauriatzahouly@gmail.com", "Liste des contrats à échéance", emailContent, context.CancellationToken);
        }



    }
}
