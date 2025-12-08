using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CalendarApp.Application.Abstraction;
using CalendarApp.Domain.Entities;

namespace CalendarApp.Web.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    [Authorize(Roles = "Admin")]
    public class EventController : Controller
    {
        IEventAppService _eventAppService;
        public EventController(IEventAppService eventAppService)
        {
            _eventAppService = eventAppService;
        }

        public IActionResult Select()
        {
            var events = _eventAppService.SelectAll();
            return View(events);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Event @event)
        {
            if (ModelState.IsValid)
            {
                _eventAppService.Create(@event);
                return RedirectToAction(nameof(Select));
            }
            return View(@event);
        }

        public IActionResult Delete(int id)
        {
            bool deleted = _eventAppService.Delete(id);
            if (deleted)
            {
                return RedirectToAction(nameof(Select));
            }
            return NotFound();
        }
    }
}