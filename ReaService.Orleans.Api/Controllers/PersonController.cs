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
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBaseExt
    {
        #region Member Fields

        private readonly IClusterClient clusterClient;

        #endregion

        public PersonController(IClusterClient clusterClient)
        {
            this.clusterClient = clusterClient;
        }

        [HttpGet("{id}")]
        public Task<PersonalData> GetPerson(Guid id)
        {
            var person = clusterClient.GetVertexGrain<IPersonVertex>(id, "partition0");
            return person.GetPersonalDataAsync();
        }

        [HttpPost]
        public async Task<ActionResult> CreatePerson(PersonalData data)
        {
            var primaryKey = Guid.NewGuid();
            var person = clusterClient.GetVertexGrain<IPersonVertex>(primaryKey, "partition0");
            await person.SetPersonalDataAsync(data);
            return CreatedAtAction(nameof(GetPerson), new {id = primaryKey}, null);
        }

        [HttpPut("{id}")]
        public void Put(Guid id, PersonalData data) { }

        [HttpDelete("{id}")]
        public void Delete(int id) { }
    }
}