using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teleboard.Mapper.Initialize;
using Teleboard.Validation.Initialize;

namespace Teleboard.UI
{
    public static class ApplicationInitializer
    {
        public static void Initialize()
        {
            MapperInitializer.InitializModule();
            ValidationInitializer.InitializModule();
        }
    }
}