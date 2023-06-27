using ContractManagement.BusinessLogic.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ContractManagement.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContractController : ControllerBase
    {
        private readonly IContractService _contractService;
        public ContractController(IContractService contractService)
        {
            _contractService = contractService;
        }
        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var contract = await _contractService.GetContractsAsync(cancellationToken);
            return Ok(contract);
        }
       
        [HttpGet("dueIn/{days}")]
        public async Task<IActionResult> GetContractsDueIn(int days , CancellationToken cancellationToken)
        {
            try
            {
                var contracts = await _contractService.GetContractsByDueDateAsync(days, cancellationToken);
                return Ok(contracts);
            }
            catch (Exception ex)
            {
                // Gérer les erreurs appropriées ici
                return StatusCode(500, "Une erreur s'est produite lors de la récupération des contrats.");
            }
        }
    }
}
