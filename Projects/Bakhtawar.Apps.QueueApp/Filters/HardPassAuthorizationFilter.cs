using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hangfire.Dashboard;

namespace Bakhtawar.Apps.QueueApp.Filters
{
    public class HardPassAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            return true;
        }
    }
}
