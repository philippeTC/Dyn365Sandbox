using System;
using System.Collections.Generic;
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
    public class BusinessRuleTest
    {
        private IOrganizationService sourceOrganizationService = null;
        private IOrganizationService targetOrganizationService = null;
        //private string entityName = "tc_compensation";
        private string entityName = string.Empty;

        [TestMethod]
        public void TestGetDeployedOrder()
        {
            sourceOrganizationService = new FakeOrganizationServiceFactory().CreateOrganizationService(null);
            targetOrganizationService = new FakeOrganizationServiceFactory().CreateOrganizationService("TargetCrmConnection", null);

            // get source business rules (in correct order)
            var sourceBRdictionary = this.getBusinessRuleActivationOrder(entityName, sourceOrganizationService);

            if (sourceBRdictionary.Count > 0)
            {
                // reactivate target business rules in correct order
                this.SetStateBusinessRules(entityName, sourceBRdictionary, targetOrganizationService, false);
                this.SetStateBusinessRules(entityName, sourceBRdictionary, targetOrganizationService, true);
            }
        }

        private Dictionary<string, List<Guid>> getBusinessRuleActivationOrder(string entityName, IOrganizationService organizationService) {
            var entityTypeCondition = string.Empty;
            if (!string.IsNullOrEmpty(entityName))
            {
                var entityTypeCode = this.GetEntityTypeCode(entityName, organizationService);
                entityTypeCondition = string.Format("<condition attribute='primaryentity' operator='eq' value='{0}' />", entityTypeCode);
            }

            var fetchXml = string.Format("<fetch>" +
                    "  <entity name='workflow' >" +
                    "    <attribute name='primaryentity' />" +
                    "    <attribute name='modifiedon' />" +
                    "    <attribute name='name' />" +
                    "    <attribute name='workflowid' />" +
                    "    <filter>" +
                    "      {0}" + 
                    "      <condition attribute='category' operator='eq' value='2' />" +
                    "      <condition attribute='statuscode' operator='eq' value='2' />" +
                    "      <condition attribute='parentworkflowid' operator='null' />" +
                    "      <condition attribute='createdby' operator='neq' value='cd4efb79-f0ac-487c-92b3-38ec436baf85' />" +
                    "    </filter>" +
                    "    <order attribute='primaryentity' />" +
                    "    <order attribute='modifiedon' />" +
                    "  </entity>" +
                    "</fetch>", entityTypeCondition);

            var businessRules = organizationService.RetrieveMultiple(new FetchExpression(fetchXml));

            var businessruleDictionary = new Dictionary<string, List<Guid>>();
            string entityname = string.Empty;
            var businessRuleList = new List<Guid>();
            foreach(var br in businessRules?.Entities)
            {
                entityname = br.GetAttributeValue<string>("primaryentity");

                if (businessruleDictionary.ContainsKey(entityname)) {
                    businessruleDictionary[entityname].Add(br.Id);
                }
                else
                {
                    businessruleDictionary.Add(entityname, new List<Guid>() { br.Id });
                }
            }

            return businessruleDictionary;
        }

        //private void activateBusinessRuleInOrder(string entityName, List<Guid> sourceOrder, List<Guid> targ)
        private void SetStateBusinessRules(string entityName, Dictionary<string, List<Guid>> businessRuleDictionary, IOrganizationService organizationService, bool active) 
        {
            if (!active)
            {
                if (string.IsNullOrEmpty(entityName))
                {
                    foreach (var key in businessRuleDictionary?.Keys)
                    {
                        this.DeactivateEntityBusinessRules(businessRuleDictionary[key], organizationService);
                    }
                }
                else if (businessRuleDictionary.ContainsKey(entityName))
                {
                    this.DeactivateEntityBusinessRules(businessRuleDictionary[entityName], organizationService);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(entityName))
                {
                    foreach (var key in businessRuleDictionary?.Keys)
                    {
                        this.ActivateEntityBusinessRules(businessRuleDictionary[key], organizationService);
                    }
                }
                else if (businessRuleDictionary.ContainsKey(entityName))
                {
                    this.ActivateEntityBusinessRules(businessRuleDictionary[entityName], organizationService);
                }

            }
        }

        private void DeactivateEntityBusinessRules(List<Guid> businessRuleList, IOrganizationService organizationService)
        {
            var multipleReq = new ExecuteMultipleRequest()
            {
                // Assign settings that define execution behavior: continue on error, return responses. 
                Settings = new ExecuteMultipleSettings()
                {
                    ContinueOnError = true,
                    ReturnResponses = true
                },
                // Create an empty organization request collection.
                Requests = new OrganizationRequestCollection()
            };

            // Add business rules to reactivate 
            var updateRequestList = new List<SetStateRequest>();
            foreach (var br in businessRuleList)
            {
                // create deactivate request
                //updateRequestList.Add(new UpdateRequest {
                //    Target = new Entity("workflow")
                //    {
                //        Attributes = new AttributeCollection()
                //        {
                //            new KeyValuePair<string, object>("workflowid", br),
                //            new KeyValuePair<string, object>("statecode", new OptionSetValue(active == true ? 1 : 0)),
                //            new KeyValuePair<string, object>("statuscode", new OptionSetValue(active == true ? 2 : 1))
                //        }
                //    }
                //});
                updateRequestList.Add(new SetStateRequest
                {
                    EntityMoniker = new EntityReference("workflow", br),
                    State = new OptionSetValue(0),
                    Status = new OptionSetValue(1)
                });
            }

            multipleReq.Requests.AddRange(updateRequestList);

            // Execute all the requests in the request collection using a single web method call.
            var responseWithResults = (ExecuteMultipleResponse)organizationService.Execute(multipleReq);

            // Evaluate the results returned in the responses.
            var successList = new List<Guid>();
            var failedList = new List<Guid>();
            foreach (var responseItem in responseWithResults.Responses)
            {
                // Success
                if (responseItem.Response != null)
                {
                    //DisplayResponse(multipleReq.Requests[responseItem.RequestIndex], responseItem.Response);
                    successList.Add((((SetStateRequest)multipleReq.Requests[responseItem.RequestIndex]).EntityMoniker).Id);
                }

                // Failed
                else if (responseItem.Fault != null)
                {
                    DisplayFault(multipleReq.Requests[responseItem.RequestIndex], responseItem.RequestIndex, responseItem.Fault);
                    failedList.Add((((SetStateRequest)multipleReq.Requests[responseItem.RequestIndex]).EntityMoniker).Id);
                }
            }
        }

        /// <summary>
        /// Activates business rules in a synchronous way. 
        /// </summary>
        /// <param name="businessRuleList"></param>
        /// <param name="organizationService"></param>
        private void ActivateEntityBusinessRules(List<Guid> businessRuleList, IOrganizationService organizationService)
        {
            var failedList = new List<Guid>();
            SetStateRequest setstateReq = null;
            foreach (var br in businessRuleList)
            {
                try
                {
                    setstateReq = new SetStateRequest
                    {
                        EntityMoniker = new EntityReference("workflow", br),
                        State = new OptionSetValue(1),
                        Status = new OptionSetValue(2)
                    };
                    organizationService.Execute(setstateReq);
                    System.Threading.Thread.Sleep(1000);    // needed because modified time does not have miliseconds and needs to differ between each business rule.
                }
                catch (FaultException<OrganizationServiceFault> ex)
                {
                    Debug.WriteLine("Error(OrganizationServiceFault) occured: " + ex.Detail != null ? ex.Detail.Message : ex.Message);
                    failedList.Add(br);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error occured: " + ex.ToString());
                    failedList.Add(br);
                }
            }
        }

        private void DisplayResponse(OrganizationRequest organizationRequest, OrganizationResponse organizationResponse)
        {
            Debug.WriteLine("Updated " + ((Entity)organizationRequest.Parameters["Target"]).LogicalName + " with id as " + organizationResponse.Results["id"].ToString());
        }

        private void DisplayFault(OrganizationRequest organizationRequest, int count, OrganizationServiceFault organizationServiceFault)
        {
            Debug.WriteLine("A fault occurred when processing {1} request, at index {0} in the request collection with a fault message: {2}", count + 1, organizationRequest.RequestName, organizationServiceFault.Message);
        }

        private int GetEntityTypeCode(string entityName, IOrganizationService organizationService)
        {
            var entityRequest = new RetrieveEntityRequest();
            entityRequest.RetrieveAsIfPublished = true;
            entityRequest.LogicalName = entityName;
            entityRequest.EntityFilters = Microsoft.Xrm.Sdk.Metadata.EntityFilters.Entity;

            var response = (RetrieveEntityResponse)organizationService.Execute(entityRequest);

            var objecttypecode = response?.EntityMetadata?.ObjectTypeCode;

            if (!objecttypecode.HasValue)
            {
                throw new Exception(string.Format("Could not retrieve metadata for entity '{0}'", entityName));
            }

            return objecttypecode.Value;
        }
    }
}
