#region Using Directives

using System;
using System.Reflection;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Orleans.Graph.Definition;
using Orleans.Graph.Edge;
using Orleans.Graph.State;
using Orleans.Graph.Vertex;

#endregion

namespace Orleans.Graph
{
    /// <summary>
    /// </summary>
    public static class GrainExtensions
    {
        private static readonly Regex LabelRegEx = new Regex(@"^I(\w+?)(?:Grain|Actor|Vertex|Edge|\r?$)", RegexOptions.Compiled | RegexOptions.Singleline);

        /// <summary>
        /// </summary>
        /// <param name="grain"></param>
        /// <returns></returns>
        [NotNull]
        public static string GetGraphLabel(this IGraphElementGrain grain)
        {
            grain.GetPrimaryKey(out var keyExt);
            var split = keyExt.Split('|');
            return split[0];
        }

        /// <summary>
        /// </summary>
        /// <param name="grain"></param>
        /// <returns></returns>
        [CanBeNull]
        public static string GetGraphPartition(this IGraphElementGrain grain)
        {
            grain.GetPrimaryKey(out var keyExt);
            var split = keyExt.Split('|');
            return split.Length == 2 ? split[1] : null;
        }

        /// <summary>
        /// </summary>
        /// <param name="grain"></param>
        /// <returns></returns>
        [NotNull]
        public static string GetGraphRuntimeIdString(this IGraphElementGrain grain) => grain.GetGraphRuntimeId().ToString();

        /// <summary>
        /// </summary>
        /// <param name="grain"></param>
        /// <returns></returns>
        public static Guid GetGraphRuntimeId(this IGraphElementGrain grain) => grain.GetPrimaryKey(out _);

        /// <summary>
        /// </summary>
        /// <param name="grain"></param>
        /// <returns></returns>
        public static string ToKeyString(this IGraphElementGrain grain)
        {
            const int trimStartLength = 14;
            const int trimEndLength = 11;
            const string prefix = "GrainReference=";

            string identityString = grain.GetGrainIdentity().IdentityString;
            return prefix + identityString.Substring(trimStartLength, (identityString.Length - trimEndLength) - trimStartLength);
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="TGrainInterface"></typeparam>
        /// <param name="grainFactory"></param>
        /// <param name="primaryKeyGuid"></param>
        /// <param name="partition"></param>
        /// <returns></returns>
        public static TGrainInterface GetVertexGrain<TGrainInterface>(this IGrainFactory grainFactory, [NotNull] string primaryKeyGuid, string partition = null) where TGrainInterface : IVertex
        {
            if (primaryKeyGuid == null)
                throw new ArgumentNullException(nameof(primaryKeyGuid));

            return grainFactory.GetVertexGrain<TGrainInterface>(Guid.Parse(primaryKeyGuid), partition);
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="TGrainInterface"></typeparam>
        /// <param name="grainFactory"></param>
        /// <param name="primaryKey"></param>
        /// <param name="partition"></param>
        /// <returns></returns>
        [NotNull]
        public static TGrainInterface GetVertexGrain<TGrainInterface>(this IGrainFactory grainFactory, Guid primaryKey, string partition = null) where TGrainInterface : IVertex
        {
            return grainFactory.GetGrain<TGrainInterface>(primaryKey, GetKeyExtension(typeof(TGrainInterface), partition), null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TGrainInterface"></typeparam>
        /// <param name="grainFactory"></param>
        /// <param name="primaryKeyGuid"></param>
        /// <param name="outVertex"></param>
        /// <returns></returns>
        public static TGrainInterface GetEdgeGrain<TGrainInterface>(this IGrainFactory grainFactory, [NotNull] string primaryKeyGuid, IVertex outVertex) where TGrainInterface : IEdge
        {
            if (primaryKeyGuid == null)
                throw new ArgumentNullException(nameof(primaryKeyGuid));

            return grainFactory.GetEdgeGrain<TGrainInterface>(Guid.Parse(primaryKeyGuid), outVertex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TGrainInterface"></typeparam>
        /// <param name="grainFactory"></param>
        /// <param name="primaryKey"></param>
        /// <param name="outVertex"></param>
        /// <returns></returns>
        [NotNull]
        public static TGrainInterface GetEdgeGrain<TGrainInterface>(this IGrainFactory grainFactory, Guid primaryKey, IVertex outVertex) where TGrainInterface : IEdge
        {
            return grainFactory.GetGrain<TGrainInterface>(primaryKey, GetKeyExtension(typeof(TGrainInterface), outVertex.GetGraphPartition()), null);
        }
        
        private static string GetKeyExtension(MemberInfo grainMemberInfo, string partition = null)
        {
            string defaultLabel = null;
            string defaultPartition = null;

            var attributes = grainMemberInfo.GetCustomAttributes(typeof(GraphElementAttribute), true);
            if (attributes.Length == 1 && attributes[0] is GraphElementAttribute attribute)
            {
                defaultLabel = attribute.Label;
                defaultPartition = attribute.DefaultPartition;
            }
            
            if(string.IsNullOrWhiteSpace(defaultLabel))
                defaultLabel = LabelRegEx.Match(grainMemberInfo.Name).Groups[1].Value.LowercaseFirst();

            if (!string.IsNullOrWhiteSpace(partition))
                defaultPartition = partition;

            return !string.IsNullOrWhiteSpace(defaultPartition) ? $"{defaultLabel}|{defaultPartition}" : defaultLabel;
        }
    }
}