using SchoolScheduleLibrary.Model.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolScheduleLibrary.Model
{
    public class Classroom : IBaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string ClassName { get; set; }
        public int StartYear { get; set; }
        public int EndYear { get; set; }
        public List<Student> Students { get; set; } = new();
    }
}
