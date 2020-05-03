using Scheduler.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Scheduler.Model
{
    public class UserSchedule: IEntityBase
    {
        public long Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public string Profession { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime TimeStart { get; set; }
        public DateTime TimeEnd { get; set; }
        public string Location { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        int IEntityBase.Id { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
