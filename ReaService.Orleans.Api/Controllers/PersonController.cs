#region Using Directives

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Orleans;
using Orleans.Graph;
using Orleans.Graph.Test.Definition;

#endregion

namespace ReaService.Orleans.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController
    {
        #region Member Fields

        private readonly IClusterClient clusterClient;

        #endregion

        public PersonController(IClusterClient clusterClient)
        {
            this.clusterClient = clusterClient;
        }
        
        // GET api/values/5
        [HttpGet("{id}")]
        public Task<PersonalData> Get(Guid id)
        {
            var person = clusterClient.GetVertexGrain<IPersonVertex>(id, "partition0");
            return person.GetPersonalDataAsync();
        }

        [HttpPost]
        public void Post([FromBody] string value) { }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value) { }

        [HttpDelete("{id}")]
        public void Delete(int id) { }
    }
}