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

        [HttpPost("schedule")]
        public async Task <IActionResult> SheduleEmails()
        {
            try
            {
                var contracts = await _contractService.GetContractsByDueDateAsync(7, CancellationToken.None);
                // planification du travail EnvoieEmailJob avec les contrats récupérer
                var jobDataMap = new JobDataMap();
                jobDataMap["Contracts"] = contracts;
                var jobKey = new JobKey("EnvoiContractsJobService");
                var triggerKey = new TriggerKey("EmailJobTrigger");

                var jobDetail = JobBuilder.Create<EnvoiContractsJobService>()
                    .WithIdentity(jobKey)
                    .UsingJobData(jobDataMap)
                    .Build();
                var trigger = TriggerBuilder.Create()
                    .WithIdentity(triggerKey)
                    .StartNow()
                    .Build();
                _scheduler.ScheduleJob(jobDetail, trigger);
                return Ok("Planification email ok");
            } 
            catch (Exception ex)
            {
                return StatusCode(500, $"Une erreur s'est produite lors de la planification des e-mails : {ex.Message}");
            }
        }

    }
}
