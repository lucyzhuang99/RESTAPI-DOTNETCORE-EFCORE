using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Scheduler.Model
{
    public interface IEntityBase
    {
        int Id { get; set; }
    }
}
