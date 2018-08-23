#region Using Directives

using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Orleans.Graph.Test.Definition;
using ReaService;

#endregion

namespace Orleans.Graph.Test
{
    /// <summary>
    /// </summary>
    [UsedImplicitly]
    public class PersonVertex : VertexGrain, IPersonVertex
    {
        #region IPersonVertex Members

        /// <summary>
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task IPersonVertex.SetPersonalDataAsync(PersonalData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            State["firstName"] = data.FirstName;
            State["middleName"] = data.MiddleName;
            State["lastName"] = data.LastName;
            State["birthDate"] = data.Birthdate;

            return WriteStateAsync();
        }

        Task<PersonalData> IPersonVertex.GetPersonalDataAsync()
        {
            var data = new PersonalData(State["firstName"], State["lastName"])
            {
                MiddleName = State["middleName"],
                Birthdate = State["birthDate"]
            };

            return Task.FromResult(data);
        }

        public async Task<IProfileVertex> AddProfileAsync(ProfileData data)
        {
            await SynchronizationContextRemover.Instance;

            var hasProfileEdge = GrainFactory.GetEdgeGrain<IHasProfileEdge>(Guid.NewGuid(), this);
            var profileVertex = await hasProfileEdge.AddProfile(this, data);

            State.AddOutEdge(hasProfileEdge, profileVertex);

            return profileVertex;
        }

        #endregion
    }
}