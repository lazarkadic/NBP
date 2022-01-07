
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
public class PersonController : ControllerBase
{
    private readonly IGraphClient _client;

    public PersonController(IGraphClient client)
    {
        _client =client;
    }

    [HttpPost]
    [Route("CreateActor")]
    public async Task<IActionResult> CreateNodeActor([FromBody]Person  person)
    {
        await _client.Cypher.Create("(p:Person $person)")
                            .WithParam("person",person)
                            .ExecuteWithoutResultsAsync();

        return Ok();
    }
    [HttpGet]
    [Route("GetAllPersons")]
    public async Task<IActionResult> GetAllPersons()
    {
        
        var persons=await _client.Cypher.Match("(n:Person)").Return(n=> n.As<Person>()).ResultsAsync;
        return Ok(persons);
    }

    [HttpGet]
    [Route("GetAllPersons/{id}")]
    public async Task<IActionResult> GetPersonById(Guid id)
    {
        var person=await _client.Cypher.Match("(p:Person)").Where((Person p)=>p.Id==id).Return(p=> p.As<Person>()).ResultsAsync;
        return Ok(person);
    }

}