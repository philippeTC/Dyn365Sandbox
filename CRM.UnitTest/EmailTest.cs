using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using CRM.UnitTest.Fakes;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata.Query;
using Microsoft.Xrm.Sdk.Query;

namespace CRM.UnitTest
{
    [TestClass]
    public class EmailTest
    {
        private IOrganizationService organizationService = null;


        [TestMethod]
        public void TestRemoveParties()
        {
            organizationService = new FakeOrganizationServiceFactory().CreateOrganizationService(null);

            var email = organizationService.Retrieve("email", new Guid("578327CF-1697-E811-813A-5065F38B5651"), new ColumnSet("statecode", "statuscode","to", "cc", "bcc", "activityid"));

            var updateEmail = new Entity("email");
            updateEmail.Id = email.Id;
            updateEmail.Attributes.Add("statecode", new OptionSetValue(0));
            updateEmail.Attributes.Add("statuscode", new OptionSetValue(1));
            //organizationService.Update(updateEmail);

            updateEmail = new Entity("email");
            updateEmail.Id = email.Id;
            updateEmail.Attributes.Add("to", null);
            updateEmail.Attributes.Add("statecode", new OptionSetValue(1));
            updateEmail.Attributes.Add("statuscode", new OptionSetValue(4));
            organizationService.Update(updateEmail);
            
            //var fetchXml = string.Format("<fetch version='1.0' mapping='logical' distinct='true' no-lock='true'>" +
            //        "  <entity name='activityparty' >" +
            //        "    <all-attributes/>" +
            //        "    <filter>" +
            //        "      <condition attribute='activityid' operator='eq' value='{0}' />" +
            //        "      <condition attribute='participationtypemask' operator='eq' value='2' />" +
            //        "    </filter>" +
            //        "  </entity>" +
            //        "</fetch>", email.Id);
            //var parties = organizationService.RetrieveMultiple(new FetchExpression(fetchXml));

            //// delete party
            //foreach (var party in parties?.Entities)
            //{
            //    organizationService.Delete("activityparty",party.Id);
            //}
        }

    }
}
