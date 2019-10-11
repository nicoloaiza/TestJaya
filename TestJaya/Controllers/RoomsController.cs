using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TestJaya.Business.Services;
using TestJaya.Data.DTO;
using TestJaya.Data.Models;
using TestJaya.Business.Util;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TestJaya.Controllers
{
    public class RoomsController : Controller
    {

        private readonly IStoredDataService<Room, RoomDto, Guid> roomService;

        public RoomsController(IStoredDataService<Room, RoomDto, Guid> service)
        {
            roomService = service;
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<RoomDto> rooms = roomService.RetrieveAll().Data.ToList();
            byte[] userNameArray;
            bool userNameAvailable = HttpContext.Session.TryGetValue("User", out userNameArray);
            if(userNameAvailable)
            {
                UserDto user = (UserDto)userNameArray.ToObject();
            }
            return View("Index", rooms);
        }

        [HttpPost]
        public IActionResult Index(string room)
        {
            return View("Rooms", room);
        }

        [HttpGet]
        [Route("Rooms/New")]
        public IActionResult New()
        {
            return View("NewRoom");
        }

        [HttpPost]
        [Route("Rooms/New")]
        public IActionResult New(Room room)
        {
            if (ModelState.IsValid)
            {
                RoomDto exixtingRoom = roomService.RetrieveAll().Data.Where(x => x.Name == room.Name).FirstOrDefault();
                if(exixtingRoom == null)
                {
                    RoomDto newRoom = new RoomDto()
                    {
                        Id = Guid.NewGuid(),
                        Name = room.Name,
                        Description = room.Description
                    };
                    roomService.Create(newRoom);
                }
                return Redirect("/Rooms/Index");
            }
            return View("NewRoom");
        }

    }
}
