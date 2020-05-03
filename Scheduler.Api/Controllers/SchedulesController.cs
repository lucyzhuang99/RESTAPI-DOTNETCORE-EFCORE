using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scheduler.Api.Core;
//using Scheduler.Api.ViewModels;
//using Scheduler.Api.ViewModels.Mappings;
using Scheduler.API.ViewModels;
using Scheduler.Data;
using Scheduler.Data.Abstract;
using Scheduler.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Scheduler.Api.Controllers
{
    [Route("api/[controller]")]
    public class SchedulesController : Controller
    {
        private IScheduleRepository _scheduleRepository;
        private IAttendeeRepository _attendeeRepository;
        private IUserRepository _userRepository;

        int page = 1;
        int pageSize = 4;

        private readonly IMapper _mapper;
        private SchedulerContext _context;

        // Assign the object in the constructor for dependency injection
        public SchedulesController(IScheduleRepository scheduleRepository,
                                    IAttendeeRepository attendeeRepository,
                                    IUserRepository userRepository,
                                    IMapper mapper,
                                    SchedulerContext context)
        {
            _scheduleRepository = scheduleRepository;
            _attendeeRepository = attendeeRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _context = context;
        }

        public IActionResult Get()
        {
            var pagination = Request.Headers["Pagination"];

            if (!string.IsNullOrEmpty(pagination))
            {
                string[] vals = pagination.ToString().Split(',');
                int.TryParse(vals[0], out page);
                int.TryParse(vals[1], out pageSize);
            }

            int currentPage = page;
            int currentPageSize = pageSize;
            var totalSchedules = _scheduleRepository.Count();
            var totalPages = (int)Math.Ceiling((double)totalSchedules / pageSize);

            IEnumerable<Schedule> _schedules = _scheduleRepository
                .AllIncluding(s => s.Creator, s => s.Attendees)
                .OrderBy(s => s.Id)
                .Skip((currentPage - 1) * currentPageSize)
                .Take(currentPageSize)
                .ToList();

            Response.AddPagination(page, pageSize, totalSchedules, totalPages);

            IEnumerable<ScheduleViewModel> _schedulesVM = _mapper.Map<IEnumerable<Schedule>, IEnumerable<ScheduleViewModel>>(_schedules);

            return new OkObjectResult(_schedulesVM);
        }

        [HttpGet("{id}", Name = "GetSchedule")]
        public IActionResult Get(int id)
        {
            Schedule _schedule = _scheduleRepository
                .GetSingle(s => s.Id == id, s => s.Creator, s => s.Attendees);

            if (_schedule != null)
            {
                ScheduleViewModel _scheduleVM = _mapper.Map<Schedule, ScheduleViewModel>(_schedule);
                return new OkObjectResult(_scheduleVM);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("businessschedules", Name = "GetBusinessSchedules")]
        public IActionResult GetBusinessSchedules()
        {
            if (_context.BusinessSchedules != null)
            {
                var bussinessSchedules = _context.BusinessSchedules.FromSqlRaw("sp_GetGroupSchedule").ToList();

                if (bussinessSchedules.Any())
                {
                    return new OkObjectResult(bussinessSchedules);
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("workschedules", Name = "GetWorkSchedules")]
        public IActionResult GetWorkSchedules()
        {
            if (_context.ScheduleLights != null)
            {
                var scheduleLights = _context.ScheduleLights.FromSqlRaw("sp_GetWorkSchedule").ToList();

                if (scheduleLights.Any())
                {
                    return new OkObjectResult(scheduleLights);
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("{id}/details", Name = "GetScheduleDetails")]
        public IActionResult GetScheduleDetails(int id)
        {
            Schedule _schedule = _scheduleRepository
                .GetSingle(s => s.Id == id, s => s.Creator, s => s.Attendees);

            if (_schedule != null)
            {
                ScheduleDetailsViewModel _scheduleDetailsVM = _mapper.Map<Schedule, ScheduleDetailsViewModel>(_schedule);

                foreach (var attendee in _schedule.Attendees)
                {
                    User _userDb = _userRepository.GetSingle(attendee.UserId);
                    _scheduleDetailsVM.Attendees.Add(_mapper.Map<User, UserViewModel>(_userDb));
                }

                return new OkObjectResult(_scheduleDetailsVM);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody]ScheduleViewModel schedule)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Schedule _newSchedule = _mapper.Map<ScheduleViewModel, Schedule>(schedule);
            _newSchedule.DateCreated = DateTime.Now;

            _scheduleRepository.Add(_newSchedule);
            _scheduleRepository.Commit();

            foreach (var userId in schedule.Attendees)
            {
                _newSchedule.Attendees.Add(new Attendee { UserId = userId });
            }
            _scheduleRepository.Commit();

            schedule = _mapper.Map<Schedule, ScheduleViewModel>(_newSchedule);

            CreatedAtRouteResult result = CreatedAtRoute("GetSchedule", new { controller = "Schedules", id = schedule.Id }, schedule);
            return result;
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]ScheduleViewModel schedule)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Schedule _scheduleDb = _scheduleRepository.GetSingle(id);

            if (_scheduleDb == null)
            {
                return NotFound();
            }
            else
            {
                _scheduleDb.Title = schedule.Title;
                _scheduleDb.Location = schedule.Location;
                _scheduleDb.Description = schedule.Description;
                _scheduleDb.Status = (ScheduleStatus)Enum.Parse(typeof(ScheduleStatus), schedule.Status);
                _scheduleDb.Type = (ScheduleType)Enum.Parse(typeof(ScheduleType), schedule.Type);
                _scheduleDb.TimeStart = schedule.TimeStart;
                _scheduleDb.TimeEnd = schedule.TimeEnd;

                // Remove current attendees
                _attendeeRepository.DeleteWhere(a => a.ScheduleId == id);

                foreach (var userId in schedule.Attendees)
                {
                    _scheduleDb.Attendees.Add(new Attendee { ScheduleId = id, UserId = userId });
                }

                _scheduleRepository.Commit();
            }

            schedule = _mapper.Map<Schedule, ScheduleViewModel>(_scheduleDb);

            return new NoContentResult();
        }

        [HttpDelete("{id}", Name = "RemoveSchedule")]
        public IActionResult Delete(int id)
        {
            Schedule _scheduleDb = _scheduleRepository.GetSingle(id);

            if (_scheduleDb == null)
            {
                return new NotFoundResult();
            }
            else
            {
                _attendeeRepository.DeleteWhere(a => a.ScheduleId == id);
                _scheduleRepository.Delete(_scheduleDb);

                _scheduleRepository.Commit();

                return new NoContentResult();
            }
        }

        [HttpDelete("{id}/removeattendee/{attendee}")]
        public IActionResult Delete(int id, int attendee)
        {
            Schedule _scheduleDb = _scheduleRepository.GetSingle(id);

            if (_scheduleDb == null)
            {
                return new NotFoundResult();
            }
            else
            {
                _attendeeRepository.DeleteWhere(a => a.ScheduleId == id && a.UserId == attendee);

                _attendeeRepository.Commit();

                return new NoContentResult();
            }
        }
    }
}
