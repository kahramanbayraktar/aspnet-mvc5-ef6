using AutoMapper;
using Ced.BusinessEntities;
using Ced.Data.Models;
using Ced.Data.UnitOfWork;
using ITE.Utility.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Ced.BusinessServices
{
    public class LogServices : ILogServices
    {
        private readonly UnitOfWork _unitOfWork;

        public LogServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = (UnitOfWork)unitOfWork;
        }

        public LogEntity GetLogById(int logId)
        {
            var log = _unitOfWork.LogRepository.GetById(logId);
            if (log != null)
            {
                var logModel = Mapper.Map<Log, LogEntity>(log);
                return logModel;
            }
            return null;
        }

        public LogEntity GetLatestLogByAction(string controller, string action)
        {
            controller = controller.ToLower();
            action = action.ToLower();

            var query = _unitOfWork.LogRepository.GetManyQueryable(x =>
                x.Controller.ToLower() == controller && x.Action.ToLower() == action);

            var logs = query.OrderByDescending(x => x.CreatedOn).Take(1).ToList();

            if (logs.Any())
            {
                var logModel = Mapper.Map<Log, LogEntity>(logs.First());
                return logModel;
            }

            return null;
        }

        public IList<LogEntity> GetAllLogs(bool localIncluded = false)
        {
            var query = _unitOfWork.LogRepository.GetAll();
            var logs = query.ToList();
            if (logs.Any())
            {
                var logsModel = Mapper.Map<List<Log>, List<LogEntity>>(logs);
                return logsModel;
            }
            return new List<LogEntity>();
        }

        public IList<LogEntityLight> GetAllLogsLight(bool localIncluded = false)
        {
            var query = _unitOfWork.LogRepository.GetManyQueryableProjected<LogEntityLight>(x => true);
            if (!localIncluded) query = query.Where(x => x.Ip != "::1");
            var logs = query.ToList();
            return logs;
        }

        public IList<LogEntity> GetLogsByActor(string userEmail)
        {
            userEmail = userEmail.Trim().ToLower();
            var query = _unitOfWork.LogRepository.GetManyQueryable(x => x.ActorUserEmail.ToLower() == userEmail);

            var logs = query.ToList();

            if (logs.Any())
            {
                var logsModel = Mapper.Map<List<Log>, List<LogEntity>>(logs);
                return logsModel;
            }
            return new List<LogEntity>();
        }

        public IList<LogEntityLight> GetLogs(int? eventId, string userEmail, int? lastXDays)
        {
            var now = DateTime.Now.Date;

            var query = _unitOfWork.LogRepository.GetManyQueryable(x => true);

            if (eventId > 0)
                query = query.Where(x => x.EventId == eventId.Value);

            if (!string.IsNullOrWhiteSpace(userEmail))
                query = query.Where(x => x.ActorUserEmail.ToLower() == userEmail.ToLower());

            if (lastXDays > 0)
                query = query.Where(x => DbFunctions.DiffDays(DbFunctions.TruncateTime(x.CreatedOn), now).Value <= lastXDays);

            var logs = query.OrderByDescending(x => x.CreatedOn).ToList();

            if (logs.Any())
            {
                var logsModel = Mapper.Map<List<Log>, List<LogEntityLight>>(logs);
                return logsModel;
            }
            return new List<LogEntityLight>();
        }

        public IList<LogEntity> GetLogsByEdition(int editionId, string entityType, string action, DateTime logDate)
        {
            var query = _unitOfWork.LogRepository.GetManyQueryable(x => x.EntityId == editionId);

            query = query.Where(x => x.EntityType.ToLower() == entityType.ToLower() && x.Action.ToLower() == action.ToLower() && DbFunctions.DiffDays(x.CreatedOn, logDate) == 0);

            var logs = query.ToList();

            if (logs.Any())
            {
                var logsModel = Mapper.Map<List<Log>, List<LogEntity>>(logs);
                return logsModel;
            }
            return new List<LogEntity>();
        }

        //public IList<LogEntity> GetRecentlyViewedEvents(int actorUserId, string orderExpression)
        //{
        //    var query = _unitOfWork.LogRepository.GetManyQueryable(x => x.ActorUser == actorUserId);
        //    query = query.Where(x => x.Controller.ToLower() == "edition"
        //        && (x.Action.ToLower() == "details" || x.Action.ToLower() == "edit")
        //        && x.MethodType.ToLower() == "get");
        //    query = query.OrderBy(orderExpression);

        //    var logs = query.ToList();

        //    if (logs.Any())
        //    {
        //        var logsModel = Mapper.Map<List<Log>, List<LogEntity>>(logs);
        //        return logsModel;
        //    }
        //    return new List<LogEntity>();
        //}

        //public IList<LogEntity> GetRecentlyViewedEvents(int actorUserId, string orderExpression)
        //{
        //    var query = _unitOfWork.LogRepository.GetManyQueryable(x => x.ActorUser == actorUserId);
        //    query = query.Where(x =>
        //        (x.ActionType == "EventDashboard" || x.ActionType == "EditionDetails" || x.ActionType == "EditionEdit")
        //        && x.EntityName != null)
        //        .GroupBy(x => new {x.EntityId, x.EntityName, x.Description})
        //        //.OrderBy(orderExpression)
        //        .Take(10)
        //        .Select(g => new Log
        //        {
        //            EntityId = g.Key.EntityId,
        //            EntityName = g.Key.EntityName,
        //            Description = g.Key.Description
        //        })
        //        .Distinct();

        //    var logs = query.ToList();

        //    if (logs.Any())
        //    {
        //        var logsModel = Mapper.Map<List<Log>, List<LogEntity>>(logs);
        //        return logsModel;
        //    }
        //    return new List<LogEntity>();
        //}

        public IList<LogEntity> GetRecentlyViewedEditionIds(int actorUserId, string orderExpression)
        {
            var query = _unitOfWork.LogRepository.GetManyQueryable(x => x.ActorUserId == actorUserId);
            query = query.Where(x =>
                (x.ActionType == "EventDashboard" || x.ActionType == "EditionDetails" || x.ActionType == "EditionEdit"))
                .GroupBy(x => new { x.EntityId, x.EntityType })
                //.OrderBy(orderExpression)
                .Take(10)
                .Select(g => new Log
                {
                    EntityId = g.Key.EntityId
                })
                .Distinct();

            var logs = query.ToList();

            if (logs.Any())
            {
                var logsModel = Mapper.Map<List<Log>, List<LogEntity>>(logs);
                return logsModel;
            }
            return new List<LogEntity>();
        }

        public int CreateLog(LogEntity logEntity)
        {
            var log = Mapper.Map<LogEntity, Log>(logEntity);

            log.AdditionalInfo = log.AdditionalInfo.Substr(1000);
            log.CreatedOn = DateTime.Now;

            _unitOfWork.LogRepository.Insert(log);
            _unitOfWork.Save();
            return log.LogId;
        }

        public bool DeleteLog(int logId)
        {
            var success = false;
            if (logId > 0)
            {
                var log = _unitOfWork.LogRepository.GetById(logId);
                if (log != null)
                {
                    _unitOfWork.LogRepository.Delete(log);
                    _unitOfWork.Save();
                    success = true;
                }
            }
            return success;
        }
    }
}
