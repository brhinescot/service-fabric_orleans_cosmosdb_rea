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
    [UsedImplicitly]
    public class PersonAgent : AgentGrain, IPerson
    {
        private const string FirstName = "firstName";
        private const string LastName = "lastName";
        private const string MiddleName = "middleName";
        private const string Birthdate = "birthDate";

        #region IPerson Members
        
        Task IPerson.SetPersonalDataAsync(PersonalData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            State[FirstName] = data.FirstName;
            State[MiddleName] = data.MiddleName;
            State[LastName] = data.LastName;
            State[Birthdate] = data.Birthdate;

            return WriteStateAsync();
        }

        Task<PersonalData> IPerson.GetPersonalDataAsync()
        {
            var data = new PersonalData(State[FirstName], State[LastName])
            {
                MiddleName = State[MiddleName],
                Birthdate = State[Birthdate]
            };

            return Task.FromResult(data);
        }

        async Task<IProfile> IPerson.AddProfileAsync(ProfileData data)
        {
            if (data == null) 
                throw new ArgumentNullException(nameof(data));

            await SynchronizationContextRemover.Instance;

            var hasProfileEdge = GrainFactory.GetEdgeGrain<IHasProfile>(Guid.NewGuid(), this);
            var profileVertex = await hasProfileEdge.AddProfile(this, data);

            State.AddOutEdge(hasProfileEdge, profileVertex);

            return profileVertex;
        }

        #endregion
    }
}