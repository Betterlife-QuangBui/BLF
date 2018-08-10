using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using WareHouseJP.Website.Models;

namespace WareHouseJP.Website.Helpers
{
    public class QuartzSchedule : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                Thread tSendMails;
                tSendMails = new System.Threading.Thread(delegate ()
                {
                    //get list booking need alert email
                    WareHouseJPDB db = new WareHouseJPDB();
                    foreach (var item in db.ExportGoods.Where(n => n.AirId == null))
                    {
                        string html = "Body send email";
                        GMail.Send(item.Agency.Email, "[V/v] Yêu cầu booking " + DateTime.Now.ToString("dd.MM.yyyy"), html);
                    }
                });
                tSendMails.IsBackground = true;
                tSendMails.Priority = ThreadPriority.Highest;
                tSendMails.Start();
            }
            catch { }
        }
    }
}