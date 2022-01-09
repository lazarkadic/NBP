
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
    public async Task<IActionResult> CreateNodeActor(Person  person)
    {

        Person person_new=new()
        {
            Id = Guid.NewGuid(), 
            Name = person.Name, 
            BornYear=person.BornYear,
        };
        await _client.Cypher.Create("(p:Person $person)")
                            .WithParam("person",person_new)
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
    [Route("GetProperty/{id}")]
    public async Task<IActionResult> GetProperty(Guid id)
    {
        
        var person=await _client.Cypher.Match("(p:Person)").Where((Person p)=>p.Id==id).Return(p=> p.As<Person>().Name).ResultsAsync;
        return Ok(person);
    }
    [HttpGet]
    [Route("GetPersonsById/{id}")]
    public async Task<IActionResult> GetPersonById(Guid id)
    {
        var person=await _client.Cypher.Match("(p:Person)").Where((Person p)=>p.Id==id).Return(p=> p.As<Person>()).ResultsAsync;
        return Ok(person);
    }
    [HttpGet]
    [Route("GetPersonByName/{name}")]
    public async Task<IActionResult> GetPersonByName(String name)
    {
        var person=await _client.Cypher.Match("(p:Person)").Where((Person p)=>p.Name==name).Return(p=> p.As<Person>()).ResultsAsync;
        return Ok(person);
    }
    [HttpDelete]
    [Route("DeletePersonByName/{name}")]
    public async Task<IActionResult> DeletePersonByName(string name)
    {
        await _client.Cypher.Match("(p:Person)").Where((Person p)=>p.Name==name).Delete("p").ExecuteWithoutResultsAsync();

        return Ok();
    }
    [HttpPut]
    [Route("UpdatePersonByName/{name}")]
    public async Task<IActionResult> UpdatePersonByName(string name,Person person)
    {
        Person person_new=new()
        {
            Name=person.Name,
            BornYear=person.BornYear,

        };
        
        await _client.Cypher.Match("(p:Person)").Where((Person p)=>p.Name==name).Set("p=$person").WithParam("person",person_new).ExecuteWithoutResultsAsync();
        return Ok();
    }

    [HttpGet]
    [Route("{pid}/{rtype}/{title}")]
    public async Task<IActionResult> Connect(Guid pid, string rtype, string title)
    {
        if(rtype.ToLower() == "actedin")
            rtype = "Acted_in";
        if(rtype.ToLower() == "directedby")
            rtype = "Directed_by";
        
        await _client.Cypher.Match("(p:Person), (m:Movie)")
                            .Where((Person p, Movie m) => p.Id == pid &&  m.Title == title)
                            .Create("(p)-[r:" + rtype + "]->(m)")
                            .ExecuteWithoutResultsAsync();
        return Ok();
    }

    [HttpGet]
    [Route("{rtype}/Diconnect/{title}")]
    public async Task<IActionResult> Disconnect(string rtype, string title)
    {
        if(rtype.ToLower() == "actedin")
            rtype = "Acted_in";
        if(rtype.ToLower() == "directedby")
            rtype = "Directed_by";
        
        await _client.Cypher.Match("p=()-[r:" + rtype + "]->(m:Movie)")
                            .Where((Movie m) => m.Title == title)
                            .Delete("r")
                            .ExecuteWithoutResultsAsync();
        return Ok();
    }

}