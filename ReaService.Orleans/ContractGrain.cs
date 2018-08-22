#region Using Directives

using Orleans;
using ReaService.Orleans.Definition;

#endregion

namespace ReaService.Orleans
{
    public class ContractGrain : Grain, IContract { }
}