using Ced.BusinessEntities;
using ITE.Logger;
using ITE.Logger.Adapters;
using log4net;
using Newtonsoft.Json;

namespace Ced.BusinessServices.Helpers
{
    public class ExternalLogHelper
    {
        public static void Log(string message, LoggingEventType logLevel)
        {
            var logger = new Log4NetAdapter(LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType));
            logger.Log(new LogEntry(logLevel, message));
        }

        //public static void Log(string message, EditionEntity edition, LoggingEventType logLevel)
        //{
        //    message += " EventId=" + edition.EventId + ", EditionId=" + edition.EditionId;
        //    var logger = new Log4NetAdapter(LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType));
        //    logger.Log(new LogEntry(logLevel, message));
        //}

        public static void Log(LogEntity log, LoggingEventType logLevel)
        {
            var message = JsonConvert.SerializeObject(log);
            var logger = new Log4NetAdapter(LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType));
            logger.Log(new LogEntry(logLevel, message));
        }
    }
}