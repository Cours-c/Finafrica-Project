using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractManagement.BusinessLogic.ClasseQuartz
{
    public class JobSchedule
    {
        public Type _JobType { get; }
        public string _CronExpression { get; }
        public JobSchedule(Type jobType, string cronExpression)
        {
            _JobType = jobType;
            _CronExpression = cronExpression;
        }
    }

}
