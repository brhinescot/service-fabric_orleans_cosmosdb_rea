#region Using Directives

using System.Threading.Tasks;
using JetBrains.Annotations;
using Orleans.Graph.Definition;

#endregion

namespace Orleans.Graph.Test.Definition
{
    /// <summary>
    /// 
    /// </summary>
    [GraphElement(DefaultPartition = "users")]
    public interface IPersonVertex : IVertex
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
        Task<IProfileVertex> AddProfileAsync(ProfileData data);
    }
}