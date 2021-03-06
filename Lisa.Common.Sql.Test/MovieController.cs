﻿using Microsoft.AspNet.Mvc;

namespace Lisa.Common.Sql.Test
{
    [Route("/")]
    public class MovieController
    {
        public ActionResult Get()
        {
            var movies = _db.FetchMovies();
            return new HttpOkObjectResult(movies);
        }

        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            _db.FetchMovies();
            var movie = _db.FetchMovie(id);
            return new HttpOkObjectResult(movie);
        }

        [HttpPost]
        public ActionResult Post([FromBody] Movie movie)
        {
            var id =_db.CreateMovie(movie);
            var created = _db.FetchMovie(id);
            return new HttpOkObjectResult(created);
        }

        [HttpPost("many")]
        public ActionResult PostMany([FromBody] Movie[] movies)
        {
            _db.CreateMovies(movies);
            return new HttpOkResult();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            _db.DeleteMovie(id);
            return new HttpStatusCodeResult(204);
        }

        private Database _db = new Database();
    }
}