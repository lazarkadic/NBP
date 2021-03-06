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
    public async Task<IActionResult> GetAllMovies()//NBP1.Pages.MovieModel model)
    {
        var movies = await _client.Cypher.Match("(m:Movie)")
                                        .Return(m => m.As<Movie>()).ResultsAsync;
        return Ok(movies);
        // var query = await _client.Cypher.Match("(m:Movie)")
        //                                 .Return((m) => new { movie = m.As<Movie>() }).ResultsAsync;

        // foreach (var item in query)
        // {
        //     model.Movies.Add(item.movie);
        // }

        // return Pages // fali samo kako da se vrati nazad na Razor stranicu??
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

    [HttpDelete]
    [Route("DeleteMovieById/{id}")]
    public async Task<IActionResult> DeleteMovieById(Guid id)
    {
        await _client.Cypher.Match("(m:Movie)")
                            .Where((Movie m) => m.Id == id)
                            .Delete("m")
                            .ExecuteWithoutResultsAsync();

        return Ok();
    }

    [HttpGet]
    [Route("GetAllLabelsForMovie/{title}")]
    public async Task<IActionResult> GetAllLabelsForMovie(string title)
    {
        var movieLabels = await _client.Cypher.Match("(m:Movie)")
                                        .Where((Movie m) => m.Title == title)
                                        .ReturnDistinct(m => m.Labels()).ResultsAsync;
        return Ok(movieLabels);
    }

    [HttpGet]
    [Route("GetAllLabels")]
    public async Task<IActionResult> GetAllLabels()
    {
        var movieLabels = await _client.Cypher.Match("(m)")
                                        .ReturnDistinct(m => m.Labels()).ResultsAsync;
        return Ok(movieLabels);
    }

    [HttpPost]
    [Route("SetLabelForMovie/{title}/{label}")]
    public async Task<IActionResult> GetAllLabels(string title, string label)
    {
        var movieLabels = await _client.Cypher.Match("(m:Movie)")
                                              .Where((Movie m) => m.Title == title)
                                              .Set("m:"+ label +"")
                                              .ReturnDistinct(m => m.Labels()).ResultsAsync;
        return Ok(movieLabels);
    }

}