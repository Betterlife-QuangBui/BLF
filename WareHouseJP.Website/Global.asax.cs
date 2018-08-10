using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WareHouseJP.Website.Helpers;

namespace WareHouseJP.Website
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            try
            {
                String start_one = WebConfigurationManager.AppSettings["update_data_at_one"];
                IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
                scheduler.Start();
                IJobDetail job = JobBuilder.Create<QuartzSchedule>()
                    .WithIdentity("jobName", "jobGroup")
                    .Build();
                var trigger = TriggerBuilder.Create()
                .WithIdentity("trigger3", "group1")
                .WithCronSchedule(start_one)
                .ForJob("jobName", "jobGroup")
                .Build();
                scheduler.ScheduleJob(job, trigger);
            }
            catch { }
        }
    }
}
