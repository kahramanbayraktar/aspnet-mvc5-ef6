using AutoMapper;
using Ced.BusinessEntities;
using Ced.Data.Models;
using Ced.Data.UnitOfWork;
using ITE.Utility.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace Ced.BusinessServices
{
    public class SocialMediaServices : ISocialMediaServices
    {
        private readonly UnitOfWork _unitOfWork;

        public SocialMediaServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = (UnitOfWork)unitOfWork;
        }

        public SocialMediaEntity GetSocialMediaById(string socialMediaId)
        {
            socialMediaId = socialMediaId.Trim().ToLower();
            var query = _unitOfWork.SocialMediaRepository.GetManyQueryable(x => x.SocialMediaId.ToLower() == socialMediaId);

            var socialMedia = query.SingleOrDefault();
            if (socialMedia != null)
            {
                var entity = Mapper.Map<SocialMedia, SocialMediaEntity>(socialMedia);
                return entity;
            }
            return null;
        }

        public IList<SocialMediaEntity> GetAllSocialMedias()
        {
            var objs = _unitOfWork.SocialMediaRepository.GetAll().ToList();
            if (objs.Any())
            {
                var entities = Mapper.Map<List<SocialMedia>, List<SocialMediaEntity>>(objs);
                return entities;
            }
            return new List<SocialMediaEntity>();
        }

        public string CreateSocialMedia(string socialMediaId)
        {
            var socialMedia = new SocialMedia
            {
                SocialMediaId = socialMediaId.RemoveDuplicateChars(" ").ToLower(),
                Name = socialMediaId
            };

            _unitOfWork.SocialMediaRepository.Insert(socialMedia);
            _unitOfWork.Save();

            return socialMedia.SocialMediaId;
        }
    }
}
