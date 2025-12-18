using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CalendarApp.Application.Abstraction;
using CalendarApp.Domain.Entities;

namespace CalendarApp.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
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

        public IActionResult Details(int id)
        {
            var @event = _eventAppService.GetById(id);
            if (@event == null)
            {
                return NotFound();
            }
            return View(@event);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var @event = _eventAppService.GetById(id);
            if (@event == null)
            {
                return NotFound();
            }
            
            return View(@event);
        }

        [HttpPost]
        public IActionResult Edit(Event @event)
        {
            // Remove Owner validation error since we're using OwnerId
            ModelState.Remove("Owner");
            
            if (ModelState.IsValid)
            {
                try
                {
                    _eventAppService.Update(@event);
                    TempData["Success"] = "Event updated successfully!";
                    return RedirectToAction(nameof(Select));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error updating event: " + ex.Message);
                }
            }
            return View(@event);
        }

        [HttpPost]
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