using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using DbConnection;
using Newtonsoft.Json.Linq;

namespace movieAPI.Controllers
{
    public class HomeController : Controller
    {
        private readonly DbConnector _dbConnector;

        public HomeController(DbConnector connect)
        {
            _dbConnector = connect;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            List<Dictionary<string, object>> allMovies = _dbConnector.Query("SELECT * FROM movie order by released desc;");
            ViewBag.Movies = allMovies;
            // Other code
            return View();
        }

        [HttpPost]
        [Route("/movies/{movietitle}")]
        public IActionResult QueryMovie(string movietitle)
        {
            var MovieInfo = new Dictionary<string, object>();
            WebRequest.GetMovieDataAsync(movietitle, ApiResponse =>
                {
                    MovieInfo = ApiResponse;
                }
            ).Wait();

            JArray items = (JArray)MovieInfo["results"];
            if(items.Count > 0){
                var item = items.First;
                string t = (string)item["title"];
                double r = (double)item["vote_average"];
                DateTime d = (DateTime)item["release_date"];
                _dbConnector.Query($"insert into movie (name, rating, released) values ('{t}', {r}, '{d.ToString("yyyy-MM-dd")}');");

            }
            // Other code
            return RedirectToAction("Index");
        }

    }
}