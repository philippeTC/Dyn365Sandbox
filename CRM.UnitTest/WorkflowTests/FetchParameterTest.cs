using System;
using System.Activities;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Xrm.Sdk.Workflow.Activities;

using CRM.UnitTest.Fakes;

namespace CRM.UnitTest.WorkflowTests
{
    [TestClass]
    public class FetchParameterTest
    {
        [TestMethod]
        public void GetParameterValue()
        {
 //           #region Prepare Test data and context
 //
 //           FakeWorkflowContext workflowContext = new FakeWorkflowContext();
 //           //workflowContext.PrimaryEntityName = "inf_parameter";
 //           //workflowContext.PrimaryEntityId = new Guid("290BAF71-E408-E411-B206-005056BC2F49");
 //           var inputs = new Dictionary<string, object>
 //           {
 //               {"Key", "MyStringValue"},
 //           };
 //
 //
 //           #endregion
 //
 //           #region Execute Workflow
 //
 //           var workflow = new FetchParameter();
 //
 //           var invoker = new WorkflowInvoker(workflow);
 //           invoker.Extensions.Add<ITracingService>(() => new FakeTracingService());
 //           invoker.Extensions.Add<IWorkflowContext>(() => workflowContext);
 //           invoker.Extensions.Add<IOrganizationServiceFactory>(() => new FakeOrganizationServiceFactory());
 //
 //           // Correctly pass in InArgument<T> properties
 //           // the InArgument<T> properties need to be passed in as a dictionary when call WorkflowInvoker.Invoke method
 //           var output = invoker.Invoke(inputs);
 //
 //           #endregion
 //
 //           #region Assert
 //
 //           var expectedParameterValue = "Parameter Value which contains a string";
 //           string actualParametervalue = output.ContainsKey("StringValue") ? output["StringValue"] as string : string.Empty;
 //           Assert.AreEqual<string>(expectedParameterValue, actualParametervalue.ToString());
 //
 //           #endregion

        }


    }
}
