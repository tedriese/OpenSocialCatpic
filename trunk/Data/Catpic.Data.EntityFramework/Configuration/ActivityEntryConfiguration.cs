﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActivityEntryConfiguration.cs" company="Catpic Software">
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
    public class ActivityEntryConfiguration : EntityTypeConfiguration<EntityActivityEntry>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityEntryConfiguration"/> class.
        /// </summary>
        public ActivityEntryConfiguration()
        {
            ToTable("ActivityEntries");
            HasKey(t => t.Id);
            Property(p => p.Id).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
        }
    }
}
