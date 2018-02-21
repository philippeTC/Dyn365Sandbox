using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;

using CRM.UnitTest.Fakes;

namespace CRM.UnitTest.PluginTests
{
    [TestClass]
    public class MyPluginFunctionalityTest
    {
        [TestMethod]
        public void CopyAddressToName()
        {
//            #region Prepare Test data and context
//
//            // Use the FakeOrganizationServiceFactory to fetch data from CRM
//            // IOrganizationService organizationService = new FakeOrganizationServiceFactory().CreateOrganizationService(null);
//            string entityName = "account";
//            var preimage = new Entity(entityName);
//
//            var originalAccountName = "Infront";
//            var originalAddress = "Veldkant 33 A";
//            preimage.Attributes.Add("name", originalAccountName);
//            preimage.Attributes.Add("address1_line1", originalAddress);
//
//            var target = new Entity(entityName);
//            string newAddress = "Veldkant 33 B";
//            target.Attributes.Add("address1_line1", newAddress);
//
//            FakePluginExecutionContext pluginExecutionContext = new FakePluginExecutionContext()
//            {
//                PrimaryEntityName = target.LogicalName,
//                MessageName = MessageName.Update,
//                Mode = MessageMode.Synchronous,
//                Stage = MessageProcessingStage.BeforeMainOperationInsideTransaction
//            };
//
//            pluginExecutionContext.InputParameters["Target"] = target;
//            pluginExecutionContext.PreEntityImages["preimage"] = preimage;
//
//            #endregion
//
//            #region Execute Plugin
//
//            FakeServiceProvider serviceProvider = new FakeServiceProvider(pluginExecutionContext);
//
//			var plugin = new MyPluginFunctionality();
//            plugin.Execute(serviceProvider);
//
//            #endregion
//
//            #region Assert
//
//            var expectedAccountName = originalAccountName + " " + newAddress;
//            var updatedAccountName = target.GetAttributeValue<string>("name");
//            Assert.AreEqual<string>(expectedAccountName, updatedAccountName);
//
//            #endregion

        }
    }
}
