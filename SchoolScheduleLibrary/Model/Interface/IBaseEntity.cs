using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.Model.Interface
{
    public interface IBaseEntity
    {
        public Guid Id { get; set; }
    }
}
