using AutoMapper;
using Ced.BusinessEntities;
using Ced.Data.GenericRepository;
using Ced.Data.Models;
using Ced.Data.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ced.BusinessServices
{
    public class EditionDiscountApproverServices : IEditionDiscountApproverServices
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly GenericRepository<EditionDiscountApprover> _repository;

        public EditionDiscountApproverServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = (UnitOfWork)unitOfWork;
            _repository = _unitOfWork.EditionDiscountApproverRepository;
        }

        public EditionDiscountApproverEntity GetById(int id)
        {
            var discountApprover = _repository.GetById(id);
            if (discountApprover != null)
            {
                var entity = Mapper.Map<EditionDiscountApprover, EditionDiscountApproverEntity>(discountApprover);
                return entity;
            }
            return null;
        }

        public EditionDiscountApproverEntity Get(int editionId, string approvingUser)
        {
            var discountApprovers = _repository.GetManyQueryable(x =>
                x.EditionId == editionId && x.ApprovingUser.ToLower() == approvingUser)
                .ToList();

            if (discountApprovers.Count != 1)
                return null;

            var discountApprover = discountApprovers.First();
            
            var entity = Mapper.Map<EditionDiscountApprover, EditionDiscountApproverEntity>(discountApprover);
            return entity;
        }

        public IList<EditionDiscountApproverEntity> GetByEdition(int id)
        {
            var query = _repository.GetManyQueryable(x => x.EditionId == id);
            var discountApprovers = query.ToList();
            if (discountApprovers.Any())
            {
                var entity = Mapper.Map<List<EditionDiscountApprover>, List<EditionDiscountApproverEntity>>(discountApprovers);
                return entity;
            }
            return new List<EditionDiscountApproverEntity>();
        }

        public int Create(EditionDiscountApproverEntity entity, int userId)
        {
            try
            {
                var discountApprover = new EditionDiscountApprover
                {
                    EditionId = entity.EditionId,
                    ApprovingUser = entity.ApprovingUser,
                    ApprovalLowerPercentage = entity.ApprovalLowerPercentage,
                    ApprovalUpperPercentage = entity.ApprovalUpperPercentage,
                    CreatedOn = DateTime.Now,
                    CreatedBy = userId
                };

                _unitOfWork.EditionDiscountApproverRepository.Insert(discountApprover);
                _unitOfWork.Save();

                return discountApprover.EditionDiscountApproverId;
            }
            catch (Exception exc)
            {
                return -1;
            }
        }

        public bool Delete(int id)
        {
            var success = false;
            if (id > 0)
            {
                try
                {
                    var discountApprover = _repository.GetById(id);
                    if (discountApprover != null)
                    {
                        _repository.Delete(discountApprover);
                        _unitOfWork.Save();
                        success = true;
                    }
                }
                catch (Exception exc)
                {
                    
                }
            }
            return success;
        }
    }
}
