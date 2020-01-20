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
    public class EditionPaymentScheduleServices : IEditionPaymentScheduleServices
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly GenericRepository<EditionPaymentSchedule> _repository;

        public EditionPaymentScheduleServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = (UnitOfWork)unitOfWork;
            _repository = _unitOfWork.EditionPaymentScheduleRepository;
        }

        public EditionPaymentScheduleEntity GetById(int id)
        {
            var editionPaymentSchedule = _repository.GetById(id);
            if (editionPaymentSchedule != null)
            {
                var entity = Mapper.Map<EditionPaymentSchedule, EditionPaymentScheduleEntity>(editionPaymentSchedule);
                return entity;
            }
            return null;
        }

        public EditionPaymentScheduleEntity Get(int editionId, string name)
        {
            var editionPaymentSchedules = _repository.GetManyQueryable(x =>
                x.EditionId == editionId && x.Name.ToLower() == name)
                .ToList();

            if (editionPaymentSchedules.Count != 1)
                return null;

            var obj = editionPaymentSchedules.First();
            
            var entity = Mapper.Map<EditionPaymentSchedule, EditionPaymentScheduleEntity>(obj);
            return entity;
        }

        public IList<EditionPaymentScheduleEntity> GetByEdition(int id)
        {
            var query = _repository.GetManyQueryable(x => x.EditionId == id);
            var editionPaymentSchedules = query.ToList();
            if (editionPaymentSchedules.Any())
            {
                var entity = Mapper.Map<List<EditionPaymentSchedule>, List<EditionPaymentScheduleEntity>>(editionPaymentSchedules);
                return entity;
            }
            return new List<EditionPaymentScheduleEntity>();
        }

        public int Create(EditionPaymentScheduleEntity entity, int userId)
        {
            try
            {
                var editionPaymentSchedule = new EditionPaymentSchedule
                {
                    EditionId = entity.EditionId,
                    Name = entity.Name,
                    ActivationDate = entity.ActivationDate,
                    ExpiryDate = entity.ExpiryDate,
                    Installment1Percentage = entity.Installment1Percentage,
                    Installment1DueDate = entity.Installment1DueDate,
                    Installment2Percentage = entity.Installment2Percentage,
                    Installment2DueDate = entity.Installment2DueDate,
                    Installment3Percentage = entity.Installment3Percentage,
                    Installment3DueDate = entity.Installment3DueDate,
                    Installment4Percentage = entity.Installment4Percentage,
                    Installment4DueDate = entity.Installment4DueDate,
                    Installment5Percentage = entity.Installment5Percentage,
                    Installment5DueDate = entity.Installment5DueDate,
                    CreatedOn = DateTime.Now,
                    CreatedBy = userId
                };

                _unitOfWork.EditionPaymentScheduleRepository.Insert(editionPaymentSchedule);
                _unitOfWork.Save();

                return editionPaymentSchedule.EditionPaymentScheduleId;
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
                    var editionPaymentSchedule = _repository.GetById(id);
                    if (editionPaymentSchedule != null)
                    {
                        _repository.Delete(editionPaymentSchedule);
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
