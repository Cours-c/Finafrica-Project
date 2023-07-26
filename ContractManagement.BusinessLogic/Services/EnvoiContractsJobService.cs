using Microsoft.Extensions.Hosting;
using Quartz;
using System;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using ContractManagement.BusinessLogic.Services.Interfaces;
using MongoDB.Bson;
using Quartz.Impl;

public class EnvoiContractsJobService : IJob, IHostedService
{
    private readonly IContractService _contraService;
    private readonly IMongoDatabase _database;
    private readonly ISendPendingEmails _sendPendingEmails;

    public EnvoiContractsJobService(IContractService contraService, IMongoDatabase database, ISendPendingEmails sendPendingEmails)
    {
        _contraService = contraService;
        _database = database;
        _sendPendingEmails = sendPendingEmails;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        // Logique pour l'exécution du travail planifié via Quartz.NET
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

        // Enregistrement Jobs dans MongoDB cette méthode doit être dans mon program.cs 
        var email = new BsonDocument
        {
            { "recipient_email", "maud.kirlin@ethereal.email" },
            { "subject", "Liste des contrats à échéance" },
            { "body", emailContent },
            { "scheduled_time", DateTime.UtcNow },
            { "status", "En attente d'envoi" }
        };
        await _database.GetCollection<BsonDocument>("Jobs").InsertOneAsync(email);
        await _sendPendingEmails.SendPendingEmail(context.CancellationToken);
    }

    public  async Task StartAsync(CancellationToken cancellationToken)
    {
        // Démarrer le planificateur Quartz.NET ici
        // Par exemple :
        var schedulerFactory = new StdSchedulerFactory();
        var scheduler = await schedulerFactory.GetScheduler();
        await scheduler.Start();

        // Planifier le travail EnvoiContractsJobService avec Quartz.NET
        var job = JobBuilder.Create<EnvoiContractsJobService>().Build();
        var trigger = TriggerBuilder.Create()
            .WithIdentity("EnvoiContractsTrigger", "default")
            .WithSimpleSchedule(x => x
                .WithIntervalInSeconds(60) // Par exemple, toutes les 60 secondes
                .RepeatForever())
            .Build();

        await scheduler.ScheduleJob(job, trigger);

        await Task.CompletedTask;

    }
    public  async Task StopAsync(CancellationToken cancellationToken)
    {
        var schedulerFactory = new StdSchedulerFactory();
        var scheduler = await schedulerFactory.GetScheduler();
        await scheduler.Shutdown();
    }
}
