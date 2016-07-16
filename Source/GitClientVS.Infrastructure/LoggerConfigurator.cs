using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;

namespace GitClientVS.Infrastructure
{
    public static class LoggerConfigurator
    {
        public static void  Setup()
        {
            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();

            var patternLayout = new PatternLayout
            {
                ConversionPattern = "%date [%thread] %-5level %logger - %message%newline"
            };

            patternLayout.ActivateOptions();

            var roller = new RollingFileAppender
            {
                AppendToFile = false,
                File = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"GitClientVSExtension\Logs\logs.txt"),
                Layout = patternLayout,
                MaxSizeRollBackups = 5,
                MaximumFileSize = "1GB",
                RollingStyle = RollingFileAppender.RollingMode.Size,
                StaticLogFileName = true
            };
            roller.ActivateOptions();
            hierarchy.Root.AddAppender(roller);

            hierarchy.Root.Level = Level.All;
            hierarchy.Configured = true;
        }
    }
}
