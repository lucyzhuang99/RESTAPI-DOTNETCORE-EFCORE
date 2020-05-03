using Scheduler.Data.Abstract;
using Scheduler.Model;

namespace Scheduler.Data.Repositories
{
    public class UserRepository : EntityBaseRepository<User>, IUserRepository
    {
        public UserRepository(SchedulerContext context)
            : base(context)
        { }
    }
}
