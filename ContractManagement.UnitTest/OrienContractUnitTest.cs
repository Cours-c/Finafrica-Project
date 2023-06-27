using ContractManagement.BusinessLogic.Services;
using ContractManagement.DataAccess.Models;
using ContractManagement.DataAccess.Repository.Interfaces;
using Moq;
using System.Diagnostics.Contracts;

namespace ContractManagement.UnitTest
{
    [TestClass]
    public  class OrienContractUnitTest
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            var contractRepositoryMock  = new Mock<IContractRepository>();
            var cancellationToken = new CancellationToken();
            var contracts = new List<OrionContract>
        {
            new OrionContract { Id = "1", DueDate = DateTime.UtcNow.AddDays(7) },
            new OrionContract { Id = "2", DueDate = DateTime.UtcNow.AddDays(7) },
            new OrionContract { Id = "3", DueDate = DateTime.UtcNow.AddDays(30) }
        };

            contractRepositoryMock.Setup(r => r.GetContractsByDueDateAsync(
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                cancellationToken)).ReturnsAsync(contracts);

            var contractService = new ContractService(contractRepositoryMock.Object);

            var result = await contractService.GetContractsByDueDateAsync(7, cancellationToken);
            Assert.IsNotNull(result);
            Assert.AreEqual(3 , result.Count());
               
        }
    }
}
