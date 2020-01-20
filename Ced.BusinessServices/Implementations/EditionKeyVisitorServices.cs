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
    public class EditionKeyVisitorServices : IEditionKeyVisitorServices
    {
        private readonly UnitOfWork _unitOfWork;
        
        public EditionKeyVisitorServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = (UnitOfWork)unitOfWork;
        }

        public EditionKeyVisitorEntity GetEditionKeyVisitorById(int editionKeyVisitorId)
        {
            var query = _unitOfWork.EditionKeyVisitorRepository.GetManyQueryable(x => x.EditionKeyVisitorId == editionKeyVisitorId);

            var editionKeyVisitor = query.SingleOrDefault();
            if (editionKeyVisitor != null)
            {
                var editionKeyVisitorModel = Mapper.Map<EditionKeyVisitor, EditionKeyVisitorEntity>(editionKeyVisitor);
                return editionKeyVisitorModel;
            }
            return null;
        }

        public bool EditionKeyVisitorExists(int editionId, int keyVisitorId)
        {
            return _unitOfWork.EditionKeyVisitorRepository.Exists(
                x => x.EditionId == editionId && x.KeyVisitorId == keyVisitorId);
        }

        public IList<EditionKeyVisitorEntity> GetEditionKeyVisitors(int editionId)
        {
            var editionKeyVisitors = _unitOfWork.EditionKeyVisitorRepository.GetManyQueryable(x => x.EditionId == editionId).ToList();

            if (editionKeyVisitors.Any())
            {
                var editionKeyVisitorModels = Mapper.Map<IList<EditionKeyVisitor>, IList<EditionKeyVisitorEntity>>(editionKeyVisitors);
                return editionKeyVisitorModels;
            }
            return new List<EditionKeyVisitorEntity>();
        }

        public int CreateEditionKeyVisitor(EditionKeyVisitorEntity editionKeyVisitorEntity, int userId)
        {
            var editionKeyVisitor = Mapper.Map<EditionKeyVisitorEntity, EditionKeyVisitor>(editionKeyVisitorEntity);
            editionKeyVisitor.CreatedOn = DateTime.Now;
            editionKeyVisitor.CreatedBy = userId;

            _unitOfWork.EditionKeyVisitorRepository.Insert(editionKeyVisitor);
            _unitOfWork.Save();
            return editionKeyVisitor.EditionKeyVisitorId;
        }

        public bool DeleteEditionKeyVisitor(int editionKeyVisitorId)
        {
            if (editionKeyVisitorId > 0)
            {
                var exists = _unitOfWork.EditionKeyVisitorRepository.Exists(editionKeyVisitorId);
                if (exists)
                {
                    _unitOfWork.EditionKeyVisitorRepository.Delete(editionKeyVisitorId);
                    _unitOfWork.Save();

                    return true;
                }
                return false;
            }
            return false;
        }

        public bool DeleteAllEditionKeyVisitors(int editionId)
        {
            if (editionId > 0)
            {
                var editionKeyVisitors = GetEditionKeyVisitors(editionId);

                using (var scope = new TransactionScope())
                {
                    if (editionKeyVisitors != null)
                    {
                        foreach (var editionKeyVisitor in editionKeyVisitors)
                        {
                            _unitOfWork.EditionKeyVisitorRepository.Delete(editionKeyVisitor.EditionKeyVisitorId);
                            _unitOfWork.Save();
                        }
                    }
                    scope.Complete();
                    return true;
                }
            }
            return false;
        }
    }
}
