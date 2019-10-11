using System;
using AutoMapper;
using log4net;
using TestJaya.Data.DTO;
using TestJaya.Data.Models;

namespace TestJaya.Business.Util
{
    /// <summary>
    /// Contains methods to map from DTO object to Entity object and vice versa.
    /// </summary>
    public static class EntityDtoMapper
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(EntityDtoMapper));

        public static void InitMaps(IMapperConfigurationExpression cfg)
        {
            #region Entities to DTOs
            cfg.CreateMap<User, UserDto>().ForMember(x => x.UserRooms, y => y.Ignore());
            cfg.CreateMap<Room, RoomDto>();
            cfg.CreateMap<UserRoom, UserRoomDto>();
            cfg.CreateMap<ChatMessage, ChatMessageDto>().ForMember(x => x.User, y => y.Ignore()).ForMember(x => x.Room, y => y.Ignore());

            #endregion

            #region DTOs to Entities
            cfg.CreateMap<UserDto, User>().ForMember(x => x.UserRooms, y => y.Ignore());
            cfg.CreateMap<RoomDto, Room>();
            cfg.CreateMap<UserRoomDto, UserRoom>();
            cfg.CreateMap<ChatMessageDto, ChatMessage>();
            #endregion

        }
    }
}
