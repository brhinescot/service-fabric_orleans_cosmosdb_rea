#region Using Directives

using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Orleans.Graph.Test.Definition;
using ReaService.Orleans;

#endregion

namespace Orleans.Graph.Test
{
    [UsedImplicitly]
    public class OrganizationAgent : AgentGrain, IOrganization
    {
        Task IOrganization.AddPerson(IPerson person)
        {
            if (person == null)
                throw new ArgumentNullException(nameof(person));

            State["personId"] = person.ToKeyString();
            return WriteStateAsync();
        }
    }
}