using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Diagnostics;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Json;

using Microsoft.Xrm.Sdk;

using CRM.Plugins.Helpers;
using CRM.Plugins.Logic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Xml;

namespace CRM.Plugins.CrmServices
{
    /// <summary>
    /// Provides methods for creating Log records in the CRM Environment.
    /// </summary>
    public class Log
    {
        private ManagerBase _manager;

        private const string ENTITY_EVENTLOG = "inf_log";
        public struct LogItem
        {
            public string Type;
            public string Message;
            public string ExceptionDetails;
            public OperationStatus? OperationStatus;
            public int? SubErrorCode;
        }

        public Log(ManagerBase manager)
        {
            _manager = manager;
        }

        /// <summary>
        /// Creates an Log record (inf_log) in MSCRM. 
        /// </summary>
        public void LogEvent(LogItem log)
        {
            Entity logEntity = new Entity(ENTITY_EVENTLOG);
            logEntity.Attributes.Add("inf_name", log.Type);
            logEntity.Attributes.Add("inf_typename", log.Type);
            logEntity.Attributes.Add("inf_userid", _manager.PluginExecutionContext.UserId.ToString());
            logEntity.Attributes.Add("inf_messagename", _manager.PluginExecutionContext.MessageName);
            logEntity.Attributes.Add("inf_primaryentity", _manager.PluginExecutionContext.PrimaryEntityName);
            logEntity.Attributes.Add("inf_operationtype", "Plugin");
            logEntity.Attributes.Add("inf_depth", _manager.PluginExecutionContext.Depth);
            logEntity.Attributes.Add("inf_correlationid", _manager.PluginExecutionContext.CorrelationId.ToString());
            logEntity.Attributes.Add("inf_mode", new OptionSetValue(_manager.PluginExecutionContext.Mode));
            logEntity.Attributes.Add("inf_messageblock", log.Message);
            logEntity.Attributes.Add("inf_exceptiondetails", log.ExceptionDetails);
            logEntity.Attributes.Add("inf_operationstatus", log.OperationStatus.HasValue ? log.OperationStatus.Value.ToString() : null);
            logEntity.Attributes.Add("inf_suberrorcode", log.SubErrorCode.HasValue ? log.SubErrorCode.Value.ToString() : null);
            _manager.OrganizationService.Create(logEntity);
        }

        /// <summary>
        /// Creates an Log record (inf_log) in MSCRM. 
        /// </summary>
        /// <param name="ex"></param>
        public void LogInfo(string message)
        {
            LogEvent(new LogItem() { Message = message, Type = "Info" });
        }

        /// <summary>
        /// Traces the exception in MS CRM and creates an Log record (inf_log) in MSCRM. 
        /// </summary>
        /// <param name="ex"></param>
        public void LogException(Exception ex)
        {

            LogItem eventLog = new LogItem()
            {
                Type = ex.GetType().ToString(),
                Message = ex.Message,
            };

            StringBuilder details = new StringBuilder();

            if (ex.Data != null && ex.Data.Count > 0)
            {
                foreach (object key in ex.Data.Keys)
                {
                    details.Append("<" + key.ToString() + ">");
                    details.Append(ex.Data[key].ToString());
                    details.Append("</" + key.ToString() + ">");
                }
                details.Append(Environment.NewLine);
            }
            if (ex.StackTrace != null)
            {
                details.Append(Environment.NewLine + "  <STACKTRACE>");
                details.Append(ex.StackTrace);
                details.Append(Environment.NewLine + "  </STACKTRACE>");
            }
            if (ex.InnerException != null)
            {
                details.Append(Environment.NewLine + "  <INNEREXCEPTIONS>");
                AppendInnerExceptions(ex.InnerException, details);
                details.Append(Environment.NewLine + "  </INNEREXCEPTIONS>");
            }


         
            eventLog.ExceptionDetails = details.ToString();
            LogEvent(eventLog);


        }
        private void AppendInnerExceptions(Exception ex, StringBuilder message)
        {
            if (ex.InnerException != null)
            {
                AppendInnerExceptions(ex.InnerException, message);
            }
            message.Append(Environment.NewLine + "    <INNEREXCEPTION type=\"" + ex.GetType().ToString() + "\">");
            message.Append(Environment.NewLine + "    <INNEREXCEPTIONMESSAGE>" + ex.Message + "</INNEREXCEPTIONMESSAGE>");
            message.Append(Environment.NewLine + "    <INNEREXCEPTIONSTACKTRACE>" + ex.StackTrace + "</INNEREXCEPTIONSTACKTRACE>");
            message.Append(Environment.NewLine + "    " + GetFaultDetailMessage(ex));
            message.Append(Environment.NewLine + "    </INNEREXCEPTION>");
        }
        private string GetFaultDetailMessage(Exception ex)
        {
            //System.Web.Services.Protocols.SoapException soapException = null;
            FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> faultException = null;

            if (ex is FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault>)
            {
                faultException = (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault>)ex;
                if (faultException.Detail != null)
                {
                    return ("<FAULTEXCEPTION>" + faultException.Detail.Message + "</FAULTEXCEPTION>");
                }
                else
                {
                    return ("<FAULTEXCEPTION/>");
                }
            }
            else
            {
                return ("<FAULTEXCEPTION/>");
            }
        }


    }
}
