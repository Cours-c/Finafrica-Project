using ContractManagement.BusinessLogic.Services;
using ContractManagement.BusinessLogic.Services.Interfaces;
using ContractManagement.DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Quartz;

namespace ContractManagement.WebApi.Controllers
{
    public class EmailController : ControllerBase
    {
        private readonly IMailService _emailService;
        private readonly IContractService _contractService;
        private readonly IScheduler _scheduler;

        public EmailController( IContractService contractService , IScheduler scheduler , IMailService emailService)
        {
            _emailService = emailService;
            _contractService = contractService;
            _scheduler = scheduler;

        }
        [HttpPost("Email")]
        public async Task<IActionResult> SendEmail([FromBody] EmailRequestModel request, CancellationToken cancellationToken)
        {
            await _emailService.SendEmailAsync(request.Recipient, request.Subject, request.Body, cancellationToken);
            return Ok("okkkkkkkkk");
        }

        [HttpPost("schedule-emails")]
        public async Task<IActionResult> ScheduleEmails()
        {
            try
            {
                var jobKey = new JobKey("EnvoiContractsJobService");
                var triggerKey = new TriggerKey("EmailJobTrigger");

                // Vérifier si le travail existe déjà
                if (await _scheduler.CheckExists(jobKey))
                {
                    return BadRequest("Le travail d'envoi d'e-mails est déjà planifié.");
                }

                // Créer le travail et le déclencheur
                var jobDetail = JobBuilder.Create<EnvoiContractsJobService>()
                    .WithIdentity(jobKey)
                    .Build();

                var trigger = TriggerBuilder.Create()
                    .WithIdentity(triggerKey)
                    .StartNow()
                    .WithSimpleSchedule(x => x.WithIntervalInMinutes(1).RepeatForever())
                    .Build();

                // Planifier le travail
                await _scheduler.ScheduleJob(jobDetail, trigger);

                return Ok("L'envoi d'e-mails a été planifié avec succès.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Une erreur s'est produite lors de la planification de l'envoi d'e-mails : {ex.Message}");
            }
        }
    }


}

