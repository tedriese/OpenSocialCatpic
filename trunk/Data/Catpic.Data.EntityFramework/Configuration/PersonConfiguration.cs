// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonConfiguration.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Configures EF metadata
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Data.EntityFramework.Configuration
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.ModelConfiguration;

    using Catpic.Data.EntityFramework.Repositories;

    /// <summary>
    /// Configures EF metadata
    /// </summary>
    public class PersonConfiguration : EntityTypeConfiguration<EntityPerson>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PersonConfiguration"/> class.
        /// </summary>
        public PersonConfiguration()
        {
            ToTable("Persons");
            HasKey(t => t.Id);
            Property(p => p.Id).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(p => p.DisplayName).IsRequired().HasMaxLength(128);
            Property(p => p.Nickname).HasMaxLength(128);
            Property(p => p.Age).HasMaxLength(16);
            Property(p => p.Birthday).HasMaxLength(64);
            Property(p => p.Gender).HasMaxLength(64);
        }
    }
}
