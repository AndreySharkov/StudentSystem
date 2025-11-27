using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StudentSystem.Data;
using StudentSystem.Data.Models;
using StudentSystem.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StudentSystem
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<IConfiguration>(configuration);
                    services.AddDbContext<StudentSystemDbContext>(options =>
                        options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
                })
                .Build();

            using (var serviceScope = host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;
                var context = services.GetRequiredService<StudentSystemDbContext>();
                context.Database.Migrate(); // Ensure database is created and up-to-date

                // Seeding logic
                // Seed Students
                var students = new List<Student>
                {
                    new Student { Name = "Alice Smith", PhoneNumber = "1234567890", RegisteredOn = DateTime.UtcNow.AddDays(-30), Birthday = new DateTime(2000, 5, 15) },
                    new Student { Name = "Bob Johnson", PhoneNumber = "0987654321", RegisteredOn = DateTime.UtcNow.AddDays(-60), Birthday = new DateTime(1999, 10, 20) },
                    new Student { Name = "Charlie Brown", PhoneNumber = "1122334455", RegisteredOn = DateTime.UtcNow.AddDays(-90), Birthday = new DateTime(2001, 1, 25) },
                    new Student { Name = "Diana Prince", PhoneNumber = "5544332211", RegisteredOn = DateTime.UtcNow.AddDays(-120), Birthday = new DateTime(1998, 7, 30) },
                    new Student { Name = "Eve Adams", PhoneNumber = "9988776655", RegisteredOn = DateTime.UtcNow.AddDays(-150), Birthday = new DateTime(2002, 3, 5) }
                };
                if (!context.Students.Any())
                {
                    context.Students.AddRange(students);
                    context.SaveChanges();
                }

                // Seed Courses
                var courses = new List<Course>
                {
                    new Course { Name = "C# Advanced", Description = "Advanced C# programming", StartDate = DateTime.UtcNow.AddDays(-20), EndDate = DateTime.UtcNow.AddDays(10), Price = 300.00m },
                    new Course { Name = "SQL Fundamentals", Description = "Introduction to SQL databases", StartDate = DateTime.UtcNow.AddDays(-40), EndDate = DateTime.UtcNow.AddDays(-10), Price = 250.00m },
                    new Course { Name = "Web Development Basics", Description = "HTML, CSS, JavaScript", StartDate = DateTime.UtcNow.AddDays(-70), EndDate = DateTime.UtcNow.AddDays(-20), Price = 350.00m },
                    new Course { Name = "Data Structures", Description = "Algorithms and Data Structures", StartDate = DateTime.UtcNow.AddDays(-100), EndDate = DateTime.UtcNow.AddDays(-50), Price = 400.00m }
                };
                if (!context.Courses.Any())
                {
                    context.Courses.AddRange(courses);
                    context.SaveChanges();
                }

                // Seed Resources
                var resources = new List<Resource>
                {
                    new Resource { Name = "C# Advanced Video 1", Url = "http://example.com/csharp_video1", ResourceType = ResourceType.Video, Course = courses[0] },
                    new Resource { Name = "C# Advanced Presentation", Url = "http://example.com/csharp_pres", ResourceType = ResourceType.Presentation, Course = courses[0] },
                    new Resource { Name = "SQL Book", Url = "http://example.com/sql_book", ResourceType = ResourceType.Document, Course = courses[1] },
                    new Resource { Name = "Web Dev Tutorial", Url = "http://example.com/webdev_tut", ResourceType = ResourceType.Video, Course = courses[2] },
                    new Resource { Name = "Data Structures Notes", Url = "http://example.com/ds_notes", ResourceType = ResourceType.Document, Course = courses[3] }
                };
                if (!context.Resources.Any())
                {
                    context.Resources.AddRange(resources);
                    context.SaveChanges();
                }

                // Seed Homeworks
                var homeworks = new List<Homework>
                {
                    new Homework { Content = "http://example.com/hw1.zip", ContentType = ContentType.Zip, SubmissionTime = DateTime.UtcNow.AddDays(-5), Student = students[0], Course = courses[0] },
                    new Homework { Content = "http://example.com/hw2.pdf", ContentType = ContentType.Pdf, SubmissionTime = DateTime.UtcNow.AddDays(-15), Student = students[1], Course = courses[1] },
                    new Homework { Content = "http://example.com/hw3.app", ContentType = ContentType.Application, SubmissionTime = DateTime.UtcNow.AddDays(-25), Student = students[2], Course = courses[2] },
                    new Homework { Content = "http://example.com/hw4.zip", ContentType = ContentType.Zip, SubmissionTime = DateTime.UtcNow.AddDays(-35), Student = students[0], Course = courses[1] }
                };
                if (!context.Homeworks.Any())
                {
                    context.Homeworks.AddRange(homeworks);
                    context.SaveChanges();
                }

                // Seed StudentCourses
                var studentCourses = new List<StudentCourse>
                {
                    new StudentCourse { Student = students[0], Course = courses[0] },
                    new StudentCourse { Student = students[0], Course = courses[1] },
                    new StudentCourse { Student = students[1], Course = courses[1] },
                    new StudentCourse { Student = students[2], Course = courses[2] },
                    new StudentCourse { Student = students[3], Course = courses[0] },
                    new StudentCourse { Student = students[4], Course = courses[3] }
                };
                if (!context.StudentCourses.Any())
                {
                    context.StudentCourses.AddRange(studentCourses);
                    context.SaveChanges();
                }

                // LINQ Queries
                Console.WriteLine("--- LINQ Queries ---");

                // Query 1: List all students with their registered date and number of courses.
                Console.WriteLine("\n1. List all students with their registered date and number of courses.");
                var studentsWithCourses = context.Students
                    .Select(s => new
                    {
                        s.Name,
                        s.RegisteredOn,
                        CoursesCount = s.StudentCourses.Count
                    })
                    .ToList();

                foreach (var student in studentsWithCourses)
                {
                    Console.WriteLine($"Student Name: {student.Name}, Registered On: {student.RegisteredOn.ToShortDateString()}, Courses Enrolled: {student.CoursesCount}");
                }

                // Query 2: List all courses with total number of resources.
                Console.WriteLine("\n2. List all courses with total number of resources.");
                var coursesWithResources = context.Courses
                    .Select(c => new
                    {
                        c.Name,
                        ResourcesCount = c.Resources.Count
                    })
                    .ToList();

                foreach (var course in coursesWithResources)
                {
                    Console.WriteLine($"Course Name: {course.Name}, Resources Count: {course.ResourcesCount}");
                }

                // Query 3: Show all homework submissions for a given student (e.g., 'Alice Smith').
                Console.WriteLine("\n3. Show all homework submissions for a given student (e.g., 'Alice Smith').");
                var studentName = "Alice Smith"; // Example student name
                var studentHomeworks = context.Students
                    .Where(s => s.Name == studentName)
                    .Select(s => new
                    {
                        s.Name,
                        Homeworks = s.Homeworks.Select(h => new { h.Content, h.SubmissionTime, CourseName = h.Course.Name })
                    })
                    .FirstOrDefault();

                if (studentHomeworks != null)
                {
                    Console.WriteLine($"Homework submissions for {studentHomeworks.Name}:");
                    foreach (var homework in studentHomeworks.Homeworks)
                    {
                        Console.WriteLine($"- Course: {homework.CourseName}, Content: {homework.Content}, Submission Time: {homework.SubmissionTime}");
                    }
                }
                else
                {
                    Console.WriteLine($"Student with name '{studentName}' not found.");
                }

                // Query 4: Show students ordered by number of courses they are enrolled in.
                Console.WriteLine("\n4. Show students ordered by number of courses they are enrolled in.");
                var studentsOrderedByCourses = context.Students
                    .OrderByDescending(s => s.StudentCourses.Count)
                    .Select(s => new
                    {
                        s.Name,
                        CoursesCount = s.StudentCourses.Count
                    })
                    .ToList();

                Console.WriteLine("Students ordered by number of courses enrolled:");
                foreach (var student in studentsOrderedByCourses)
                {
                    Console.WriteLine($"- Student Name: {student.Name}, Courses Enrolled: {student.CoursesCount}");
                }

                // Query 5: Show all courses that have at least one homework submitted after their end date.
                Console.WriteLine("\n5. Show all courses that have at least one homework submitted after their end date.");
                var coursesWithLateHomework = context.Courses
                    .Where(c => c.Homeworks.Any(h => h.SubmissionTime > c.EndDate))
                    .Select(c => new
                    {
                        c.Name,
                        c.StartDate,
                        c.EndDate,
                        LateHomeworks = c.Homeworks.Where(h => h.SubmissionTime > c.EndDate)
                                                    .Select(h => new { h.Content, h.SubmissionTime, StudentName = h.Student.Name })
                    })
                    .ToList();

                if (coursesWithLateHomework.Any())
                {
                    Console.WriteLine("Courses with late homework submissions:");
                    foreach (var course in coursesWithLateHomework)
                    {
                        Console.WriteLine($"Course: {course.Name} (Ends: {course.EndDate.ToShortDateString()})");
                        foreach (var homework in course.LateHomeworks)
                        {
                            Console.WriteLine($"- Late Homework: {homework.Content} by {homework.StudentName} (Submitted: {homework.SubmissionTime})");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No courses found with late homework submissions.");
                }
            }
        }
    }
}
