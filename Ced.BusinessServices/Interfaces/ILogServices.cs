using System;
using System.Collections.Generic;
using Ced.BusinessEntities;

namespace Ced.BusinessServices
{
    public interface ILogServices
    {
        LogEntity GetLogById(int logId);

        LogEntity GetLatestLogByAction(string controller, string action);

        IList<LogEntity> GetAllLogs(bool localIncluded = false);

        IList<LogEntityLight> GetAllLogsLight(bool localIncluded = false);

        IList<LogEntity> GetLogsByActor(string userEmail);

        IList<LogEntityLight> GetLogs(int? eventId, string userEmail, int? lastXDays);

        IList<LogEntity> GetLogsByEdition(int editionId, string entityType, string action, DateTime logDate);

        IList<LogEntity> GetRecentlyViewedEditionIds(int actorUserId, string orderExpression);

        int CreateLog(LogEntity logEntity);

        bool DeleteLog(int logId);
    }
}
