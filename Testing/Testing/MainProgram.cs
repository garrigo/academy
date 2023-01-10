using Microsoft.VisualBasic;
using System.Collections;
using System.Reflection.Metadata.Ecma335;
using Models;
// See https://aka.ms/new-console-template for more information
using Microsoft.Data.Sqlite;
using System.Text;
using System.Security.Cryptography;
using System.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.Json.Serialization;
using System.Text.Json;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Testing.Models;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;

internal class MainProgram
{
    #region DirectSqlite
    private static void DirectSqlite()
    {
        Console.WriteLine("Starting up...");
        string uri = "Data Source=mydb.sqlite"; // TODO: Move to configuration!

        // Hashing per la password
        MD5 mD5 = MD5.Create();
        using (var cn = new SqliteConnection(uri))
        {
            cn.Open();
            var cmd = cn.CreateCommand();
            cmd.CommandText = "DROP TABLE IF EXISTS users";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "create table users (userid int primary key, username text unique, passwd text, status int)";
            cmd.ExecuteNonQuery();
            cmd.CommandText = "INSERT INTO users VALUES ($userid, $username, $passwd, $status)";
            // Parameters.Add deprecated?
            cmd.Parameters.AddWithValue("$userid", 0);
            cmd.Parameters.AddWithValue("$username", "");
            cmd.Parameters.AddWithValue("$passwd", "");
            cmd.Parameters.AddWithValue("$status", 0);


            for (int i = 0; i < 100; i++)
            {
                cmd.Parameters["$userid"].Value = i;
                cmd.Parameters["$username"].Value = string.Format("User{0}", i);
                cmd.Parameters["$status"].Value = 1;
                // Calcola hash dei bytes della stringa password e converti l'hash binario in hex format
                cmd.Parameters["$passwd"].Value = BitConverter.ToString(mD5.ComputeHash(Encoding.ASCII.GetBytes(string.Format("pass{0}", i))));
                cmd.ExecuteNonQuery();
            }
            cmd.CommandText = "SELECT * FROM users";
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                //reader.GetDataTypeName(0);
                //reader.GetName(0);
                var uid = reader.GetInt32(0);
                var nome = reader.GetString(1);
                var psw = reader.GetString(2);
                var status = reader.GetInt32(3);
                Console.WriteLine($"Ho letto: {uid}\t{nome}\t{psw}\t{status}");
            }
        }
    }
    #endregion DirectSqlite

    #region MigratedSqlite
    
    public static class MigratedSqlite
    {

        public static void PopulateExamDatabase()
        {
            using (MyContext db = new MyContext())
            {
                foreach (var item in db.Exams)
                    db.Exams.Remove(item);
                foreach (var item in db.Categories)
                    db.Categories.Remove(item);
                foreach (var item in db.Students)
                    db.Students.Remove(item);
                foreach (var item in db.Teachers)
                    db.Teachers.Remove(item);
                foreach (var item in db.Subjects)
                    db.Subjects.Remove(item);
                db.Database.ExecuteSqlRaw("UPDATE `sqlite_sequence` SET `seq` = 0");

                //db.Blogs.FromSqlRaw("UPDATE `sqlite_sequence` SET `seq` = 0 WHERE `name` = 'blogs' OR `name` = 'posts';");
                db.SaveChanges();

                CultureInfo provider = CultureInfo.InvariantCulture;
                // It throws Argument null exception  
                db.Categories.AddRange(
                    new Category() { Name = "INF", Description = "Computer Science" },
                    new Category() { Name = "MAT", Description = "Mathematics" },
                    new Category() { Name = "STAT", Description = "Statistics" }
                 );
                db.Teachers.AddRange(
                    new Teacher() { Name = "Joseph", Surname = "Joestar", DateOfBirth = DateTime.ParseExact("10/05/1975", "dd/mm/yyyy", provider)},
                    new Teacher() { Name = "Paolo", Surname = "Bianchi", DateOfBirth = DateTime.ParseExact("29/03/1979", "dd/mm/yyyy", provider) },
                    new Teacher() { Name = "Angela", Surname = "Neri", DateOfBirth = DateTime.ParseExact("04/11/1986", "dd/mm/yyyy", provider) }
                );
                db.Students.AddRange(
                    new Student() { Name = "Giovanni", Surname = "Manesco", DateOfBirth = DateTime.ParseExact("18/07/1996", "dd/mm/yyyy", provider) },
                    new Student() { Name = "Tommaso", Surname = "Salvi", DateOfBirth = DateTime.ParseExact("04/12/1996", "dd/mm/yyyy", provider) },
                    new Student() { Name = "Veronica", Surname = "Mars" , DateOfBirth = DateTime.ParseExact("31/10/1997", "dd/mm/yyyy", provider) }
                );
                db.SaveChanges();
                db.Subjects.AddRange(
                    new Subject() { SubjectName = "HPC", CFU = 6, Category = db.Categories.Where(cat => cat.CategoryId == 1).First() },
                    new Subject() { SubjectName = "Calculus", CFU = 12, Category = db.Categories.Where(cat => cat.CategoryId == 2).First() },
                    new Subject() { SubjectName = "Statistic Inference", CFU = 6, Category = db.Categories.Where(cat => cat.CategoryId == 3).First() }
                );

                db.SaveChanges();

                for (int i = 0; i < 10; i++)
                {
                    db.Exams.Add(new Exam()
                    {
                        Student = db.Students.Where(stud => stud.StudentId == (i % 3) + 1).First(),
                        Subject = db.Subjects.Where(subj => subj.SubjectId == (i % 3) + 1).First(),
                        Teacher = db.Teachers.Where(teac => teac.TeacherId == (i % 3) + 1).First(),
                        Mark = 18 + i,
                        Date = DateTime.Now
                    });
                    
                }
                db.SaveChanges();
            }
        }
        public static void PopulateBlogDatabase()
        {
            using (MyContext db = new MyContext())
            {
                foreach (var item in db.Posts)
                    db.Posts.Remove(item);
                foreach (var item in db.Blogs)
                    db.Blogs.Remove(item);
                foreach (var item in db.Categories)
                    db.Categories.Remove(item);
                db.Database.ExecuteSqlRaw("UPDATE `sqlite_sequence` SET `seq` = 0");

                //db.Blogs.FromSqlRaw("UPDATE `sqlite_sequence` SET `seq` = 0 WHERE `name` = 'blogs' OR `name` = 'posts';");
                db.SaveChanges();

                db.Categories.AddRange(
                    new Category() { Name = "INF", Description = "Computer Science" },
                    new Category() { Name = "MAT", Description = "Mathematics" },
                    new Category() { Name = "STAT", Description = "Statistics" }
                 );

                db.SaveChanges();

                for (int i = 0; i < 10; i++)
                {

                    var cat = db.Categories.Where(c => c.CategoryId == (i % 3) + 1).First();
                    Blog blog = new Blog()
                    {
                        Url = $"www.test{i}.com",
                        Category = db.Categories.Where(c => c.CategoryId == (i % 3) + 1).First()
                    };
                    for (int j = 0; j < 10; j++)
                    {
                        Post post = new Post()
                        {
                            Title = $"Title {j}",
                            Content = $"Content {j}",
                        };
                        blog.Posts.Add(post);
                    }
                    db.Blogs.Add(blog);
                }
                db.SaveChanges();
            }

        }
        public static void PrintBlog()
        {
            using (MyContext db = new MyContext()) { 
                foreach (var blog in db.Blogs.Include(blog => blog.Category).Include(blog => blog.Posts))
                {
                    Console.WriteLine($"Blog ID: {blog.BlogId} - Category: {blog.Category.Name} - URL: {blog.Url} - Posts:");
                    foreach (var post in blog.Posts)
                        Console.WriteLine($"\tPost ID: {post.PostId} - Title: {post.Title} - Content: {post.Content}");
                }

                // Se voglio visualizzare anche l'intero oggetto Blog devo fare Include (join)
                // Dato che Posts <-> Blog <-> Category, bisogna usare ThenInclude per unire Post e Category
                var posts = db.Posts.Where(post => post.Content.Contains("4")).Include(post => post.Blog).ThenInclude(blog => blog.Category).ToList();
                foreach (var post in posts)
                    Console.WriteLine($"Post title: {post.Title} - Blog ID: {post.BlogId} - Blog URL: {post.Blog.Url}");


            }
        }
        public static void PrintExam()
        {
            using (MyContext db = new MyContext())
            {
                foreach (var exam in db.Exams.Where(exam => exam.Mark > 22).Include(exam => exam.Student).Include(exam => exam.Teacher).Include(exam => exam.Subject).ThenInclude(subj => subj.Category))
                {
                    Console.WriteLine($"Exam ID: {exam.ExamId} - Subject: {exam.Subject.SubjectName} - Student: {exam.Student.FullName} - Mark: {exam.Mark} - Teacher: {exam.Teacher.Surname}");
                }
                var exam2 = db.Exams
                    .GroupBy(ex => ex.StudentId)
                    .Select(q => new { Student = q.Key, Average = q.Average(ex => ex.Mark) })
                    .OrderBy(q => q.Student);
                using (var wr = File.CreateText("C:\\Users\\boh_h\\source\\repos\\Testing\\Testing\\avg.json"))
                {
                    foreach (var line in db.Students)
                    {
                        var json = JsonSerializer.Serialize(line);
                        wr.WriteLine(json);
                        var des = JsonSerializer.Deserialize<List<Student>>(json);

                    }
                }
                using (StreamReader r = new StreamReader("C:\\Users\\boh_h\\source\\repos\\Testing\\Testing\\avg.json"))
                {
                        var des = JsonSerializer.Deserialize<List<Student>>(r.ReadToEnd());
                }


            }

        }


    }
    #endregion

    #region MongoDB
    private static void MongoTest()
    {
        string uri = "mongodb+srv://garrigo:SYey66cARzzC2chL@cluster0.uht80ny.mongodb.net/?retryWrites=true&w=majority";

        var settings = MongoClientSettings.FromConnectionString(uri);
        settings.ServerApi = new ServerApi(ServerApiVersion.V1);
        settings.LinqProvider = LinqProvider.V3;
        var client = new MongoClient(settings);

        var database = client.GetDatabase("BookStore");
        var cls = database.GetCollection<Book>("Books");

        var builder = Builders<BsonDocument>.Filter;
        var filter = builder.Gt("Price", 20) & builder.Lt("Price", 40);

        //var tutti = cls.Find(filter);

        //Delete all
        cls.DeleteMany(x => true);


        //Add Dante if not already exist
        if (cls.Find(x => x.Title == "La Divina Commedia").FirstOrDefault() == null)
        {
            Book doc = new Book
            {
                Title = "La Divina Commedia",
                Price = (decimal)12.9,
                Category = "Classics",
                Author = "Dante Alighieri"
            };

            cls.InsertOne(doc);
        }



        // Add 100 entries if not already done
        if (cls.Find(x => x.Title == "Libro 1").FirstOrDefault() == null)
        {
            List<Book> addenda = new List<Book>();
            for (int i = 1; i < 101; i++)
                addenda.Add(new Book
                {
                    Title = $"Libro {i}",
                    Price = (decimal)(14 + i),
                    Category = "Genere",
                    Author = $"Autore {(i % 3) + 1}"
                });

            cls.InsertMany(addenda);

        }

        // Write Dante
        var tutti = cls.Find(x => x.Author == "Dante Alighieri").ToList();
        foreach (var book in tutti)
        {
            Console.WriteLine($"{book.Title} di {book.Author} a {book.Price}");
        }
        // Delete Dante
        cls.DeleteMany(x => x.Title == "La Divina Commedia");

        // Find Dante
        string dante = (cls.Find(x => x.Author == "Dante Alighieri").FirstOrDefault() != null ? "Found" : "Not found");
        {
            Console.WriteLine(dante);
        }

        // Update 
        var lb = cls.Find(x => x.Author == "Autore 1").ToList();
        foreach(var l in lb)
        {
            l.Price -= 1M;
            cls.ReplaceOne(x => x.Id == l.Id, l);
        }

    }
    #endregion

    #region HttpSocket
    public static void HttpSocket ()
    {
        while (true)
        {
            Console.WriteLine("1: Find by ID - 2: Find by RX - Other: Exit");
            int choose = Int32.Parse(Console.ReadLine());
            if (choose < 1 || choose > 2)
                break;
            Console.WriteLine(String.Format("Inserisci {0} del log da trovare:", choose == 1 ? "id" : "RX"));
            var arg = Console.ReadLine();
            var client = new HttpClient();
            string uri = choose == 1 ? $"https://localhost:7038/LogEntry/Get/{arg}" 
                                     : $"https://localhost:7038/LogEntry/Get/RX/{arg}";
            var response = client.GetAsync(uri).Result;
            try
            {
                response.EnsureSuccessStatusCode();
                if (choose == 1)
                {
                    var tmp = response.Content.ReadAsStringAsync().Result;
                    Log log = JsonSerializer.Deserialize<Log>(tmp);
                    Console.WriteLine($"ID={log.id}, RX={log.nominativoRX}, TX={log.nominativoTX}");
                }
                else
                {
                    var tmp = response.Content.ReadAsStringAsync().Result.FirstOrDefault();
                    Log log = JsonSerializer.Deserialize<Log>(tmp);
                    Console.WriteLine($"ID={log.id}, RX={log.nominativoRX}, TX={log.nominativoTX}");
                }
            }
            catch (HttpRequestException e) { Console.WriteLine(e.Message); }
            finally
            {
                Console.WriteLine($"Response: {response.StatusCode}");
            }
        }
        
        
    }
    #endregion

    private static void Main(string[] args)
    {
        //Console.WriteLine(System.IO.Directory.GetCurrentDirectory().ToString());
        #region DirectSqlite
        //DirectSqlite();
        //MigratedSqlite.PopulateBlogDatabase();
        //MigratedSqlite.PrintBlog();
        #endregion

        #region MigratedSqlite
        //MigratedSqlite.PopulateExamDatabase();
        // MigratedSqlite.PrintExam();
        #endregion

        #region MongoDB
        //MongoTest();
        #endregion

        #region HttpSocket
        HttpSocket();
        #endregion

        //int choose = -1;
        //TASK
        //VANNO IN PARALLELO
        //Task.Run(() => Stampa());
        //Task.Run(() => Stampa());
        //Console.Read();

        //Delegati dlg = new Delegati();
        //dlg.Vai();


        //Band hobbits = new()
        //{
        //    Name = "The Hobbits",
        //    OriginCountry = "The Shire",
        //    Components = "Frodo, Sam, Pipino, Merry"
        //};

        //Solo hp = new()
        //{
        //    Name = "Harry Potter",
        //    OriginCountry = "The U.K."
        //};

        //Artist[] artists = { hobbits, hp };
        //foreach (Artist artist in artists)
        //{
        //    Console.WriteLine(artist.Description);
        //}

        //ArrayList lst = new();
        //lst.AddRange(new object[] { true, false, 1, 2, 3, 3.5, "stringa" });
        //var query = lst.OfType<int>();

        //Console.WriteLine(query);

        //Console.WriteLine("Welcome to ToDoList Manager.\nChoose:\n0 Esci\n1 To Do List\n2 Add thing to do\nSelect Number:");
        //choose = Int32.Parse(Console.ReadLine());
        //Console.WriteLine();
        //switch (choose)
        //{
        //    case 0:
        //        break;
        //    case 1:
        //        break;
        //    case 2:
        //        break;
        //    default:
        //        Console.WriteLine("Error: choice not recognized!");
        //        break;

        //}
        //static void NewItem()
        //{

        //}
    }

    #region Interface Testing
    class Delegati
    {
        private delegate void ioDelego(); // Dichiarazione di funzione delegate
        private delegate int del(int i);

        //Assegnazione di una lambda function a un delegate
        private del delegInt = x => x * x;
        private ioDelego Dell;
        private ioDelego Dell2;
        private ioDelego Dell3;

        public Delegati()
        {
            Dell = Saluta; // Passa indirizzo della funzione
            // Concatena due funzioni
            Dell2 = Saluta1;
            Dell3 = Dell + Dell2;
            Dell3 += Dell;

            // Funzione anonima (non lambda) per definire funzioni piccole inline
            //Dell = delegate () { Console.WriteLine("Hello there, but without using methods."); };
        }

        private void Saluta()
        {
            Console.WriteLine("A");
        }
        private void Saluta1()
        {
            Console.WriteLine("B");
        }
        public void Vai()
        {
            Dell3();
            Console.WriteLine(delegInt(4));
        }
    }

    public interface Artist
    {
        // le proprietà dell'interfaccia sono per definizione public e abstract
        string Name { get; set; }
        string OriginCountry { get; set; }
        string Description { get; }
    }

    public class Band : Artist
    { 
        public string Name { get; set; }
        public string OriginCountry { get; set; }
        public string Components { get; set; }
        public string Description { get { return "Name: "+ Name + "\nOrigin Country: " + OriginCountry + "\nComponents: "+ Components +  "\n"; } }
    }

    public class Solo : Artist
    {
        public string Name { get; set; }
        public string OriginCountry { get; set; }
        public string Description { get { return "Name: " + Name + "\nOrigin Country: " + OriginCountry + "\n"; } }
    }
    #endregion

    #region Task
    private static void Stampa()
    {
        for(int i = 0; i < 100; i++)
        {
            Console.WriteLine(i + " " +  Task.CurrentId);
        }
    }
    public void ChiamoTask()
    {
        Task.Run(() => Stampa());
    }
    #endregion
}