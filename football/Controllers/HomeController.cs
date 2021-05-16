using football.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Diagnostics;
using Dapper;
using System.Linq;


namespace football.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _config;

        public HomeController(ILogger<HomeController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }
        public IDbConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }

        }
        
        public List<komand> kk;
        public IActionResult Index()
        {
            kk = Getkomand();
            ViewBag.komand = kk;
            return View();
        }
        [HttpPost]

        public IActionResult Index(Form model)
        {
            kk = Getkomand();
            ViewBag.komand = kk;
            Debug.WriteLine(model.name_comand);

            using (IDbConnection db = Connection)
            {
                if (model.name_comand1 == null)
                {
                    var sqlQuery = "INSERT INTO footballers (name, last_name, gender,date,  komanda, country) VALUES(@name,@last_name,@gender,@date,@name_comand,@country)";
                    db.Execute(sqlQuery, model);
                }
                else {
                    var sqlQuery = "INSERT INTO komand (name_komand) VALUES(@name_comand1); SELECT CAST(SCOPE_IDENTITY() as int)";
                    model.name_comand = db.Query<int>(sqlQuery, model).FirstOrDefault().ToString();
                    db.Execute(sqlQuery, model);
                    var sqlQuery1 = "INSERT INTO footballers (name, last_name, gender,date,  komanda, country) VALUES(@name,@last_name,@gender,@date,@name_comand,@country)";
                    db.Execute(sqlQuery1, model);
                }
                

            }
            return View();
        }
        public IActionResult Privacy()
        {
            kk = Getkomand();
            ViewBag.komand = kk;
            var model = Getfots();
            return View(model);
        }
        
        [HttpPost]
        public string UpFoots (fots foots)
        {

            
            using (IDbConnection db = Connection)
            {
                var sqlQuery = "UPDATE footballers SET name = @name,last_name = @last_name,gender=@gender,date=@date, komanda=@name_komand, country=@country WHERE id = @id";
                db.Execute(sqlQuery, foots);

            }
            return $"Успешно обновленно!";
        }

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}


        private List<fots> Getfots()
        {
            using (IDbConnection db = Connection)
            {
                var result = db.Query<fots>(@" SELECT  footballers.id, footballers.name,footballers.last_name,footballers.gender,footballers.date,komand.name_komand,footballers.country
                                            FROM footballers INNER JOIN komand
                                            ON footballers.komanda = komand.id").ToList();
                return result;
            }
        }
        private List<komand> Getkomand()
        {
            using (IDbConnection db = Connection)
            {
                var result = db.Query<komand>(@"SELECT * FROM komand ").ToList();
                return result;
            }
        }
        public class komand
        {
            public int id { get; set; }
            public string name_komand { get; set; }
        }

        public class fots
        {
            public int id { get; set; }
            public string name { get; set; }
            public string last_name { get; set; }
            public string gender { get; set; }
            public string date { get; set; }
            public string name_komand { get; set; }
            public string country { get; set; }

        }


    }
}
