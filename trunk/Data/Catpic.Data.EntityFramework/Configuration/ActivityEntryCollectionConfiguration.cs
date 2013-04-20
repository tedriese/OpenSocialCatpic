// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActivityEntryCollectionConfiguration.cs" company="Catpic Software">
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
    public class ActivityEntryCollectionConfiguration : EntityTypeConfiguration<EntityActivityEntryCollection>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityEntryCollectionConfiguration"/> class.
        /// </summary>
        public ActivityEntryCollectionConfiguration()
        {
            ToTable("ActivityEntryCollections");
            HasKey(t => t.Id);
            Property(p => p.Id).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
        }
    }
}
