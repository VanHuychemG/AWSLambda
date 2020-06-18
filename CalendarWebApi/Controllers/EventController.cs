using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace CalendarWebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/events")]
    public class EventsController : Controller
    {
        private static List<CalendarEvent> _events;

        public EventsController()
        {

            if (_events == null)
            {
                var calEvent = new CalendarEvent()
                {
                    id = Guid.NewGuid().ToString(),
                    title = "Test Event Title",
                    allDay = false,
                    start = DateTime.Now,
                    end = DateTime.Now.AddHours(2)
                };
                _events = new List<CalendarEvent>(new[] { calEvent });
            }
        }

        // GET api/events
        [HttpGet]
        [Authorize(Policy = "InCalendarWriterGroup")]
        public IEnumerable<CalendarEvent> Get(DateTime? start, DateTime? end)
        {
            start = start ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            end = end ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
            return _events.FindAll(x => x.end >= start && x.start <= end);
        }

        // POST api/events

        [HttpPost]
        [Authorize(Policy = "InCalendarWriterGroup")]
        public void Post([FromForm] CalendarEvent calEvent)
        {
            calEvent.id = Guid.NewGuid().ToString();
            _events.Add(calEvent);
        }

        // PUT api/events/5
        [HttpPut("{id}")]
        [Authorize(Policy = "InCalendarWriterGroup")]
        public void Put(string id, [FromForm] CalendarEvent calEvent)
        {
            var index = _events.FindIndex(x => x.id == id);
            _events[index] = calEvent;
        }
    }
}
