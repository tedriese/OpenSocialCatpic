// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CatpicContext.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Represents EF context which uses "Code First" approach
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Data.EntityFramework
{
    using System;
    using System.Data.Entity;

    using Catpic.Data.EntityFramework.Configuration;
    using Catpic.Data.EntityFramework.Repositories;
    
    /// <summary>
    /// Represents EF context which uses "Code First" approach
    /// </summary>
    public class CatpicContext : DbContext
    {
        /// <summary>
        /// Thread-safe instance of context
        /// </summary>
        [ThreadStatic]
        private static CatpicContext _current;

        /// <summary>
        /// Initializes a new instance of the <see cref="CatpicContext"/> class.
        /// </summary>
        public CatpicContext()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CatpicContext"/> class.
        /// </summary>
        /// <param name="connectionString"> The connection string. </param>
        public CatpicContext(string connectionString)
            : base(connectionString)
        {
        }

        /// <summary>
        /// Gets or sets Persons.
        /// </summary>
        public DbSet<EntityPerson> Persons { get; set; }

        /// <summary>
        /// Gets or sets Activities.
        /// </summary>
        public DbSet<EntityActivityEntry> Activities { get; set; }

        /// <summary>
        /// Gets or sets Messages.
        /// </summary>
        public DbSet<EntityMessage> Messages { get; set; }

        /// <summary>
        /// Gets or sets Groups.
        /// </summary>
        public DbSet<EntityGroup> Groups { get; set; }

        /// <summary>
        /// Gets or sets PersonCollections.
        /// </summary>
        public DbSet<EntityPersonCollection> PersonCollections { get; set; }

        /// <summary>
        /// Gets or sets ActivityEntryCollections.
        /// </summary>
        public DbSet<EntityActivityEntryCollection> ActivityEntryCollections { get; set; }

        /// <summary>
        /// Gets or sets MessageCollections.
        /// </summary>
        public DbSet<EntityMessageCollection> MessageCollections { get; set; }

        /// <summary>
        /// Gets or sets GroupCollections.
        /// </summary>
        public DbSet<EntityGroupCollection> GroupCollections { get; set; }

        /// <summary>
        /// Gets thread-safe instance of context.
        /// </summary>
        /// <param name="connectionString"> The connection String. </param>
        /// <returns> The current. </returns>
        public static CatpicContext Current(string connectionString)
        {
            return _current ?? (_current = connectionString == null ? 
                                   new CatpicContext() : 
                                   new CatpicContext(connectionString));
        }

        /// <summary>
        /// Provides the way to configure EF mapping
        /// </summary>
        /// <param name="modelBuilder"> The model builder. </param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new PersonConfiguration());
            modelBuilder.Configurations.Add(new PersonCollectionConfiguration());
            modelBuilder.Configurations.Add(new MessageConfiguration());
            modelBuilder.Configurations.Add(new MessageCollectionConfiguration());
            modelBuilder.Configurations.Add(new ActivityEntryConfiguration());
            modelBuilder.Configurations.Add(new ActivityEntryCollectionConfiguration());
            modelBuilder.Configurations.Add(new GroupConfiguration());
            modelBuilder.Configurations.Add(new GroupCollectionConfiguration());
        }
    }
}
