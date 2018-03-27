using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Linq;
using System.Text;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

using CRM.Plugins.Helpers;
using CRM.Plugins.CrmServices;

namespace CRM.Plugins.Logic
{
    /// <summary>
    /// Contains methods which are specific for the current CRM Organization
    /// </summary>
    public class ManagerBase : EntityManager
    {
        private static object sequenceLock = new Object();

        public ManagerBase(IServiceProvider serviceProvider, bool useCurrentUserId)
            : base(serviceProvider, useCurrentUserId)
        {
        }

        /// <summary>
        /// Retrieves the value of the entity with the specified key. The value is stored in the MemoryCache for 10 minutes.
        /// To force the cache to be cleared; When running in Sandbox Isolation mode, restart the "CRM Sandbox Processing Service", otherwise restart the "CRMAppPool" and the "CRM Asynchronous Service"
        /// </summary>
        /// <param name="organizationService"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetParameterValue(string key)
        {
            ObjectCache cache = MemoryCache.Default;

            // Store data in the cache
            CacheItemPolicy cacheItemPolicy = new CacheItemPolicy() {
                AbsoluteExpiration = DateTime.Now.AddMinutes(10)
            };

            string value = (string)cache.Get(key);

            if (value == null)
            {
                value = String.Empty;

                QueryByAttribute queryByAttribute = new QueryByAttribute("inf_parameter")
                {
                    ColumnSet = new ColumnSet("inf_value"),
                    Attributes = { "inf_key" },
                    Values = { key }
                };

                EntityCollection col = base.OrganizationService.RetrieveMultiple(queryByAttribute);

                if (col != null && col.Entities.Count > 0)
                {
                    value = col.Entities[0]["inf_value"].ToString();
                }

                cache.Add(key, value, cacheItemPolicy);
                TracingService.Trace("Parameter '{0}' added to MemoryCache, expires on '{1}'",key, cacheItemPolicy.AbsoluteExpiration);
            }

            return value;
        }

        /// <summary>
        /// Retrieves the value of the entity with the specified key
        /// </summary>
        /// <param name="organizationService"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetParameterExtendedValue(string key)
        {
            QueryByAttribute queryByAttribute = new QueryByAttribute();
            queryByAttribute.EntityName = "inf_parameter";
            queryByAttribute.Attributes.Add("inf_key");
            queryByAttribute.Values.Add(key);
            queryByAttribute.ColumnSet = new ColumnSet(new string[1] { "inf_extendedvalue" });

            DataCollection<Entity> dataCollection = base.OrganizationService.RetrieveMultiple(queryByAttribute).Entities;
            if (dataCollection != null && dataCollection.Count > 0)
            {
                return ((Entity)dataCollection[0]).Attributes["inf_extendedvalue"].ToString();
            }
            return string.Empty;
        }

        public Guid GetParentUserId(IPluginExecutionContext pluginExecutionContext)
        {
            if (pluginExecutionContext.ParentContext == null)
            {
                return pluginExecutionContext.UserId;
            }
            return GetParentUserId(pluginExecutionContext.ParentContext);
        }

        /// <summary>
        /// Validates the registration requirements against the runtime execution. Requirements can be specified with "RegistrationInfo"-attributes on plugin class level
        /// Returns false if none of the registration set requirements is valid, returns true if at least one set of registration requirement is valid.
        /// </summary>
        /// <param name="pluginType">Pass the type of your plugin class - e.g. typeof(yourplugin)</param>
        /// <param name="throwError">Throw an exception instead of returning false</param>
        /// <returns></returns>
        public bool ValidateExecutionContext(Type pluginType, bool throwError)
        {
            int numberOfRegistrationRequirements = 0;
            string validationErrors = string.Empty;

            System.Reflection.MemberInfo pluginTypeInfo = pluginType;
            object[] attributes = pluginTypeInfo.GetCustomAttributes(true);
            TracingService.Trace("----- Start Execution Context Validation -----");
            foreach (object attribute in attributes)
            {
                if (attribute is RegistrationInfo)
                {
                    RegistrationInfo registrationInfo = attribute as RegistrationInfo;
                    numberOfRegistrationRequirements++;

                    bool isValid = false;
                    bool messageNameIsEqual = true;
                    bool modeIsEqual = true;
                    bool stageIsEqual = true;
                    bool preEntityImageNameIsEqual = true;
                    bool primaryEntityNameIsEqual = true;
                    bool postEntityImageNameIsEqual = true;

                    TracingService.Trace("-- Registration Requirement {0}", numberOfRegistrationRequirements);
                    if (!String.IsNullOrEmpty(registrationInfo.MessageName))
                    {
                        messageNameIsEqual = (PluginExecutionContext.MessageName == registrationInfo.MessageName);
                        TracingService.Trace("---- messageNameIsEqual : " + messageNameIsEqual);
                    }
                    if (!String.IsNullOrEmpty(registrationInfo.PrimaryEntityName))
                    {
                        primaryEntityNameIsEqual = (PluginExecutionContext.PrimaryEntityName == registrationInfo.PrimaryEntityName);
                        TracingService.Trace("---- primaryEntityNameIsEqual (" + PluginExecutionContext.PrimaryEntityName + ") : " + primaryEntityNameIsEqual);
                    }
                    //if (registrationInfo.Mode != 0) // Mode will always be validated (also if it is not specified) because an int value will always be initiated to 0.
                    //{
                    modeIsEqual = (PluginExecutionContext.Mode == registrationInfo.Mode);
                    TracingService.Trace("---- modeIsEqual : " + modeIsEqual);
                    //}
                    if (registrationInfo.Stage != 0)
                    {
                        stageIsEqual = (PluginExecutionContext.Stage == registrationInfo.Stage);
                        TracingService.Trace("---- stageIsEqual : " + stageIsEqual);
                    }
                    if (!String.IsNullOrEmpty(registrationInfo.PreEntityImageName))
                    {
                        CRMImage preEntityImage = new CRMImage(registrationInfo.PreEntityImageName);
                        preEntityImageNameIsEqual = PluginExecutionContext.PreEntityImages.ContainsKey(preEntityImage.Name);
                        TracingService.Trace("---- preEntityImageNameIsEqual : " + preEntityImageNameIsEqual);
                    }
                    if (!String.IsNullOrEmpty(registrationInfo.PostEntityImageName))
                    {
                        CRMImage postEntityImage = new CRMImage(registrationInfo.PostEntityImageName);
                        postEntityImageNameIsEqual = PluginExecutionContext.PostEntityImages.ContainsKey(postEntityImage.Name);
                        TracingService.Trace("---- postEntityImageNameIsEqual : " + postEntityImageNameIsEqual);
                    }

                    isValid = messageNameIsEqual
                                && primaryEntityNameIsEqual
                                && stageIsEqual
                                && preEntityImageNameIsEqual
                                && postEntityImageNameIsEqual;

                    if (isValid)
                    {
                        TracingService.Trace("----- End Execution Context Validation -----");
                        return true;
                    }
                    validationErrors += numberOfRegistrationRequirements + ") " + registrationInfo.ToString() + Environment.NewLine;
                }
            }
            if (numberOfRegistrationRequirements > 0)
            {
                TracingService.Trace("----- End Execution Context Validation -----");
                if (throwError) throw new Exception(String.Format("Incorrect Plugin registration, none of the specified registration requirements are valid : {0}", validationErrors));
                return false;
            }
            return true;
        }

        private class CRMImage
        {
            public CRMImage(string image)
            {
                string[] imageItems = image.Split(new string[] { "|" }, StringSplitOptions.None);
                this.Name = imageItems[0];
                if (imageItems.Length > 1)
                {
                    this.Attributes = imageItems[1].Split(new string[] { ";" }, StringSplitOptions.None);
                }
            }
            public string Name { get; set; }
            public string[] Attributes { get; set; }
        }

    }
}
