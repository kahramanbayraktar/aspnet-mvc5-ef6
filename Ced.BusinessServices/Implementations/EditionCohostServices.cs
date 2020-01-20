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
    public class EditionCohostServices : IEditionCohostServices
    {
        private readonly UnitOfWork _unitOfWork;
        
        public EditionCohostServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = (UnitOfWork)unitOfWork;
        }

        public EditionCohostEntity GetEditionCohostById(int editionCohostId)
        {
            var query = _unitOfWork.EditionCohostRepository.GetManyQueryable(x => x.EditionCohostId == editionCohostId);

            var editionCohost = query.SingleOrDefault();
            if (editionCohost != null)
            {
                var editionCohostModel = Mapper.Map<EditionCohost, EditionCohostEntity>(editionCohost);
                return editionCohostModel;
            }
            return null;
        }

        public bool EditionCohostExists(int firstEditionId, int secondEditionId)
        {
            return _unitOfWork.EditionCohostRepository.Exists(
                x => x.FirstEditionId == firstEditionId && x.SecondEditionId == secondEditionId
                     || x.FirstEditionId == secondEditionId && x.SecondEditionId == firstEditionId);
        }

        public IList<EditionCohostEntity> GetEditionCohosts(int editionId)
        {
            var editionCohosts = _unitOfWork.EditionCohostRepository.GetManyQueryable(x =>
                x.FirstEditionId == editionId || x.SecondEditionId == editionId).ToList();

            if (editionCohosts.Any())
            {
                var editionCohostModels = Mapper.Map<IList<EditionCohost>, IList<EditionCohostEntity>>(editionCohosts);
                return editionCohostModels;
            }
            return new List<EditionCohostEntity>();
        }

        public int CreateEditionCohost(EditionCohostEntity editionCohostEntity, int userId)
        {
            var editionCohost = Mapper.Map<EditionCohostEntity, EditionCohost>(editionCohostEntity);
            editionCohost.CreatedOn = DateTime.Now;
            editionCohost.CreatedBy = userId;

            _unitOfWork.EditionCohostRepository.Insert(editionCohost);
            _unitOfWork.Save();
            return editionCohost.EditionCohostId;
        }

        public bool DeleteEditionCohost(int editionCohostId)
        {
            if (editionCohostId > 0)
            {
                var exists = _unitOfWork.EditionCohostRepository.Exists(editionCohostId);
                if (exists)
                {
                    _unitOfWork.EditionCohostRepository.Delete(editionCohostId);
                    _unitOfWork.Save();

                    return true;
                }
                return false;
            }
            return false;
        }

        public bool DeleteAllEditionCohosts(int editionId)
        {
            if (editionId > 0)
            {
                var cohosts = GetEditionCohosts(editionId);

                using (var scope = new TransactionScope())
                {
                    if (cohosts != null)
                    {
                        foreach (var cohost in cohosts)
                        {
                            _unitOfWork.EditionCohostRepository.Delete(cohost.EditionCohostId);
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
