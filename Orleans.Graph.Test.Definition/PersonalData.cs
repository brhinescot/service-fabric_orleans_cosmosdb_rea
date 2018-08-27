#region Using Directives

using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using JetBrains.Annotations;

#endregion

namespace Orleans.Graph.Test.Definition
{
    /// <summary>
    /// PersonalData
    /// </summary>
    [DebuggerDisplay("FirstName: {FirstName,nq}, LastName: {LastName,nq}")]
    public class PersonalData
    {
        /// <summary>
        /// </summary>
        public DateTimeOffset Birthdate { get; set; }

        /// <summary>
        /// The first name
        /// </summary>
        [Required]
        [NotNull]
        public string FirstName { get; set; }

        /// <summary>
        /// The last name.
        /// </summary>
        [Required]
        [NotNull]
        public string LastName { get; set; }

        /// <summary>
        /// </summary>
        [CanBeNull]
        public string MiddleName { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:PersonalData" /> class.
        /// </summary>
        public PersonalData([NotNull] string firstName, [NotNull] string lastName)
        {
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
        }
    }
}