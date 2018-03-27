using CRM.Plugins.Logic;
using CRM.Plugins.CrmServices;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace CRM.Plugins.Helpers
{
    public static class ExceptionHandler
    {
        public static void HandlePluginException(Exception pluginexception, ManagerBase manager, string pluginname)
        {
            var pluginexceptiontype = pluginexception.GetType();
            if (pluginexceptiontype == typeof(InvalidPluginExecutionException))
            {
                throw pluginexception;
            }
            else if (pluginexceptiontype == typeof(ApplicationException))
            {
                HandleApplicationException((ApplicationException)pluginexception, manager);
            }
            else if (pluginexceptiontype == typeof(NonBlockingException))
            {
                HandleNonBlockingException((NonBlockingException)pluginexception, manager);
            }
            else
            {
                throw new InvalidPluginExecutionException(String.Format("Unexpected error occurred in the {0} plug-in.", pluginname), pluginexception);
            }
        }
 
        private static void HandleApplicationException(ApplicationException ex, ManagerBase manager)
        {
            string message = null;
            var userId = manager.GetParentUserId(manager.PluginExecutionContext);
            var userSettings = manager.GetUserSettings(userId);

            if (ex.Data != null && ex.Data.Values != null)
            {
                message = CrmSdkHelper.Translate(ex.Message, CrmSdkHelper.GetUserCulture(userSettings), ex.Data.Values.Cast<String>().ToArray());
            }
            else
            {
                message = CrmSdkHelper.Translate(ex.Message, CrmSdkHelper.GetUserCulture(userSettings));
            }       
            if (String.IsNullOrEmpty(message)) message = ex.Message;   
            throw new InvalidPluginExecutionException(OperationStatus.Succeeded,111,message);
    }

        private static void HandleNonBlockingException(NonBlockingException ex, ManagerBase manager)
        {
            LogException(ex, manager);
        }

        private static void LogException(Exception pluginexception, ManagerBase manager)
        {
            // Add target of plugin as Additional Info
            if (manager != null && manager.PluginExecutionContext != null && manager.PluginExecutionContext.InputParameters != null &&
                (manager.PluginExecutionContext.InputParameters.Contains("Target") || manager.PluginExecutionContext.InputParameters.Contains("EntityMoniker")))
            {
                Object obj = null;

                if (manager.PluginExecutionContext.InputParameters.Contains("Target"))
                {
                    obj = manager.PluginExecutionContext.InputParameters["Target"];
                }
                else if (manager.PluginExecutionContext.InputParameters.Contains("EntityMoniker"))
                {
                    obj = manager.PluginExecutionContext.InputParameters["EntityMoniker"];
                }

                var targetstr = string.Empty;

                if (obj.GetType() == typeof(EntityReference))
                {
                    EntityReference entityRef = obj as EntityReference;

                    if (entityRef != null)
                    {
                        if (false == String.IsNullOrEmpty(entityRef.Name))
                        {
                            targetstr = String.Format("{0}: {1}|{2}", entityRef.LogicalName, entityRef.Id, entityRef.Name);
                        }
                        else
                        {
                            targetstr = String.Format("{0}: {1}", entityRef.LogicalName, entityRef.Id);
                        }
                    }
                }
                else if (obj.GetType() == typeof(Entity))
                {
                    Entity entity = obj as Entity;

                    if (entity != null)
                    {
                        EntityReference entityRef = entity.ToEntityReference();

                        if (entityRef != null)
                        {
                            if (false == String.IsNullOrEmpty(entityRef.Name))
                            {
                                targetstr = String.Format("{0}: {1}|{2}", entityRef.LogicalName, entityRef.Id, entityRef.Name);
                            }
                            else
                            {
                                targetstr = String.Format("{0}: {1}", entityRef.LogicalName, entityRef.Id);
                            }
                        }
                    }
                }

                if (false == String.IsNullOrEmpty(targetstr))
                {
                    pluginexception.Data.Add("target", targetstr);
                }
            }

            // Add execution info of plugin as Additional Info

            if (manager != null && manager.PluginExecutionContext != null)
            {
                // Sets the GUID of the system user for whom the plug-in invokes web service methods on behalf of. (inherited from IExecutionContext)
                pluginexception.Data.Add("Execution UserId", manager.PluginExecutionContext.UserId);
                pluginexception.Data.Add("InitiatingUserId", manager.PluginExecutionContext.InitiatingUserId);
            }

            // Log the exception
            Log log = new Log(manager);
            log.LogException(pluginexception);
        }

    }


    public class NonBlockingException : Exception
    {
        public NonBlockingException(string message,Exception innerException) : base(message,innerException)
        {
        }
    }

}
