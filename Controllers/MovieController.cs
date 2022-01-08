using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NBP1.Models;
using Neo4j.Driver;
using Neo4jClient;

[Route("[controller]")]
[ApiController]
public class MovieController : ControllerBase
{
    private readonly IGraphClient _client;

    public MovieController(IGraphClient client)
    {
        _client = client;
    }

    [HttpPost]
    [Route("CreateMovie")]
    public async Task<IActionResult> CreateNodeActor(Movie  movie)
    {

        Movie movieNew = new()
        {
            Id = Guid.NewGuid(), 
            Title = movie.Title, 
            Description = movie.Description,
            ImageUri = movie.ImageUri,
            PublishingDate = movie.PublishingDate,
            Rate = movie.Rate,
            RateCount = movie.RateCount
        };

        await _client.Cypher.Create("(m:Movie $movie)")
                            .WithParam("movie", movieNew)
                            .ExecuteWithoutResultsAsync();

        return Ok();
    }

    [HttpGet]
    [Route("GetAllMovies")]
    public async Task<IActionResult> GetAllMovies()
    {
        var movies = await _client.Cypher.Match("(m:Movie)")
                                        .Return(m => m.As<Movie>()).ResultsAsync;
        return Ok(movies);
    }

    [HttpGet]
    [Route("GetMoviesById/{id}")]
    public async Task<IActionResult> GetMoviesById(Guid id)
    {
        var movie = await _client.Cypher.Match("(m:Movie)")
                                        .Where((Movie m) => m.Id == id)
                                        .Return(m => m.As<Movie>()).ResultsAsync;
        return Ok(movie);
    }

    [HttpGet]
    [Route("GetMovieByTitle/{title}")]
    public async Task<IActionResult> GetMovieByTitle(String title)
    {
        var movie = await _client.Cypher.Match("(m:Movie)")
                                        .Where((Movie m) => m.Title == title)
                                        .Return(m => m.As<Movie>()).ResultsAsync;
        return Ok(movie);
    }

    [HttpGet]
    [Route("GetMovieByRate/{rate}")]
    public async Task<IActionResult> GetMovieByRate(double rate)
    {
        var movie = await _client.Cypher.Match("(m:Movie)")
                                        .Where((Movie m) => m.Rate == rate)
                                        .Return(m => m.As<Movie>()).ResultsAsync;
        return Ok(movie);
    }

    [HttpPut]
    [Route("AddMovieRate/{id}/{rate}")]
    public async Task<IActionResult> AddMovieRate(Guid id, double rate)
    {
        if(rate >= 0 && rate <= 5)
        {
            var query = await _client.Cypher.Match("(m:Movie)")
                                            .Where((Movie m) => m.Id == id)
                                            .Return((m) => new { movie = m.As<Movie>() }).ResultsAsync;

            Movie m = new Movie();
            foreach (var item in query)
            {
                m = item.movie;
            }

            m.RateCount += 1;
            m.Rate += rate;
            m.Rate = m.Rate/m.RateCount;

            query = await _client.Cypher.Match("(m:Movie)")
                                            .Where((Movie m) => m.Id == id)
                                            .Set("m = $movie")
                                            .WithParam("movie", m)
                                            .Return((m) => new { movie = m.As<Movie>() }).ResultsAsync;
            return Ok(query);
        }
        return BadRequest();
    }

}