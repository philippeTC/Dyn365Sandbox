using System;
using Microsoft.Xrm.Sdk;
using System.ServiceModel;

using CRM.Plugins.Helpers;
using CRM.Plugins.CrmServices;
using CRM.Plugins.Logic;
using CRM.Plugins.CrmServices.XrmSdk;


namespace CRM.Plugins
{
    /// <summary>
    /// What does this plugin do?
    /// </summary>
    [RegistrationInfo(MessageName = MessageName.Update,
        PrimaryEntityName = "new_testflow",
        Stage = MessageProcessingStage.AfterMainOperationInsideTransaction)]
    
	/// Use the PluginRegistration.bat file to automatically register the plugin based 
    public class StagePlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ManagerBase managerBase = new ManagerBase(serviceProvider, false);

            IPluginExecutionContext context = managerBase.PluginExecutionContext;

            managerBase.TracingService.Trace("Begin StagePlugin:\tDepth: {0}\tCorrelationId: {1}\tContextInfo: {2} {3} {4}", context.Depth, context.CorrelationId, context.PrimaryEntityName, context.Stage, context.MessageName);

            
            #region Verify execution context

            managerBase.ValidateExecutionContext(this.GetType(), true);

            #endregion Verify execution context

            try
            {
                // Your code
                Entity target = managerBase.PluginExecutionContext.InputParameters["Target"] as Entity;
                Entity preimage = managerBase.PluginExecutionContext.PreEntityImages["preImage"];
                managerBase.TracingService.Trace("pre activestageid: {0}", preimage.GetAttributeValue<EntityReference>("activestageid")?.Id);
                managerBase.TracingService.Trace("current activestageid: {0}", target.GetAttributeValue<EntityReference>("activestageid")?.Id);

                /*
                stage GetBookings	08ac5449-98e2-476d-9352-ba51001f9b66
                stage Gen MCB		882788cb-8ba2-4701-a5bd-5fa60ee17d9c
                */

                var mcbookingGenerationBpfPath = "08ac5449-98e2-476d-9352-ba51001f9b66,882788cb-8ba2-4701-a5bd-5fa60ee17d9c";   // from parameter
                var singlecaseGenerationBpfPath = "882788cb-8ba2-4701-a5bd-5fa60ee17d9c,a6a95005-68ab-4b83-b96d-a6931e77d54e";  // from parameter
                var currentStageId = target.GetAttributeValue<EntityReference>("activestageid")?.Id.ToString();
                var prevStageId = preimage.GetAttributeValue<EntityReference>("activestageid")?.Id.ToString();

                managerBase.TracingService.Trace("mcbookingGenerationBpfPath: {0}", mcbookingGenerationBpfPath);
                managerBase.TracingService.Trace("singlecaseGenerationBpfPath: {0}", singlecaseGenerationBpfPath);

                // get related record
                var mci = new Entity("new_test");
                mci.Id = preimage.GetAttributeValue<EntityReference>("bpf_new_testid").Id;
                managerBase.TracingService.Trace("Image: Related record id: {0}", preimage.Contains("bpf_new_testid") ? preimage.GetAttributeValue<EntityReference>("bpf_new_testid").Id.ToString() : "<missing>");
                managerBase.TracingService.Trace("Context: Related record id: {0}", preimage.Contains("bpf_new_testid") ? target.GetAttributeValue<EntityReference>("bpf_new_testid").Id.ToString() : "<missing>");

                if (string.Format("{0},{1}",prevStageId,currentStageId) == mcbookingGenerationBpfPath)
                {
                    // set statuscode to 'Creating multicase bookings'
                    managerBase.TracingService.Trace("set statuscode to 'Creating multicase bookings'");
                    mci.Attributes.Add("statuscode", new OptionSetValue(100000004));
                    managerBase.OrganizationService.Update(mci);
                }
                else if (string.Format("{0},{1}",prevStageId,currentStageId) == singlecaseGenerationBpfPath)
                {
                    // set statuscode to 'Creating single cases'
                    managerBase.TracingService.Trace("set statuscode to 'Creating single cases'");
                    mci.Attributes.Add("statuscode", new OptionSetValue(100000003));
                    managerBase.OrganizationService.Update(mci);
                }
            }
            catch (Exception ex)
            {
                // The plugin message will be logged to the Plugin-Trace Log.
                // Use the Recurring Workflow Assembly "HandlePluginTraceLogs" to copy the Plugin-Trace Log message to the "Log"-entity (inf_log)
                ExceptionHandler.HandlePluginException(ex, managerBase, this.GetType().Name);
            }
        }
    }
}
