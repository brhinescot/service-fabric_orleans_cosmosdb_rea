#region Using Directives

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class PersonController : ControllerBaseExt
    {
        #region Member Fields

        private readonly IGrainFactory grainFactory;
        private readonly IOrganization organization;

        #endregion

        /// <summary>
        ///     The PersonController
        /// </summary>
        /// <param name="grainFactory"></param>
        /// <param name="organization">The organization to which the user belongs.</param>
        public PersonController(IGrainFactory grainFactory, IOrganization organization)
        {
            this.grainFactory = grainFactory;
            this.organization = organization;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PersonalData>> GetPerson(Guid id)
        {
            var person = grainFactory.GetVertexGrain<IPerson>(id, "partition0");
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
            var person = grainFactory.GetVertexGrain<IPerson>(primaryKey, "partition0");
            model.LastName = organization.ToKeyString();
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