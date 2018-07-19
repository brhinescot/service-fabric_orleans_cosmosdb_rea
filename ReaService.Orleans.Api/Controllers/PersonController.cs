using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Orleans;
using Orleans.Graph;
using Orleans.Graph.Test.Definition;

namespace ReaService.Orleans.Api.Controllers
{
    [Route("api/[controller]")]
    public class PersonController : Controller
    {
        private readonly IClusterClient clusterClient;

        public PersonController(IClusterClient clusterClient)
        {
            this.clusterClient = clusterClient;
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public Task<PersonalData> Get(Guid id)
        {
            var person = clusterClient.GetVertexGrain<IPersonVertex>(id, "partition0"); 
            return person.GetPersonalDataAsync();
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
