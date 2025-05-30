using System;
using System.ComponentModel.DataAnnotations;

namespace project.Models
{
    public class Enrollment
    {
        public int Id { get; set; }

        public DateTime EnrollmentDate { get; set; } = DateTime.Now;

        public int StudentId { get; set; }
        public virtual Student Student { get; set; }

        public int CourseId { get; set; }
        public virtual Course Course { get; set; }

        public decimal? Grade { get; set; }
    }
}