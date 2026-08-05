using SchoolScheduleLibrary.Model.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.Model
{
    public class Institution : IBaseEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        DateTime CreatedAt { get; set; }
    }
}
