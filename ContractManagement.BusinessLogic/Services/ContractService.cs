using ContractManagement.BusinessLogic.DTO;
using ContractManagement.BusinessLogic.Services.Interfaces;
using ContractManagement.DataAccess.Models;
using ContractManagement.DataAccess.Repository.Interfaces;


namespace ContractManagement.BusinessLogic.Services
{
         public  class ContractService : IContractService {
         private readonly IContractRepository _contractRepository;

       
        public ContractService(IContractRepository contractRepository)
        {
            _contractRepository = contractRepository;
        }

        public async Task<IEnumerable<OrionContractDto>> GetContractsAsync(CancellationToken cancellationToken)
        {
            IEnumerable<OrionContract> contractRepository = await _contractRepository.GetContractsAsync(cancellationToken);

            var contracts = contractRepository.Select(c => new OrionContractDto
            {
                Id = c.Id,
                Created = c.Created,
                Updated = c.Updated,
                PolicyNumber = c.PolicyNumber,
                ClientId = c.ClientId,
                QuotationId = c.QuotationId,
                ActiveQuotationId = c.ActiveQuotationId,
                ContractDate = c.ContractDate,
                DueDate = c.DueDate,
                IntermediaryId = c.IntermediaryId,
            });

            return contracts;
        }


        public async Task<IEnumerable<OrionContractDto>> GetContractsByDueDateAsync(int days, CancellationToken cancellationToken)
        {
            var currentDate = DateTime.UtcNow;
            var endDate = currentDate.AddDays(days);

            var contracts = await _contractRepository.GetContractsByDueDateAsync(currentDate, endDate, cancellationToken);

            var contractDtos = contracts.Select(c => new OrionContractDto
            {
                Id = c.Id,
                Created = c.Created,
                Updated = c.Updated,
                PolicyNumber = c.PolicyNumber,
                ClientId = c.ClientId,
                QuotationId = c.QuotationId,
                ActiveQuotationId = c.ActiveQuotationId,
                ContractDate = c.ContractDate,
                DueDate = c.DueDate,
                IntermediaryId = c.IntermediaryId,
            });;

            return contractDtos;
        }

    }


}
