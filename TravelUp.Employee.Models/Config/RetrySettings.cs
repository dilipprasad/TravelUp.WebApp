using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelUp.EmployeeAPI.Models.Config
{
    public class RetrySettings
    {
        public int MaxRetries { get; set; }
        public int DelaySeconds { get; set; }
    }
}
