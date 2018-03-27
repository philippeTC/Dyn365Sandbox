using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;

namespace CRM.PowerShell.Deployment
{
    [Cmdlet(VerbsCommon.Get, "LatestVersion")]
    public class GetLatestVersionCmdlet : Cmdlet
    {

    }

    [Cmdlet(VerbsCommon.Reset, "ComponentSolutions")]
    public class ResetComponentSolutionsCmdlet : Cmdlet
    {

    }

    [Cmdlet(VerbsCommon.Set, "SolutionVersion")]
    public class SetSolutionVersionCmdlet : Cmdlet
    {

    }
}
