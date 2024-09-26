﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace IssueManagementSystem
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()

        {
            Database.SetInitializer<Models.FLINTEC_Context>(null);

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            var connectionString = ConfigurationManager.ConnectionStrings["issue_management_systemEntities2"];
            var connectionString2 = ConfigurationManager.ConnectionStrings["BigRedEntities2"];
            SqlDependency.Start(@connectionString.ToString());
            SqlDependency.Start(@connectionString2.ToString());
            JobScheduler.Start();
        }
    }
}

