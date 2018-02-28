using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace CRM.PowerShell.Deployment
{
    [Cmdlet(VerbsCommon.Set, "BusinessRuleOrder")]
    [OutputType(typeof(string))]
    public class SetBusinessRuleOrderCmdlet : Cmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public string CrmSourceConnectionString
        {
            get { return crmSourceConnectionString; }
            set { crmSourceConnectionString = value; }
        }
        private string crmSourceConnectionString;

        [Parameter(Position = 0, Mandatory = true)]
        public string CrmTargetConnectionString
        {
            get { return crmTargetConnectionString; }
            set { crmTargetConnectionString = value; }
        }
        private string crmTargetConnectionString;

        protected override void ProcessRecord()
        {
            


            WriteObject("Updated version info");
        }
    }
}
