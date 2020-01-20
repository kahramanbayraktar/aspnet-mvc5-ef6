using Ced.BusinessEntities;
using Ced.BusinessServices.Interfaces;
using Ced.Data.UnitOfWork;
using Ced.Utility.Web;
using ITE.Utility.Extensions;
using System;
using System.Web;

namespace Ced.BusinessServices
{
    public class ServiceBase : IServiceBase
    {
        private readonly LogServices _logServices;

        protected ServiceBase(IUnitOfWork unitOfWork)
        {
            _logServices = new LogServices(unitOfWork);
        }

        public LogEntity CreateInternalLog(Exception exc, string extraInfo = null)
        {
            var message = exc.GetFullMessage();
            if (!string.IsNullOrWhiteSpace(extraInfo))
                message = extraInfo + " | " + message;

            var log = new LogEntity
            {
                Url = HttpContext.Current.Request.RawUrl,
                ActorUserId = Helpers.Constants.AutoIntegrationUserId,
                ActorUserEmail = null,
                Controller = GetType().ToString(),
                Action = System.Reflection.MethodBase.GetCurrentMethod().Name,
                MethodType = HttpContext.Current.Request.HttpMethod,
                Ip = WebConfigHelper.IsLocal ? Helpers.Constants.LocalIp : HttpContext.Current.Request.UserHostAddress,
                AdditionalInfo = message,
                CreatedOn = DateTime.Now
            };
            log.LogId = _logServices.CreateLog(log);
            return log;
        }
    }
}
