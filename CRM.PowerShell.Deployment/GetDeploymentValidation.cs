using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using Microsoft.Xrm.Sdk;

namespace CRM.PowerShell.Deployment
{
    [Cmdlet(VerbsCommon.Get, "DeploymentValidation")]
    [OutputType(typeof(string))]
    public class GetDeploymentValidationCmdlet : Cmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public string CrmConnectionString
        {
            get { return crmConnectionString; }
            set { crmConnectionString = value; }
        }
        private string crmConnectionString;

        protected override void ProcessRecord()
        {
            //// Retrieve last deployed TC solution
            //IOrganizationService organizationService = new FakeOrganizationServiceFactory().CreateOrganizationService(null);
            //var fetchXml = string.Format("<fetch version='1.0' mapping='logical' distinct='true' no-lock='true'>" +
            //    "  <entity name='solution' >" +
            //    "    <attribute name='solutionid' />" +
            //    "    <attribute name='uniquename' />" +
            //    "    <attribute name='description' />" +
            //    "    <filter type='and' >" +
            //    "      <condition attribute='publisherid' operator='eq' value='{0}' />" +
            //    "      <condition attribute='uniquename' operator='like' value='tc_%' />" +
            //    "    </filter>" +
            //    "  </entity>" +
            //    "</fetch>", publisherId);
            //var thomascookSolutions = organizationService.RetrieveMultiple(new FetchExpression(fetchXml));

            //// loop solutions and find most recent version date
            //var solutionInfoList = new List<SolutionInfo>();
            //    foreach (var solution in thomascookSolutions.Entities)
            //    {
            //        solutionInfoList.Add(new SolutionInfo(solution));
            //    }

            //    return solutionInfoList.OrderByDescending(s => s.VersionDate).FirstOrDefault()?.VersionDate;

            WriteObject("1001: Workflows are disabled");
        }
    }
}
