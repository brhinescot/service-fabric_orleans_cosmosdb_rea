#region Using Directives

using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Orleans.Graph.Test.Definition;
using ReaService;
using ReaService.Orleans;

#endregion

namespace Orleans.Graph.Test
{
    /// <summary>
    /// </summary>
    [UsedImplicitly]
    public class PersonVertex : VertexGrain, IPerson
    {
        #region IPerson Members

        /// <summary>
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task IPerson.SetPersonalDataAsync(PersonalData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            State["firstName"] = data.FirstName;
            State["middleName"] = data.MiddleName;
            State["lastName"] = data.LastName;
            State["birthDate"] = data.Birthdate;

            return WriteStateAsync();
        }

        Task<PersonalData> IPerson.GetPersonalDataAsync()
        {
            var data = new PersonalData(State["firstName"], State["lastName"])
            {
                MiddleName = State["middleName"],
                Birthdate = State["birthDate"]
            };

            return Task.FromResult(data);
        }

        public async Task<IProfile> AddProfileAsync(ProfileData data)
        {
            await SynchronizationContextRemover.Instance;

            var hasProfileEdge = GrainFactory.GetEdgeGrain<IHasProfile>(Guid.NewGuid(), this);
            var profileVertex = await hasProfileEdge.AddProfile(this, data);

            State.AddOutEdge(hasProfileEdge, profileVertex);

            return profileVertex;
        }

        #endregion
    }
    
    [UsedImplicitly]
    public class OrganizationAgent : AgentGrain, IOrganization
    {
        public Task AddPerson(IPerson person)
        {
            State["personId"] = person.ToKeyString();
            return WriteStateAsync();
        }
    }
}