using ContractManagement.DataAccess.Models;
using ContractManagement.DataAccess.Repository.Interfaces;
using MongoDB.Driver;

namespace ContractManagement.DataAccess.Repository
{
    public class ContractRepository : IContractRepository
    {
        private readonly IMongoDatabase _database;
        public ContractRepository(IMongoDatabase database)
        {
            _database = database;
        }

        public async Task<IEnumerable<OrionContract>> GetContractsAsync(CancellationToken cancellationToken)
        {
            var contract = await _database.GetCollection<OrionContract>("Contract").AsQueryable().ToListAsync(cancellationToken);
            return contract;
        }
        
        public async Task<IEnumerable<OrionContract>> GetContractsByDueDateAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
        {
            var contracts = await _database.GetCollection<OrionContract>("Contract")
                .Find(c => c.DueDate >= startDate && c.DueDate <= endDate)
                .ToListAsync(cancellationToken);
            return contracts;
        }

    }
}
