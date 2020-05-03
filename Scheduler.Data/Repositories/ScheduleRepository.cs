using Scheduler.Data.Abstract;
using Scheduler.Model;

namespace Scheduler.Data.Repositories
{
    public class ScheduleRepository : EntityBaseRepository<Schedule>, IScheduleRepository
    {
        public ScheduleRepository(SchedulerContext context)
            : base(context)
        { }
    }
}
