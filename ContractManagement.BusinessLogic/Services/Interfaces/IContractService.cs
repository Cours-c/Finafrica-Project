using ContractManagement.BusinessLogic.DTO;

namespace ContractManagement.BusinessLogic.Services.Interfaces
{
    public interface IContractService
    {
        Task<IEnumerable<OrionContractDto>> GetContractsAsync(CancellationToken cancellationToken);
        //Task<IEnumerable<OrionContractDto>> GetContractsDueIn7DaysAsync(CancellationToken cancellationToken);
        Task<IEnumerable<OrionContractDto>> GetContractsByDueDateAsync(int days, CancellationToken cancellationToken);
    }
}
