using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using CRM.UnitTest.Fakes;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using System.ServiceModel;
using Microsoft.Crm.Sdk.Messages;
using CRM.UnitTest.Models;
using System.Linq;
using Microsoft.Xrm.Sdk.Messages;

namespace CRM.UnitTest
{
    [TestClass]
    public class SolutionComponentTest
    {
        private Guid publisherId = new Guid("E8E7A6A2-09D8-E611-80F9-3863BB354FF0");
        private Guid thomascookSolutionId = new Guid("4AEED0AB-09D8-E611-80F9-3863BB354FF0");

        [TestMethod]
        public void TestGetLatestVersion()
        {
            var latestversion = this.GetLatestVersion();
        }

        [TestMethod]
        public void TestIncrementSolutionVersion()
        {
            IOrganizationService organizationService = new FakeOrganizationServiceFactory().CreateOrganizationService(null);

            var latestversion = this.IncrementSolutionVersion("tc_optionsets", VersionPart.Build, organizationService);
        }

        [TestMethod]
        public void TestUpdateComponentSolutions()
        {
            try
            {
                IOrganizationService organizationService = new FakeOrganizationServiceFactory().CreateOrganizationService(null);

                // get most recent version date from 'TC_' component solutions
                var latestVersionDate = this.GetLatestVersion();

                // get components since last release from the thomascook solution
                var thomascookComponents = this.GetModifiedSolutionComponents(latestVersionDate, organizationService);

                // ### Update Webresources ###
                var relatedTypes = new List<int>() {
                    (int)ComponentType.WebResource
                };
                this.UpdateComponentSolution("tc_webresources", thomascookComponents, relatedTypes, organizationService);
                
                // ### Update OptionSets ###
                relatedTypes = new List<int>() {
                    (int)ComponentType.OptionSet
                };
                this.UpdateComponentSolution("tc_optionsets", thomascookComponents, relatedTypes, organizationService);

                // ### Update Entities ###
                relatedTypes = new List<int>() {
                    (int)ComponentType.Entity,
                    (int)ComponentType.Attribute,
                    (int)ComponentType.Relationship,
                    (int)ComponentType.AttributePicklistValue,
                    (int)ComponentType.AttributeLookupValue,
                    (int)ComponentType.ViewAttribute,
                    (int)ComponentType.LocalizedLabel,
                    (int)ComponentType.RelationshipExtraCondition,
                    (int)ComponentType.EntityRelationship,
                    (int)ComponentType.DisplayString,
                    (int)ComponentType.DisplayStringMap,
                    (int)ComponentType.Form,
                    (int)ComponentType.SavedQuery,
                    (int)ComponentType.EntityMap,
                    (int)ComponentType.AttributeMap,
                    (int)ComponentType.SystemForm,
                    (int)ComponentType.FieldSecurityProfile,
                    (int)ComponentType.FieldPermission
                };
                this.UpdateComponentSolution("tc_entities_fls", thomascookComponents, relatedTypes, organizationService);

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
        public void TestUpdateVersionInfo()
        {
            // update the versioninfo in the description field of the component solutions

        }

        [TestMethod]
        public void TestDifferentialSolution()
        {
            var thomascookSolutionId = "4AEED0AB-09D8-E611-80F9-3863BB354FF0";
            var lastDeploymentDate = "2018-01-1";
            var publisherId = new Guid("E8E7A6A2-09D8-E611-80F9-3863BB354FF0");

            // Retrieve last modified components in the Thomas Cook solution
            IOrganizationService organizationService = new FakeOrganizationServiceFactory().CreateOrganizationService(null);
            var fetchXml = string.Format("<fetch version='1.0' mapping='logical' distinct='true' no-lock='true'>" +
                    "  <entity name='solutioncomponent' >" +
                    "    <all-attributes/>" +
                    "    <filter>" +
                    "      <condition attribute='modifiedon' operator='on-or-after' value='{0}' />" +
                    "      <condition attribute='solutionid' operator='eq' value='{1}' />" +
                    "    </filter>" +
                    "  </entity>" +
                    "</fetch>", lastDeploymentDate, thomascookSolutionId);
            var thomascookComponents = organizationService.RetrieveMultiple(new FetchExpression(fetchXml));

            // Create Solution
            var solution = new Entity("solution");
            solution.Attributes.Add("friendlyname", "TC Differential");
            solution.Attributes.Add("uniquename", "TCDifferential"); // no spaces, 
            solution.Attributes.Add("publisherid", new EntityReference("publisher", publisherId));   // Thomas Cook
            solution.Attributes.Add("version", "1.0.0");

            // Add solution components
            EntityCollection solutionComponents = new EntityCollection();
            Entity solutioncomponent = null;
            foreach (var sc in thomascookComponents.Entities)
            {
                solutioncomponent = new Entity("solutioncomponent");
                solutioncomponent["componenttype"] = sc["componenttype"];
                solutioncomponent["ismetadata"] = sc["ismetadata"];
                solutioncomponent["rootsolutioncomponentid"] = sc.GetAttributeValue<Guid?>("rootsolutioncomponentid");
                solutioncomponent["rootcomponentbehavior"] = sc.GetAttributeValue<OptionSetValue>("rootcomponentbehavior");
                solutioncomponent["objectid"] = sc["objectid"];
                solutionComponents.Entities.Add(solutioncomponent);
            }

            //Creates the reference between Solution and Component
            Relationship solutionRelationship = new Relationship("solution_solutioncomponent");

            //Adds the components to the solution under the specified relationship
            solution.RelatedEntities.Add(solutionRelationship, solutionComponents);

            try
            {
                // create all
                organizationService.Create(solution);
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
        public void TestExportSolution()
        {
            var solutionId = new Guid("207C362F-A1FA-E711-8115-5065F38BB571");

            IOrganizationService organizationService = new FakeOrganizationServiceFactory().CreateOrganizationService(null);

            ExportSolutionRequest exportRequest = new ExportSolutionRequest();
            exportRequest.SolutionName = "TCDifferential";
            exportRequest.Managed = false;
            exportRequest.TargetVersion = "8.2";

            try
            {
                var exportResult = organizationService.Execute(exportRequest);
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

        public DateTime? GetLatestVersion()
        {
            // Retrieve last deployed TC solution
            IOrganizationService organizationService = new FakeOrganizationServiceFactory().CreateOrganizationService(null);
            var fetchXml = string.Format("<fetch version='1.0' mapping='logical' distinct='true' no-lock='true'>" +
                "  <entity name='solution' >" +
                "    <attribute name='solutionid' />" +
                "    <attribute name='uniquename' />" +
                "    <attribute name='description' />" +
                "    <filter type='and' >" +
                "      <condition attribute='publisherid' operator='eq' value='{0}' />" +
                "      <condition attribute='uniquename' operator='like' value='tc_%' />" +
                "    </filter>" +
                "  </entity>" +
                "</fetch>", publisherId);
            var thomascookSolutions = organizationService.RetrieveMultiple(new FetchExpression(fetchXml));

            // loop solutions and find most recent version date
            var solutionInfoList = new List<SolutionInfo>();
            foreach (var solution in thomascookSolutions.Entities)
            {
                solutionInfoList.Add(new SolutionInfo(solution));
            }

            return solutionInfoList.OrderByDescending(s => s.VersionDate).FirstOrDefault()?.VersionDate;
        }

        public List<Entity> GetModifiedSolutionComponents(DateTime? latestVersionDate, IOrganizationService organizationService)
        {
            // Retrieve last modified components in the Thomas Cook solution
            var fetchXml = string.Format("<fetch version='1.0' mapping='logical' distinct='true' no-lock='true'>" +
                    "  <entity name='solutioncomponent' >" +
                    "    <all-attributes/>" +
                    "    <filter>" +
                    "      <condition attribute='modifiedon' operator='on-or-after' value='{0}' />" +
                    "      <condition attribute='solutionid' operator='eq' value='{1}' />" +
                    "    </filter>" +
                    "  </entity>" +
                    "</fetch>", String.Format("{0:MM/dd/yyyy}", latestVersionDate), thomascookSolutionId);
            return organizationService.RetrieveMultiple(new FetchExpression(fetchXml))?.Entities?.ToList();
        }

        public void UpdateComponentSolution(string componentSolutionName, List<Entity> thomascookComponents, List<int> relatedTypes, IOrganizationService organizationService)
        {
            if (thomascookComponents == null) return;

            var components = thomascookComponents.Where(sc => relatedTypes.Contains(sc.GetAttributeValue<OptionSetValue>("componenttype").Value));
            if (components.Count() > 0)
            {
                // get component solution
                var solution = this.GetSolutionByName(componentSolutionName, organizationService);
                if (solution != null)
                {
                    // clear components
                    this.ClearSolutionComponents(solution, organizationService);
                    // add related components
                    this.AddSolutionComponents(solution, components.ToList(), organizationService);
                }
            }
        }

        private Entity GetSolutionByName(string uniquename, IOrganizationService organizationService)
        {
            var fetchXml = string.Format("<fetch version='1.0' mapping='logical' distinct='true' no-lock='true'>" +
                "  <entity name='solution' >" +
                "    <attribute name='solutionid' />" +
                "    <attribute name='uniquename' />" +
                "    <attribute name='version' />" +
                "    <filter type='and' >" +
                "      <condition attribute='uniquename' operator='eq' value='{0}' />" +
                "    </filter>" +
                "  </entity>" +
                "</fetch>", uniquename);
            var solutions = organizationService.RetrieveMultiple(new FetchExpression(fetchXml));

            return solutions?.Entities.FirstOrDefault();
        }

        private void ClearSolutionComponents(Entity solution, IOrganizationService organizationService)
        {
            var solutionComponents = this.GetSolutionComponents(solution.Id, organizationService);

            if (solutionComponents?.Count > 0)
            {
                foreach (var c in solutionComponents)
                {
                    var removeRequest = new RemoveSolutionComponentRequest()
                    {
                        ComponentType = c.GetAttributeValue<OptionSetValue>("componenttype").Value,
                        ComponentId = c.GetAttributeValue<Guid>("objectid"),
                        SolutionUniqueName = solution.GetAttributeValue<string>("uniquename")
                    };
                    var resp = (RemoveSolutionComponentResponse)organizationService.Execute(removeRequest);
                }
            }
        }

        private List<Entity> GetSolutionComponents(Guid solutionId, IOrganizationService organizationService)
        {
            var fetchXml = string.Format("<fetch version='1.0' mapping='logical' distinct='true' no-lock='true'>" +
                    "  <entity name='solutioncomponent' >" +
                    "    <all-attributes/>" +
                    "    <filter>" +
                    "      <condition attribute='solutionid' operator='eq' value='{0}' />" +
                    "    </filter>" +
                    "  </entity>" +
                    "</fetch>", solutionId);
            return organizationService.RetrieveMultiple(new FetchExpression(fetchXml))?.Entities?.ToList();
        }

        private void AddSolutionComponents(Entity solution, List<Entity> components, IOrganizationService organizationService)
        {
            foreach (var c in components)
            {
                AddSolutionComponentRequest addReq = new AddSolutionComponentRequest()
                {
                    ComponentType = c.GetAttributeValue<OptionSetValue>("componenttype").Value,
                    ComponentId = c.GetAttributeValue<Guid>("objectid"),
                    AddRequiredComponents = false,
                    //DoNotIncludeSubcomponents = c.GetAttributeValue<OptionSetValue>("rootcomponentbehavior")?.Value == 0,
                    SolutionUniqueName = solution.GetAttributeValue<string>("uniquename")                    
                };
                // add only for Entity component type
                if (addReq.ComponentType == (int)ComponentType.Entity)
                {
                    addReq.DoNotIncludeSubcomponents = c.GetAttributeValue<OptionSetValue>("rootcomponentbehavior")?.Value != 0;
                }
                var resp = (AddSolutionComponentResponse)organizationService.Execute(addReq);
            }
        }

        private string IncrementSolutionVersion(string solutionName, VersionPart versionPart, IOrganizationService organizationService)
        {
            var solution = this.GetSolutionByName(solutionName, organizationService);
            var newVersion = string.Empty;

            if (solution != null && solution.Contains("version"))
            {
                var version = new Version(solution.GetAttributeValue<string>("version"));

                switch (versionPart)
                {
                    case VersionPart.Major:
                        newVersion = new Version(version.Major + 1, version.Minor, version.Build).ToString();
                        break;
                    case VersionPart.Minor:
                        newVersion = new Version(version.Major, version.Minor + 1, version.Build).ToString();
                        break;
                    case VersionPart.Build:
                        newVersion = new Version(version.Major, version.Minor, version.Build + 1).ToString();
                        break;
                }
            }

            // update solution
            organizationService.Update(new Entity("solution") {
                Attributes = new AttributeCollection()
                {
                    new KeyValuePair<string, object>("solutionid", solution.Id),
                    new KeyValuePair<string, object>("version", newVersion)
                }
            });

            return newVersion;
        }

    }
}
