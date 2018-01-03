using AutoMapper;
using CoralTime.BL.ServicesInterfaces;
using CoralTime.DAL.Repositories;
using CoralTime.ViewModels.PicturesCacheGuid;

namespace CoralTime.BL.Services
{
    public class PicturesCacheGuidService : _BaseService, IPicturesCacheGuid
    {
        public PicturesCacheGuidService(UnitOfWork uow, IMapper mapper)
            : base(uow, mapper) { }

        public PicturesCacheGuidView GetPicturesCacheGuid()
        {
            var picturesCacheGuidView = new PicturesCacheGuidView
            {
                PicturesCacheGuid = MemberAvatarRepository.PicturesCacheGuid
            };

            return picturesCacheGuidView;
        }
    }
}
