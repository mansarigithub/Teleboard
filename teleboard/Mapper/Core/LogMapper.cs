using System;
using System.Linq;
using Teleboard.DomainModel.Core;
using Teleboard.Mapper.Attributes;
using Teleboard.Mapper.Profile;
using Teleboard.PresentationModel.Model.Log;

namespace Teleboard.Mapper.Core
{
    [ObjectMapper]
    public static class LogMapper
    {
        public static void CreateMap(AutoMapperProfile profile)
        {
            profile.CreateMap<Log, LogPM>();
            profile.CreateMap<LogPM, Log>();
        }

        public static Log GetLog(this LogPM presentationModel)
        {
            return AutoMapper.Mapper.Map<LogPM, Log>(presentationModel);
        }

        public static LogPM GetLogPM(this Log domainModel)
        {
            var logPm = AutoMapper.Mapper.Map<Log, LogPM>(domainModel);
            if (logPm.Description != null)
                logPm.Description = logPm.Description.Replace(Environment.NewLine, "<br/>");
            return logPm;
        }
    }
}

