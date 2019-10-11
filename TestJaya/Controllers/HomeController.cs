using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TestJaya.Business.Services;
using TestJaya.Data.DTO;
using TestJaya.Data.Models;
using TestJaya.Models;
using TestJaya.Business.Util;

namespace TestJaya.Controllers
{
    public class HomeController : Controller
    {
        private readonly IStoredDataService<User, UserDto, Guid> userService;

        public HomeController(IStoredDataService<User, UserDto, Guid> service)
        {
            userService = service;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View("InsertUserName");
        }

        [HttpPost]
        public IActionResult Index(string userName)
        {
            UserDto user = this.userService.RetrieveAll(x => x.Name == userName).Data.FirstOrDefault();
            if(user == null)
            {
                user = new UserDto()
                {
                    Name = userName,
                    Id = Guid.NewGuid()
                };
                user = userService.Create(user);
            }
            HttpContext.Session.Set("User", user.ToByteArray());
            return Redirect("/Rooms/Index");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
