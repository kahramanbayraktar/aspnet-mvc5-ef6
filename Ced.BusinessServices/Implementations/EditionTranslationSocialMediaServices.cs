using AutoMapper;
using Ced.BusinessEntities;
using Ced.Data.Models;
using Ced.Data.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ced.BusinessServices
{
    public class EditionTranslationSocialMediaServices : IEditionTranslationSocialMediaServices
    {
        private readonly UnitOfWork _unitOfWork;

        public EditionTranslationSocialMediaServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = (UnitOfWork)unitOfWork;
        }

        public EditionTranslationSocialMediaEntity GetById(int id)
        {
            var obj = _unitOfWork.EditionTranslationSocialMediaRepository.GetById(id);
            if (obj != null)
            {
                var entity = Mapper.Map<EditionTranslationSocialMedia, EditionTranslationSocialMediaEntity>(obj);
                return entity;
            }
            return null;
        }

        public EditionTranslationSocialMediaEntity Get(int editionTranslationId, string socialMediaId)
        {
            var objs = _unitOfWork.EditionTranslationSocialMediaRepository.GetManyQueryable(x =>
                x.EditionTranslationId == editionTranslationId && x.SocialMediaId.ToLower() == socialMediaId)
                .ToList();

            if (objs.Count != 1)
                return null;

            var obj = objs.First();
            
            var entity = Mapper.Map<EditionTranslationSocialMedia, EditionTranslationSocialMediaEntity>(obj);
            return entity;
        }

        public IList<EditionTranslationSocialMediaEntity> GetByEdition(int id, string languageCode = null)
        {
            var query = _unitOfWork.EditionTranslationSocialMediaRepository.GetManyQueryable(x => x.EditionId == id);
            if (!string.IsNullOrWhiteSpace(languageCode))
                query = query.Where(x => x.EditionTranslation.LanguageCode == languageCode);
            var socialMedias = query.ToList();
            if (socialMedias.Any())
            {
                var entity = Mapper.Map<List<EditionTranslationSocialMedia>, List<EditionTranslationSocialMediaEntity>>(socialMedias);
                return entity;
            }
            return new List<EditionTranslationSocialMediaEntity>();
        }

        public int Create(EditionTranslationSocialMediaEntity entity, int userId)
        {
            try
            {
                var socialMedia = new EditionTranslationSocialMedia
                {
                    EditionTranslationId = entity.EditionTranslationId,
                    SocialMediaId = entity.SocialMediaId,
                    EditionId = entity.EditionId,
                    AccountName = entity.AccountName,
                    CreatedOn = DateTime.Now,
                    CreatedBy = userId
                };

                _unitOfWork.EditionTranslationSocialMediaRepository.Insert(socialMedia);
                _unitOfWork.Save();

                return socialMedia.EditionTranslationSocialMediaId;
            }
            catch (Exception exc)
            {
                return -1;
            }
        }

        public bool Update(int id, EditionTranslationSocialMediaEntity entity, int userId)
        {
            var success = false;
            if (entity != null)
            {
                var socialMedia = _unitOfWork.EditionTranslationSocialMediaRepository.GetById(id);
                if (socialMedia != null)
                {
                    socialMedia.AccountName = entity.AccountName;
                    socialMedia.SocialMediaId = entity.SocialMediaId;
                    socialMedia.UpdatedOn = DateTime.Now;
                    socialMedia.UpdatedBy = userId;

                    _unitOfWork.EditionTranslationSocialMediaRepository.Update(socialMedia);
                    _unitOfWork.Save();
                    success = true;
                }
            }
            return success;
        }

        public bool Delete(int id)
        {
            var success = false;
            if (id > 0)
            {
                try
                {
                    var obj = _unitOfWork.EditionTranslationSocialMediaRepository.GetById(id);
                    if (obj != null)
                    {
                        _unitOfWork.EditionTranslationSocialMediaRepository.Delete(obj);
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
