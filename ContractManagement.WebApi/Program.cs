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

//CONFIGURATION DE LA BASE DE DONNEE MONGODB
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
//  Ajout des services Quartz.NET
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
void ConfigureServices(IServiceCollection service)
{
    service.AddTransient<IContractService, ContractService>();
    service.AddTransient<ISendPendingEmails, SendPendingEmails>();
    service.AddHostedService<EnvoiContractsJobService>();
}

builder.Services.AddScoped<IContractRepository, ContractRepository>();
builder.Services.AddScoped<IContractService, ContractService>();
builder.Services.AddScoped<IMailService, MailKitService>();
builder.Services.AddScoped<ISendPendingEmails, SendPendingEmails>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Injection du service de planification d'envoi d'email pour qu'il soit exécuter au démmerrage de l'application


var app = builder.Build();

//Démarrerage du scheduler et  planification du travail
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
