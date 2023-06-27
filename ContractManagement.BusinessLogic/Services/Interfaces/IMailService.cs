using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractManagement.BusinessLogic.Services.Interfaces
{
    public interface IMailService
    {
        Task SendEmailAsync(string recipient, string subject, string body, CancellationToken cancellationToken);
    }
}
