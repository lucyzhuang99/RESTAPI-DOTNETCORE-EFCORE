using Scheduler.Data.Abstract;
using Scheduler.Model;

namespace Scheduler.Data.Repositories
{
    public class AttendeeRepository : EntityBaseRepository<Attendee>, IAttendeeRepository
    {
        public AttendeeRepository(SchedulerContext context)
            : base(context)
        { }
    }
}
