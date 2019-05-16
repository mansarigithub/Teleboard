using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Teleboard.Common.ExtensionMethod;
using Teleboard.DataAccess.Context;
using Teleboard.PresentationModel.Model.Device;
using Teleboard.Common.Data;
using System.Data.Entity;
using Teleboard.Mapper.Core;
using Teleboard.PresentationModel.Model.Log;
using Teleboard.Common.Enum;
using Teleboard.DomainModel.Core;

namespace Teleboard.Business.Core
{
    public class LogBiz
    {
        private ApplicationDbContext Context { get; set; }

        public LogBiz(ApplicationDbContext context)
        {
            Context = context;
        }

        public void CreateLog(LogType type, string title, string description)
        {
            Context.Logs.Add(new Log() {
                CreateDate = DateTime.Now,
                Type = type,
                Title = title,
                Description = description,
            });
            Context.SaveChanges();
        }

        public void DeleteLog(int id)
        {
            Context.Logs.Remove(Context.Logs.Find(id));
            Context.SaveChanges();
        }

        public DataSourceResult ReadLogs(DataSourceRequest request)
        {
            return Context.Logs
                .OrderByDescending(log => log.Id)
                .MapTo<LogPM>()
                .ToDataSourceResult(request);
        }
    }
}
