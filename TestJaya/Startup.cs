using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using log4net.Repository.Hierarchy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimpleInjector;
using SimpleInjector.Integration.AspNetCore.Mvc;
using TestJaya.Business.Services;
using TestJaya.Business.Services.Data;
using TestJaya.Configuration;
using TestJaya.Data.DAL;
using TestJaya.Data.DTO;
using TestJaya.Data.Models;
using TestJaya.Data.Repositories;
using TestJaya.MiddleWare;

namespace TestJaya
{
    public class Startup
    {
        private Container container = new Container();
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            RegisterContainer(services);

            services.AddSession(options =>
            {
                options.Cookie.Name = ".TestJaya.Session";
                options.IdleTimeout = TimeSpan.FromSeconds(10000);
                options.Cookie.IsEssential = true;
            });
        }

        private void RegisterContainer(IServiceCollection services)
        {
            container = DependenciesInitializer.Init();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<IControllerActivator>(
                new SimpleInjectorControllerActivator(container));
            services.AddSingleton<IViewComponentActivator>(
                new SimpleInjectorViewComponentActivator(container));
            services.EnableSimpleInjectorCrossWiring(container);

            services.UseSimpleInjectorAspNetRequestScoping(container);


            services.AddSimpleInjector(container, options =>
            {
                // AddAspNetCore() wraps web requests in a Simple Injector scope.
                options.AddAspNetCore()
                    // Ensure activation of a specific framework type to be created by
                    // Simple Injector instead of the built-in configuration system.
                    .AddControllerActivation()
                    .AddViewComponentActivation()
                    .AddPageModelActivation()
                    .AddTagHelperActivation();
            });
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            //container.CrossWire<ILoggerFactory>(app);
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseWebSockets();
            app.UseSession();

            app.UseRouting();

            app.UseAuthorization();
            
            //app.UseMiddleware<ChatWebSocketMiddleware>();
            app.Use(async (context, next) => {
                var middleware = new ChatWebSocketMiddleware(
                    request => next(),
                    container.GetInstance<IStoredDataService<ChatMessage, ChatMessageDto, int>>(),
                    container.GetInstance<IStoredDataService<Room, RoomDto, Guid>>(),
                    container.GetInstance<IStoredDataService<UserRoom, UserRoomDto, int>>(),
                    container.GetInstance<IStoredDataService<User, UserDto, Guid>>());

                await middleware.Invoke(context);
            });

            container.Verify();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
