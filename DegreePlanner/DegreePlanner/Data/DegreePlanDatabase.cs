using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DegreePlanner.Models;
using SQLite;
using Xamarin.Forms.Xaml;
using System.Linq;
using Xamarin.Essentials;

namespace DegreePlanner.Data
{
    public class DegreePlanDatabase
    {
        readonly SQLiteAsyncConnection connection;

        public DegreePlanDatabase(string dbPath)
        {
            connection = new SQLiteAsyncConnection(dbPath);

            connection.CreateTableAsync<Term>().Wait();
            connection.CreateTableAsync<Course>().Wait();
            connection.CreateTableAsync<Note>().Wait();
            connection.CreateTableAsync<Assessment>().Wait();

            //DeleteSampleData();
            
        }

        //Methods for the Terms Table
        public Task<List<Term>> GetTermsAsync()
        {
            return connection.Table<Term>().ToListAsync();
        }

        public Task<Term> GetTermAsync(int id)
        {
            return connection.Table<Term>().Where(i => i.Id == id).FirstOrDefaultAsync();
        }

        public Task<int> SaveTermAsync(Term term)
        {
            if (term.Id != 0)
            {
                return connection.UpdateAsync(term);
            }
            else
            {
                return connection.InsertAsync(term);
            }
        }

        public Task<int> DeleteTermAsync(Term term)
        {
            return connection.DeleteAsync(term);
        }

        //Methods for the Course Table
        public Task<List<Course>> GetCoursesAsync(Term term)
        {
            var list = connection.Table<Course>().Where(c => c.TermId == term.Id).ToListAsync();
            return list;
        }

        //If there are six courses already, return true
        public async Task<bool> IsCourseCountSix(Term term)
        {
            var list = await connection.Table<Course>().Where(c => c.TermId == term.Id).ToListAsync();

            if (list.Count == 6)
            {
                return true;
            }
            return false;
        }

        public Task<int> SaveCourseAsync(Course course)
        {
            if (course.Id != 0)
            {
                return connection.UpdateAsync(course);
            }
            else
            {
                return connection.InsertAsync(course);
            }
        }

        public Task<int> DeleteCourseAsync(Course course)
        {
            return connection.DeleteAsync(course);
        }

        //Methods for Notes Table
        public Task<List<Note>> GetNotesAsync(Course course)
        {
            var list = connection.Table<Note>().Where(n => n.CourseId == course.Id).ToListAsync();
            return list;
        }

        public Task<int> SaveNoteAsync(Note note)
        {
            if (note.Id != 0)
            {
                return connection.UpdateAsync(note);
            }
            else
            {
                return connection.InsertAsync(note);
            }
        }

        public Task<int> DeleteNoteAsync(Note note)
        {
            return connection.DeleteAsync(note);
        }

        //Share Notes
        public async Task ShareNote(string note, string contact)
        {
            var message = new SmsMessage(note, contact);
            await Sms.ComposeAsync(message);
        }

        //Methods for Assessment Table
        public Task<List<Assessment>> GetAssessmentsAsync(Course course)
        {
            var list = connection.Table<Assessment>().Where(a => a.CourseId == course.Id).ToListAsync();
            return list;
        }

        public Task<int> SaveAssessmentAsync(Assessment assessment)
        {
            if (assessment.Id != 0)
            {
                return connection.UpdateAsync(assessment);
            }
            else
            {
                return connection.InsertAsync(assessment);
            }
        }

        public Task<int> DeleteAssessmentAsync(Assessment assessment)
        {
            return connection.DeleteAsync(assessment);
        }

        //If the assessment was already added to the specified course, return true
        public async Task<bool> IsAssessmentTypeAdded(Course course, string assessType)
        {
            bool exists = false;
            
            var list = await connection.Table<Assessment>().Where(a => a.CourseId == course.Id).ToListAsync();
            list.ForEach(a =>
            {
                if (a.AssessType == assessType) 
                {
                    exists = true;
                }
            });

            if(exists == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //Clear and set sample data for testing purposes
        public async void DeleteSampleData()
        {
            await connection.DeleteAllAsync<Term>();
            await connection.DeleteAllAsync<Course>();
            await connection.DeleteAllAsync<Assessment>();
            await connection.DeleteAllAsync<Note>();
        }

        public async Task<bool> AddSampleData()
        {
            var record = await connection.Table<Term>().Where(t => t.Title == "Fall Term").ToListAsync();
            if (record.Count > 0)
            {
                return true;
            }


            var term = new Term 
            { 
                Title = "Fall Term", 
                Start = DateTime.Now.AddDays(10), 
                End = DateTime.Now.AddDays(40) 
            };
            await connection.InsertAsync(term);

            var course = new Course 
            { 
                Title = "Software Development", 
                TermId = term.Id, 
                Start = DateTime.Now.AddDays(10), 
                End = DateTime.Now.AddDays(40), 
                Status = "Plan To Take", 
                InstructorName = "Austin Briere",
                InstructorPhone = "8124302576", 
                InstructorEmail = "abriere@wgu.edu"
            };
            await connection.InsertAsync(course);

            var oa = new Assessment
            {
                Name = "Objective Assessment",
                CourseId = course.Id,
                Start = DateTime.Now.AddDays(10),
                End = DateTime.Now.AddDays(40),
                AssessType = "Objective"
            };
            await connection.InsertAsync(oa);

            var pa = new Assessment
            {
                Name = "Performance Assessment",
                CourseId = course.Id,
                Start = DateTime.Now.AddDays(10),
                End = DateTime.Now.AddDays(40),
                AssessType = "Performance"
            };
            await connection.InsertAsync(pa);

            return true;
        }
    }
}
