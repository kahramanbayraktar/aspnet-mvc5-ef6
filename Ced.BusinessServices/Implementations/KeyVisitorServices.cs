using AutoMapper;
using Ced.BusinessEntities;
using Ced.Data.Models;
using Ced.Data.UnitOfWork;
using System.Collections.Generic;
using System.Linq;

namespace Ced.BusinessServices
{
    public class KeyVisitorServices : IKeyVisitorServices
    {
        private readonly UnitOfWork _unitOfWork;

        public KeyVisitorServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = (UnitOfWork)unitOfWork;
        }

        public KeyVisitorEntity GetKeyVisitorById(int keyVisitorId)
        {
            var query = _unitOfWork.KeyVisitorRepository.GetManyQueryable(x => x.KeyVisitorId == keyVisitorId);

            var keyVisitor = query.SingleOrDefault();
            if (keyVisitor != null)
            {
                var keyVisitorModel = Mapper.Map<KeyVisitor, KeyVisitorEntity>(keyVisitor);
                return keyVisitorModel;
            }
            return null;
        }

        public IList<KeyVisitorEntity> SearchKeyVisitors(string searchTerm, int pageSize, int pageNum)
        {
            var query = _unitOfWork.KeyVisitorRepository.GetManyQueryableProjected<KeyVisitorEntity>(x => x.Name.ToLower().Contains(searchTerm.ToLower()));

            query = query.OrderBy(x => x.Name);
            query = query.Skip(pageSize * (pageNum - 1));
            query = query.Take(pageSize);

            var keyVisitors = query.ToList();
            return keyVisitors;
        }

        public IList<KeyVisitorEntity> GetAllKeyVisitors()
        {
            var keyVisitors = _unitOfWork.KeyVisitorRepository.GetAll().ToList();
            if (keyVisitors.Any())
            {
                var keyVisitorsModel = Mapper.Map<List<KeyVisitor>, List<KeyVisitorEntity>>(keyVisitors);
                return keyVisitorsModel;
            }
            return new List<KeyVisitorEntity>();
        }
    }
}
