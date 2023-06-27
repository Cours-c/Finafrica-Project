using ContractManagement.BusinessLogic.Services;
using ContractManagement.BusinessLogic.Services.Interfaces;
using ContractManagement.DataAccess.Data;
using ContractManagement.DataAccess.Repository;
using ContractManagement.DataAccess.Repository.Interfaces;
using MongoDB.Driver;
using Quartz;
using Quartz.Impl;
using Quartz.Simpl;
using Microsoft.Extensions.Options;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//DataBaseSettings
builder.Services.Configure<DataBaseSettings>(builder.Configuration.GetSection("CrontactManagement"));
var dbSettings = builder.Configuration.GetSection("CrontactManagement").Get<DataBaseSettings>();
builder.Services.AddSingleton<IMongoClient>(_ =>
{
    MongoClientSettings mongoSettings = MongoClientSettings.FromConnectionString(dbSettings.ConncectionString);
    return new MongoClient(mongoSettings);
});

builder.Services.AddSingleton<IMongoDatabase>(x =>
{
    var client = x.GetRequiredService<IMongoClient>();
    return client.GetDatabase(dbSettings.DataBaseName);
}
);
//CONFIGURATION POUR LE SERVEUR SMTP
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));

//CONFIGURATION DE QUARTZ
// Étape 1 : Ajouter les services Quartz.NET
builder.Services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
builder.Services.AddSingleton<IScheduler>(provider =>
{
    var schedulerFactory = provider.GetRequiredService<ISchedulerFactory>();
    var scheduler = schedulerFactory.GetScheduler().Result;

    // Configuration du job factory pour la résolution des dépendances
    // Dans la configuration du JobFactory
    scheduler.JobFactory = new MicrosoftDependencyInjectionJobFactory(provider, provider.GetRequiredService<IOptions<QuartzOptions>>());
    return scheduler;
});

//Etape 2 Enregistrer le service EnvoiContractsJobService en tant que service Scoped
builder.Services.AddScoped<EnvoiContractsJobService>();
//Etape 3 Configurer les travaux et les déclencheurs 
builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionScopedJobFactory();

    // Configuration du travail EnvoiContractsJobService
    var jobKey = new JobKey("EnvoiContractsJobService");
    q.AddJob<EnvoiContractsJobService>(jobOpts =>
        jobOpts.WithIdentity(jobKey)
            .StoreDurably());

    // Configuration du déclencheur
    q.AddTrigger(triggerOpts =>
    triggerOpts.ForJob(jobKey)
        .WithIdentity("EmailJobTrigger")
        .WithSimpleSchedule(scheduleOpts =>
            scheduleOpts.WithIntervalInMinutes(1)
                .RepeatForever()));  // Planifie l'exécution chaque 1 min
});

builder.Services.AddScoped<IContractRepository, ContractRepository>();
builder.Services.AddScoped<IContractService, ContractService>();
builder.Services.AddScoped<IMailService, MailKitService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//Etape 4 Démarrer le scheduler et planifier le travail
var scheduler = builder.Services.BuildServiceProvider().GetService<IScheduler>();
await scheduler.Start();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
