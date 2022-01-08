using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Neo4jClient;
using NBP1.Models;

namespace NBP1.Pages
{
    public class MovieModel : PageModel
    {
        private readonly IGraphClient _client;
        public List<Movie> Movies {get; set;}
        public MovieModel(IGraphClient client)
        {
            _client = client;
            Movies = new List<Movie>();
        }
        public async Task OnGetAsync()
        {
            var query = await _client.Cypher.Match("(m:Movie)")
                                            .Return((m) => new { movie = m.As<Movie>() }).ResultsAsync;

            foreach (var item in query)
            {
                Movies.Add(item.movie);
            }
        }
    }
}
