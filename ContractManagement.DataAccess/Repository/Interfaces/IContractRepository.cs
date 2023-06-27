using ContractManagement.DataAccess.Models;


namespace ContractManagement.DataAccess.Repository.Interfaces
{
    public interface IContractRepository
    {
        Task<IEnumerable<OrionContract>> GetContractsAsync(CancellationToken cancellationToken);
        Task<IEnumerable<OrionContract>> GetContractsByDueDateAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken);

    }
}