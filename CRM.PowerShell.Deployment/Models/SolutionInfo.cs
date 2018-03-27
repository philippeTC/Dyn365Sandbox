using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.PowerShell.Deployment.Models
{
    public class SolutionInfo
    {
        public Guid Id { get; set; }
        public string Version { get; set; }
        public DateTime? VersionDate { get; set; }
        public DateTime? DeploymentDate { get; set; }

        public SolutionInfo(Entity solution)
        {
            this.Id = solution.Id;
            this.Version = solution.GetAttributeValue<string>("version");

            JObject jObject = JObject.Parse(solution.GetAttributeValue<string>("description"));
            JToken versionDate = null;
            if (jObject.TryGetValue("versionDate", out versionDate))
            {
                VersionDate = DateTime.Parse(versionDate.ToString());
            }
            JToken deploymentDate = null;
            if (jObject.TryGetValue("deploymentDate", out deploymentDate))
            {
                DeploymentDate = DateTime.Parse(deploymentDate.ToString());
            }
        }
    }
}
