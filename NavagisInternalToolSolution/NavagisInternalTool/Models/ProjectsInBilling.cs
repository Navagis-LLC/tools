using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NavagisInternalTool.Models
{
    public class ProjectsInBilling
    {
        public string ProjectId { get; set; }
        public string Name { get; set; }
        public string BillingAccountName { get; set; }
        public bool BillingEnabled { get; set; }
    }
}