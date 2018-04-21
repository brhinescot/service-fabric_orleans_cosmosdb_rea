#region Using Directives

using System;

#endregion

namespace Orleans.Graph
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    public class GraphElementAttribute : Attribute
    {
        #region Properties

        /// <summary>
        /// Represents the default partition for all activations of this grain type. 
        /// </summary>
        /// <remarks>
        /// This value may be overridden when calling
        /// <see cref="GrainExtensions.GetVertexGrain{TGrainInterface}"/> -or-
        /// <see cref="GrainExtensions.GetVertexGrain{TGrainInterface}(Orleans.IGrainFactory,string,string)"/> and 
        /// passing in the partition key.
        /// </remarks>
        public string DefaultPartition { get; set; }
        
        /// <summary>
        /// The label for the graph element.
        /// </summary>
        /// <remarks>
        /// If this property is not set the label is extracted from the name of the interface, i.e. IPersonGrain will 
        /// have a label of 'person' as will IPersonVertex.
        /// </remarks>
        public string Label { get; set; }

        /// <summary>
        /// When applied to a grain interface representing a vertex, determines if the vertex edges are read during grain activation.
        /// </summary>
        public bool ReadVertexEdges { get; set; }

        #endregion
    }
}