using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;

//namespace CRM.UnitTest.Fakes
namespace CRM.UnitTest.Fakes
{
    public class FakeServiceProvider : IServiceProvider
    {
        private FakePluginExecutionContext _pluginExecutionContext;
        private FakeWorkflowContext _workflowContext;

        public FakeServiceProvider(FakePluginExecutionContext pluginExecutionContext)
        {
            _pluginExecutionContext = pluginExecutionContext;
        }

        public FakeServiceProvider(FakeWorkflowContext workflowContext)
        {
            _workflowContext = workflowContext;
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IPluginExecutionContext))
            {
                return _pluginExecutionContext;
            }
            if (serviceType == typeof(IWorkflowContext))
            {
                return _workflowContext;
            }
            if (serviceType == typeof(IOrganizationServiceFactory))
            {
                return new FakeOrganizationServiceFactory();
            }
            if (serviceType == typeof(ITracingService))
            {
                return new FakeTracingService();
            }
            return null;
        }
    }
}
