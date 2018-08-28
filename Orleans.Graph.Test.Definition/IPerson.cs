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
        [NotNull]
        Task<PersonalData> GetPersonalDataAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [NotNull]
        Task<IProfile> AddProfileAsync([NotNull] ProfileData data);
    }
    
    [GraphElement(DefaultPartition = "organizations")]
    public interface IOrganization : IAgent
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        Task AddPerson([NotNull] IPerson person);
    }
}