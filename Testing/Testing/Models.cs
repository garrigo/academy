using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Models
{
    public class ToDoItem
    {
        public string Item {get; set;}
        public DateTime Date {get; set;}
        public bool IsCompleted {get; set;}
        public string Description {get; set;}
        public override string ToString()
        {
            return String.Format("{0}: {1} ({2}) => {3}",
                this.Date.ToShortDateString(),
                this.Item,
                this.Description,
                this.IsCompleted ? "X" : "O");
            //Or
            //return $"{Data:d}: {Oggetto} ({Categoria}) => {Completato}";
        }
    }
    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }
        public List<Post> Posts { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public Blog()
        {
            Posts = new List<Post>();
        }


    }
    public class Post
    {
        public int PostId {get; set;}
        public string Title {get; set;}
        public string Content {get; set;}
        public DateTime Date { get; set; }
        public int BlogId { get; set; }
        public Blog Blog {get; set;}
    }

    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class Person
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string FullName { get { return $"{this.Name} {this.Surname}"; } } // Proprietà calcolata
    }
    public class Student : Person
    {
        public int StudentId { get; set; }

    }
        public class Teacher : Person
    {
        public int TeacherId { get; set; }
    }
    public class Exam
    {
        public int ExamId { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; }
        public int SubjectId { get; set; }
        public Subject Subject { get; set; }
        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; }
        public DateTime Date { get; set; }
        public int Mark { get; set; }
    }
    public class Subject
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public int CFU { get; set; }

    }



    // Add Microsoft.Data.Sqlite -  Microsoft.EntityFrameworkCore.Sqlite - Microsoft.EntityFrameworkCore.Design
    // Using package console:
    // Install ef tools: https://learn.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=visual-studio
    // Migrate database: https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=vs
    // Read "Evolving your model" in the previous website to add functionalities in a new migration
    // https://www.entityframeworktutorial.net/efcore/entity-framework-core-console-application.aspx
    // https://fullstack-nuggets.com/ef-core-6-how-to-use-linq-to-write-strongly-typed-queries/
    // https://zetcode.com/csharp/mongodb/
    public class MyContext : DbContext
    {
        protected string uri;
        public DbSet<Post> Posts { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<Subject> Subjects { get; set; }

        public MyContext()
        {
            this.uri = "Data Source=C:\\Users\\boh_h\\source\\repos\\Testing\\Testing\\myefdb.sqlite";

        }
        protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlite(uri);
    }
}
