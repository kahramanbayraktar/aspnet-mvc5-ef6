using AutoMapper;
using Ced.BusinessEntities;
using Ced.Data.Models;
using Ced.Data.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace Ced.BusinessServices
{
    public class EditionVisitorServices : IEditionVisitorServices
    {
        private readonly UnitOfWork _unitOfWork;
        
        public EditionVisitorServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = (UnitOfWork)unitOfWork;
        }

        public EditionVisitorEntity GetEditionVisitorById(int editionVisitorId)
        {
            var query = _unitOfWork.EditionVisitorRepository.GetManyQueryable(x => x.EditionVisitorId == editionVisitorId);

            var editionVisitor = query.SingleOrDefault();
            if (editionVisitor != null)
            {
                var editionVisitorModel = Mapper.Map<EditionVisitor, EditionVisitorEntity>(editionVisitor);
                return editionVisitorModel;
            }
            return null;
        }

        public EditionVisitorEntity GetEditionVisitorByDayNumber(int editionId, byte dayNumber)
        {
            var query = _unitOfWork.EditionVisitorRepository.GetManyQueryable(x => x.EditionId == editionId && x.DayNumber == dayNumber);

            var editionVisitor = query.SingleOrDefault();
            if (editionVisitor != null)
            {
                var editionVisitorModel = Mapper.Map<EditionVisitor, EditionVisitorEntity>(editionVisitor);
                return editionVisitorModel;
            }
            return null;
        }

        public IList<EditionVisitorEntity> GetEditionVisitors(int editionId)
        {
            var editionVisitors = _unitOfWork.EditionVisitorRepository.GetManyQueryable(x => x.EditionId == editionId).ToList();

            if (editionVisitors.Any())
            {
                var editionVisitorModels = Mapper.Map<IList<EditionVisitor>, IList<EditionVisitorEntity>>(editionVisitors);
                return editionVisitorModels;
            }
            return new List<EditionVisitorEntity>();
        }

        public int CreateEditionVisitor(EditionVisitorEntity editionVisitorEntity, int userId)
        {
            var editionVisitor = Mapper.Map<EditionVisitorEntity, EditionVisitor>(editionVisitorEntity);
            editionVisitor.CreatedOn = DateTime.Now;
            editionVisitor.CreatedBy = userId;

            _unitOfWork.EditionVisitorRepository.Insert(editionVisitor);
            _unitOfWork.Save();
            return editionVisitor.EditionVisitorId;
        }

        public bool UpdateEditionVisitor(EditionVisitorEntity editionVisitorEntity, int userId)
        {
            var success = false;
            if (editionVisitorEntity != null)
            {
                var editionVisitor = _unitOfWork.EditionVisitorRepository.GetSingle(x =>
                    x.EditionId == editionVisitorEntity.EditionId && x.DayNumber == editionVisitorEntity.DayNumber);
                if (editionVisitor != null)
                {
                    Mapper.Map(editionVisitorEntity, editionVisitor);

                    editionVisitor.UpdatedOn = DateTime.Now;
                    editionVisitor.UpdatedBy = userId;

                    _unitOfWork.EditionVisitorRepository.Update(editionVisitor);
                    _unitOfWork.Save();

                    success = true;
                }
            }
            return success;
        }

        public void CreateOrUpdateEditionVisitor(EditionVisitorEntity editionVisitorEntity, int userId)
        {
            if (editionVisitorEntity != null)
            {
                var editionVisitor = GetEditionVisitorByDayNumber(editionVisitorEntity.EditionId, editionVisitorEntity.DayNumber);
                if (editionVisitor == null)
                {
                    CreateEditionVisitor(editionVisitorEntity, userId);
                }
                else
                {
                    UpdateEditionVisitor(editionVisitorEntity, userId);
                }
            }
        }

        public void CreateOrUpdateEditionVisitors(IList<EditionVisitorEntity> editionVisitorEntities, int userId)
        {
            using (var scope = new TransactionScope())
            {
                foreach (var editionVisitorEntity in editionVisitorEntities)
                {
                    CreateOrUpdateEditionVisitor(editionVisitorEntity, userId);
                }

                scope.Complete();
            }
        }
    }
}
