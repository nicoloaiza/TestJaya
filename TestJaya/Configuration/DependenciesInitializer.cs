using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using AutoMapper;
using log4net;
using log4net.Core;
using Microsoft.Extensions.Logging;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using TestJaya.Business.Services;
using TestJaya.Business.Services.Data;
using TestJaya.Business.Util;
using TestJaya.Data.DAL;
using TestJaya.Data.DTO;
using TestJaya.Data.Models;
using TestJaya.Data.Repositories;
using TestJaya.MiddleWare;
using ILogger = log4net.Core.ILogger;

namespace TestJaya.Configuration
{
    /// <summary>
    /// Contains methods to initialize the DI container.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class DependenciesInitializer
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(DependenciesInitializer));

        /// <summary>
        /// Configures the Dependency Injection container for this application, registering implementations for interfaces.
        /// </summary>
        /// <returns>DI Container that must be used to initialize the Web API dependency resolver.</returns>
        public static Container Init()
        {
            var container = new Container();
            try
            {
                container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

                var assemblies = getImplAssemblies();

                container.Register<IMapper>(() =>
                {
                    var config = new MapperConfiguration(cfg => EntityDtoMapper.InitMaps(cfg));
                    return config.CreateMapper();
                }, Lifestyle.Singleton);

                container.Register<TestJaya.Data.DAL.DbContext>(Lifestyle.Scoped);


                // This next call is not required if you are already calling AutoCrossWireAspNetComponents

                container.Register<IBaseDataRepository<User, Guid>>(() => new UserRepository(), Lifestyle.Scoped);
                container.Register<IBaseDataRepository<Room, Guid>>(() => new RoomRepository(), Lifestyle.Scoped);
                container.Register<IBaseDataRepository<ChatMessage, int>>(() => new ChatMessageRepository(), Lifestyle.Scoped);
                container.Register<IBaseDataRepository<UserRoom, int>>(() => new UserRoomRepository(), Lifestyle.Scoped);

                container.Register<IStoredDataService<User, UserDto, Guid>, BaseStoredDataService<User, UserDto, Guid>>(Lifestyle.Scoped);
                container.Register<IStoredDataService<Room, RoomDto, Guid>, BaseStoredDataService<Room, RoomDto, Guid>>(Lifestyle.Scoped);
                container.Register<IStoredDataService<ChatMessage, ChatMessageDto, int>, BaseStoredDataService<ChatMessage, ChatMessageDto, int>>(Lifestyle.Scoped);
                container.Register<IStoredDataService<UserRoom, UserRoomDto, int>, BaseStoredDataService<UserRoom, UserRoomDto, int>>(Lifestyle.Scoped);


                //container.Verify();
            }
            catch (Exception ex)
            {
                logger.Fatal("DI Container initialization failed.", ex);
                System.Diagnostics.Debugger.Break();
                throw ex;
            }
            return container;
        }

        private static IEnumerable<Assembly> getImplAssemblies()
        {
            var result = new List<Assembly>();
            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in allAssemblies)
            {
                if (assembly.FullName.Contains("WebApi")) result.Add(assembly);
            }
            return result;
        }
    }
}
