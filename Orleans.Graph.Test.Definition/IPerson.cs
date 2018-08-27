#region Using Directives

using System.Threading.Tasks;
using JetBrains.Annotations;
using Orleans.Graph.Definition;
using ReaService.Orleans.Definition;

#endregion

namespace Orleans.Graph.Test.Definition
{
    /// <summary>
    /// 
    /// </summary>
    [GraphElement(DefaultPartition = "users")]
    public interface IPerson : IVertex
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task SetPersonalDataAsync([NotNull] PersonalData data);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<PersonalData> GetPersonalDataAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<IProfile> AddProfileAsync(ProfileData data);
    }
    
    [GraphElement(DefaultPartition = "organizations")]
    public interface IOrganization : IAgent
    {
        Task AddPerson(IPerson person);
    }
}