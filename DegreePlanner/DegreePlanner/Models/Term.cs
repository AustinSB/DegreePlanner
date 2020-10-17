using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using SQLite;

namespace DegreePlanner.Models
{
    public class Term
    {
        [PrimaryKey, AutoIncrement]
        [Column("id")]
        public int Id { get; set; }

        [MaxLength(100)]
        [Column("title")]
        public string Title { get; set; }
        [Column("start")]
        public DateTime Start { get; set; }
        [Column("end")]
        public DateTime End { get; set; }
        [Column("start_end")]
        public string StartEnd { get { return $"Start: {Start.ToShortDateString()}   |   End: {End.ToShortDateString()}"; } }

    }
    
    public class Course
    {
        [PrimaryKey, AutoIncrement]
        [Column("id")]
        public int Id { get; set; }
        [Indexed]
        [Column("term_id")]
        public int TermId { get; set; }

        [MaxLength(100)]
        [Column("title")]
        public string Title { get; set; }
        [Column("start")]
        public DateTime Start { get; set; }
        [Column("end")]
        public DateTime End { get; set; }
        [Column("start_end")]
        public string StartEnd { get { return $"Start: {Start.ToShortDateString()}   |   End: {End.ToShortDateString()}"; } }
        [Column("status")]
        public string Status { get; set; }
        
        [Column("instructor_name")]
        public string InstructorName { get; set; }
        [Column("instructor_phone")]
        public string InstructorPhone { get; set; }
        [Column("instructor_email")]
        public string InstructorEmail { get; set; }

        //notifications
        [Column("start_alert_set")]
        public bool StartAlertSet { get; set; }
        [Column("end_alert_set")]
        public bool EndAlertSet { get; set; }
    }

    public class Note
    {
        [PrimaryKey, AutoIncrement]
        [Column("id")]
        public int Id { get; set; }
        [Indexed]
        [Column("course_id")]
        public int CourseId { get; set; }

        [MaxLength(500)]
        [Column("content")]
        public string Content { get; set; }
    }

    public class Assessment
    {
        [PrimaryKey, AutoIncrement]
        [Column("id")]
        public int Id { get; set; }
        [Indexed]
        [Column("course_id")]
        public int CourseId { get; set; }

        [MaxLength(100)]
        [Column("name")]
        public string Name { get; set; }
        [Column("type")]
        public string AssessType { get; set; }

        [Column("start")]
        public DateTime Start { get; set; }
        [Column("end")]
        public DateTime End { get; set; }
        [Column("start_end")]
        public string StartEnd { get { return $"Start: {Start.ToShortDateString()}   |   End: {End.ToShortDateString()}"; } }

        //notifications
        [Column("start_alert_set")]
        public bool StartAlertSet { get; set; }
        [Column("end_alert_set")]
        public bool EndAlertSet { get; set; }
    }
}
