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
            DatePublish = movie.DatePublish,
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
        var movies = await _client.Cypher.Match("(m:Movie)").Return(m => m.As<Movie>()).ResultsAsync;
        return Ok(movies);
    }

    [HttpGet]
    [Route("GetMoviesById/{id}")]
    public async Task<IActionResult> GetMoviesById(Guid id)
    {
        var movie = await _client.Cypher.Match("(m:Movie)").Where((Movie m) => m.Id == id).Return(m => m.As<Movie>()).ResultsAsync;
        return Ok(movie);
    }

    [HttpGet]
    [Route("GetMovieByTitle/{title}")]
    public async Task<IActionResult> GetMovieByTitle(String title)
    {
        var movie = await _client.Cypher.Match("(m:Movie)").Where((Movie m) => m.Title == title).Return(m => m.As<Movie>()).ResultsAsync;
        return Ok(movie);
    }

    [HttpGet]
    [Route("GetMovieByRate/{rate}")]
    public async Task<IActionResult> GetMovieByRate(double rate)
    {
        var movie = await _client.Cypher.Match("(m:Movie)").Where((Movie m) => m.Rate == rate).Return(m => m.As<Movie>()).ResultsAsync;
        return Ok(movie);
    }

    [HttpPut]
    [Route("AddMovieRate/{id}/{rate}")]
    public async Task<IActionResult> AddMovieRate(Guid id, double rate)
    {
        if(rate >= 0 && rate <= 5)
        {
            // var movie = await _client.Cypher.Match("(m:Movie)")
            //                                 .Where((Movie m) => m.Id == id)
            //                                 .Return(m => m.As<string>("m.Rate")).ResultsAsync;
            
            var movie = await _client.Cypher.Match("(m:Movie)")
                                             .Where((Movie m) => m.Id == id)
                                             .Return(m => m.As<Movie>()).ResultsAsync;

            // Dictionary<string, object> queryParameters = new Dictionary<string, object>();
            // queryParameters.Add("id", id);

            // var query = new Neo4jClient.Cypher.CypherQuery("MATCH (m:Movie) WHERE m.Id == " + id + " return m", queryParameters, Neo4jClient.Cypher.CypherResultMode.Projection, "neo4j");

            // List<Movie> movie1 = ((IRawGraphClient)_client).ExecuteGetCypherResultsAsync<Movie>(query);
            return Ok(movie);
        }
        return BadRequest();
    }

}