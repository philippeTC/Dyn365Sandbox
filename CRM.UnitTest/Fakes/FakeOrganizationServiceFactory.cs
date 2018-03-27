using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;


//namespace CRM.UnitTest.Fakes
namespace CRM.UnitTest.Fakes
{
    public class FakeOrganizationServiceFactory : IOrganizationServiceFactory
    {
        public IOrganizationService CreateOrganizationService(Guid? userId)
        {
            CrmServiceClient connection = new CrmServiceClient(ConfigurationManager.ConnectionStrings["CrmConnection"].ConnectionString);
            return (IOrganizationService)connection.OrganizationWebProxyClient != null ? (IOrganizationService)connection.OrganizationWebProxyClient : (IOrganizationService)connection.OrganizationServiceProxy;
        }

        public IOrganizationService CreateOrganizationService(string connectionStringName, Guid? userId)
        {
            CrmServiceClient connection = new CrmServiceClient(ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString);
            return (IOrganizationService)connection.OrganizationWebProxyClient != null ? (IOrganizationService)connection.OrganizationWebProxyClient : (IOrganizationService)connection.OrganizationServiceProxy;
        }

        public IOrganizationService CreateOrganizationService(Guid? userId, TimeSpan timeout)
        {
            CrmServiceClient connection = new CrmServiceClient(ConfigurationManager.ConnectionStrings["CrmConnection"].ConnectionString);
            connection.OrganizationServiceProxy.Timeout = timeout;
            return (IOrganizationService)connection.OrganizationWebProxyClient != null ? (IOrganizationService)connection.OrganizationWebProxyClient : (IOrganizationService)connection.OrganizationServiceProxy;
        }
    }
}
