#region Using Directives

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Orleans;
using Orleans.Graph;
using Orleans.Graph.Test.Definition;

#endregion

namespace ReaService.Orleans.Api.Controllers
{
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Produces("application/json")]
    [Route("person")]
    [ApiController]
    public class PersonController : ControllerBaseExt
    {
        #region Member Fields

        private readonly IClusterClient clusterClient;
        private readonly IOrganization organization;

        #endregion

        /// <summary>
        ///     The PersonController
        /// </summary>
        /// <param name="clusterClient"></param>
        /// <param name="organization"></param>
        public PersonController(IClusterClient clusterClient, IOrganization organization)
        {
            this.clusterClient = clusterClient;
            this.organization = organization;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PersonalData>> GetPerson(Guid id)
        {
            var person = clusterClient.GetVertexGrain<IPerson>(id, "partition0");
            return Ok(await person.GetPersonalDataAsync());
        }

        /// <summary>
        ///     Creates a new person.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> CreatePerson(PersonalData model)
        {
            var primaryKey = Guid.NewGuid();
            var person = clusterClient.GetVertexGrain<IPerson>(primaryKey, "partition0");
            await person.SetPersonalDataAsync(model);

            await organization.AddPerson(person);

            return CreatedAtAction(nameof(GetPerson), new {id = primaryKey}, null);
        }

        [HttpPut("{id}")]
        public void UpdatePerson(Guid id, PersonalData data) { }

        [HttpDelete("{id}")]
        public void DeletePerson(int id) { }
    }
}