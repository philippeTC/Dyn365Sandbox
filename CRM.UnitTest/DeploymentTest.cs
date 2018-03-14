using System;
using System.IO;
using System.ServiceModel;
using CRM.UnitTest.Fakes;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;

namespace CRM.UnitTest
{
    [TestClass]
    public class DeploymentTest
    {
        [TestMethod]
        public void AsynchronousDeployment()
        {
            IOrganizationService organizationService = new FakeOrganizationServiceFactory().CreateOrganizationService(null);

            string solutionName = "ThomasCook";
            string solutionFilePath = @"C:\Users\phili\Downloads\ThomasCook_6_2_9.zip";

            // export solution async

            try
            {
                // import solution async
                ExecuteAsyncRequest request = new ExecuteAsyncRequest
                {
                    Request = new ImportSolutionRequest
                    {
                        //set the compressed solutions file path to import
                        CustomizationFile = File.ReadAllBytes(solutionFilePath),
                        //set whether any processes (workflows) included in the solution should be activated after they are imported
                        PublishWorkflows = true,
                        //set whether any unmanaged customizations that have been applied over existing managed solution components should be overwritten
                        OverwriteUnmanagedCustomizations = true
                    }
                };

                ExecuteAsyncResponse response = (ExecuteAsyncResponse)organizationService.Execute(request);
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                throw new Exception("Error (OrganizationServiceFault) occured :" + ex.Detail != null ? ex.Detail.Message : ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Error occured :" + ex.Message);
            }
        }

        [TestMethod]
        public void PublishCustomizations()
        {
            IOrganizationService organizationService = new FakeOrganizationServiceFactory().CreateOrganizationService(null, new TimeSpan(0, 20, 0));

            try
            {
                var request = new PublishAllXmlRequest();
                var response  = (PublishAllXmlResponse)organizationService.Execute(request);
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                throw new Exception("Error (OrganizationServiceFault) occured :" + ex.Detail != null ? ex.Detail.Message : ex.Message);
            }
            catch (Exception ex)
            {
                 throw new Exception("Error occured :" + ex.Message);
            }
        }
    }
}
