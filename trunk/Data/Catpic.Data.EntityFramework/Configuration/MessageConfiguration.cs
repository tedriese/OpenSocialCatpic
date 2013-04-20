// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageConfiguration.cs" company="Catpic Software">
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
    public class MessageConfiguration : EntityTypeConfiguration<EntityMessage>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageConfiguration"/> class.
        /// </summary>
        public MessageConfiguration()
        {
            ToTable("Messages");
            HasKey(t => t.Id);
            Property(p => p.Id).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
        }
    }
}
