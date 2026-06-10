using NLog;
using NLog.Config;
using NLog.Targets;

namespace Mahou {
    internal class LogHelper {
        public static void ConfigureNlog() {         // Step 1. Create configuration object 
            AppPaths.EnsureLogDirectory();
            var config = new LoggingConfiguration();

            // Step 2. Create targets and add them to the configuration 
            var fileTarget = new FileTarget();
            config.AddTarget("file", fileTarget);

            // Step 3. Set target properties 
            fileTarget.FileName = System.IO.Path.Combine(AppPaths.LogDirectory, "${shortdate}.log");
#if DEBUG
            fileTarget.Layout = @"${longdate} ${uppercase:${level}} ${message} ${exception:format=toString}";
#else
            fileTarget.Layout = @"${longdate} ${uppercase:${level}} ${message} ${exception:format=Type,Message}";
#endif
            fileTarget.ArchiveAboveSize = 1024 * 1024;
            fileTarget.MaxArchiveFiles = 7;
            fileTarget.KeepFileOpen = false;

            LoggingRule rule2;
#if DEBUG
            rule2 = new LoggingRule("*", LogLevel.Trace, fileTarget);
#else
            rule2= new LoggingRule("*", LogLevel.Warn, fileTarget);
#endif
            config.LoggingRules.Add(rule2);

            // Step 5. Activate the configuration
            LogManager.Configuration = config;
        }
    }
}
