#region Using Directives

using System;
using System.Diagnostics;
using JetBrains.Annotations;

#endregion

namespace Orleans.Graph.Test.Definition
{
    /// <summary>
    /// </summary>
    [DebuggerDisplay("FirstName: {FirstName,nq}, LastName: {LastName,nq}")]
    public class PersonalData
    {
        /// <summary>Initializes a new instance of the <see cref="T:PersonalData" /> class.</summary>
        public PersonalData() { }

        /// <summary>Initializes a new instance of the <see cref="T:PersonalData" /> class.</summary>
        public PersonalData([NotNull] string firstName, [NotNull] string lastName)
        {
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
        }

        /// <summary>
        /// </summary>
        [CanBeNull]
        public string FirstName { get; set; }

        /// <summary>
        /// </summary>
        [CanBeNull]
        public string MiddleName { get; set; }

        /// <summary>
        /// </summary>
        [CanBeNull]
        public string LastName { get; set; }

        /// <summary>
        /// </summary>
        public DateTime Birthdate { get; set; }
    }
}