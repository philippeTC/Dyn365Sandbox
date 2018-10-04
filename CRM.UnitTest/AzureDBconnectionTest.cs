using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
    public class AzureDBconnectionTest
    {
        
        [TestMethod]
        public void TestActiveDirectoryPassword()
        {
            var connectionstring = "Server=tcp:asql-cview-stg-tcwe-01.database.windows.net,1433;Initial Catalog=DataExportSvc;Persist Security Info=False;User ID=xxxxxxxxx;Password=xxxxxxxx;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                SqlCommand command = new SqlCommand("select top 10 * from tc_contact", connection);
                command.Connection.Open();
                var i = command.ExecuteScalar();
            }
        }

    }
}
