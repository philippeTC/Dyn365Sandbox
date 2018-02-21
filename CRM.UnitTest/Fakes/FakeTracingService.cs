using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//namespace CRM.UnitTest.Fakes
namespace CRM.UnitTest.Fakes
{
    public class FakeTracingService : ITracingService
    {
        public void Trace(string format, params object[] args)
        {
            System.Diagnostics.Debug.WriteLine(format, args);
        }
    }
}
