using System;
using System.Collections.Generic;
using System.Text;

namespace Scheduler.Model.Entities
{
    public class UserLight : IEntityBase
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public string Profession { get; set; }
        public string Department { get; set; }
        int IEntityBase.Id { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
