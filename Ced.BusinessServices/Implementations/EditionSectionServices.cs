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
    public class EditionSectionServices : IEditionSectionServices
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly GenericRepository<EditionSection> _repository;

        public EditionSectionServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = (UnitOfWork)unitOfWork;
            _repository = _unitOfWork.EditionSectionRepository;
        }

        public EditionSectionEntity GetById(int id)
        {
            var obj = _repository.GetById(id);
            if (obj != null)
            {
                var entity = Mapper.Map<EditionSection, EditionSectionEntity>(obj);
                return entity;
            }
            return null;
        }

        public EditionSectionEntity Get(int editionId, string sections)
        {
            var objs = _repository.GetManyQueryable(x =>
                x.EditionId == editionId && x.Sections.ToLower() == sections)
                .ToList();

            if (objs.Count != 1)
                return null;

            var obj = objs.First();
            
            var entity = Mapper.Map<EditionSection, EditionSectionEntity>(obj);
            return entity;
        }

        public IList<EditionSectionEntity> GetByEdition(int id)
        {
            var query = _repository.GetManyQueryable(x => x.EditionId == id);
            var editionSections = query.ToList();
            if (editionSections.Any())
            {
                var entity = Mapper.Map<List<EditionSection>, List<EditionSectionEntity>>(editionSections);
                return entity;
            }
            return new List<EditionSectionEntity>();
        }

        public int Create(EditionSectionEntity entity, int userId)
        {
            try
            {
                var editionSection = new EditionSection
                {
                    EditionId = entity.EditionId,
                    Sections = entity.Sections,
                    CreatedOn = DateTime.Now,
                    CreatedBy = userId
                };

                _unitOfWork.EditionSectionRepository.Insert(editionSection);
                _unitOfWork.Save();

                return editionSection.EditionSectionId;
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
                    var obj = _repository.GetById(id);
                    if (obj != null)
                    {
                        _repository.Delete(obj);
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
