using System;
using System.Collections.Generic;

namespace StudentSystem.Data.Models
{
    public class Student
    {
        public int StudentId { get; set; }
        public string Name { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime RegisteredOn { get; set; }
        public DateTime? Birthday { get; set; }

        public virtual ICollection<Homework> Homeworks { get; set; } = new List<Homework>();
        public virtual ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();
    }
}
