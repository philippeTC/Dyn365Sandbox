using CRM.UnitTest.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CRM.UnitTest
{
    [TestClass]
    public class TransactionTest
    {
        [TestMethod]
        public void TestCompoundCreate()
        {
            IOrganizationService organizationService = new FakeOrganizationServiceFactory().CreateOrganizationService(null);
            try
            {
                //Account
                Entity accountToBeCreated = new Entity("account");
                accountToBeCreated["name"] = "Compound Account 1";

                //Define Tasks
                var relatedTasksToBeCreated = new EntityCollection();
                for (int i = 1; i <= 3; i++)
                {
                    var task = new Entity("task");
                    task["subject"] = string.Format("Test Account Task {0}", i.ToString());

                    ////put a mistake here
                    //if (i == 3)
                    //{
                    //    task["scheduledstart"] = "today";
                    //    //it should accepts datetime only, since it is a datetime field
                    //}

                    relatedTasksToBeCreated.Entities.Add(task);
                }

                //Creates the reference between which relationship between task and
                //Account we would like to use.
                Relationship taskRelationship = new Relationship("Account_Tasks");

                //Adds the tasks to the account under the specified relationship
                accountToBeCreated.RelatedEntities.Add(taskRelationship, relatedTasksToBeCreated);

                //Passes the Account (which contains the tasks)
                var accountId = organizationService.Create(accountToBeCreated);
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
        public void TestCompoundUpdate()
        {
            IOrganizationService organizationService = new FakeOrganizationServiceFactory().CreateOrganizationService(null);

            //Account
            Entity accountToBeUpdated = new Entity("account");
            accountToBeUpdated["accountid"] = new Guid("38B9B9CD-1101-E811-8116-5065F38B74A1");
            //accountToBeUpdated["name"] = "Compound Account 1";

            //Define Tasks
            var relatedTasksToBeCreated = new EntityCollection();
            for (int i = 1; i <= 3; i++)
            {
                var task = new Entity("task");
                task.Id = Guid.NewGuid();
                task["subject"] = string.Format("Test Account Task {0}", i.ToString());
                task["regardingobjectid"] = new EntityReference("account", accountToBeUpdated.Id);

                ////put a mistake here
                //if (i == 3)
                //{
                //    task["scheduledstart"] = "today";
                //    //it should accepts datetime only, since it is a datetime field
                //}

                relatedTasksToBeCreated.Entities.Add(task);
            }

            //Creates the reference between which relationship between task and
            //Account we would like to use.
            Relationship taskRelationship = new Relationship("Account_Tasks");

            //Adds the tasks to the account under the specified relationship
            accountToBeUpdated.RelatedEntities.Add(taskRelationship, relatedTasksToBeCreated);

            //Passes the Account (which contains the tasks)
            organizationService.Update(accountToBeUpdated);
        }

    }
}
